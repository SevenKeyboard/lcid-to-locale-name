using System.Runtime.InteropServices;
using System.Text;
using SevenKeyboard;

[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32)]

int checks = 0;
int[] sizes = [0, 85, 1, 84, 86, -1, int.MinValue, int.MaxValue];
uint[] supportedFlags = [0, LcidToLocaleName.AllowNeutralNames];

// Expectations verified against the original AHK wrapper, including its non-authoritative sizes.
(uint Locale, string Name)[] specific =
[
    (0x0409, "en-US"), (0x0411, "ja-JP"), (0x0412, "ko-KR"),
    (0x10407, "de-DE_phoneb"), (0x007F, "")
];
(uint Locale, string Neutral, string Specific)[] neutral =
[
    (0x0009, "en", "en-US"), (0x0004, "zh-Hans", "zh-CN"), (0x7C04, "zh-Hant", "zh-HK")
];

var systemName = new StringBuilder(85);
var userName = new StringBuilder(85);
Require(Native.GetSystemDefaultLocaleName(systemName, systemName.Capacity) > 0, "GetSystemDefaultLocaleName failed.");
Require(Native.GetUserDefaultLocaleName(userName, userName.Capacity) > 0, "GetUserDefaultLocaleName failed.");

foreach (int size in sizes)
{
    foreach ((uint locale, string expected) in specific)
        foreach (uint flags in supportedFlags)
            Check(locale, size, flags, expected);

    foreach ((uint locale, string neutralName, string specificName) in neutral)
    {
        Check(locale, size, 0, specificName);
        Check(locale, size, LcidToLocaleName.AllowNeutralNames, neutralName);
    }

    foreach (uint flags in supportedFlags)
    {
        Check(LcidToLocaleName.SystemDefaultLcid, size, flags, systemName.ToString());
        Check(LcidToLocaleName.UserDefaultLcid, size, flags, userName.ToString());
        Check(uint.MaxValue, size, flags, null);
    }

    // LCID-to-name forwards arbitrary flags, so invalid flags fail natively.
    foreach (uint flags in new uint[] { 1, 0x08000001, uint.MaxValue })
    {
        Check(0x0409, size, flags, null);
        Check(0x0009, size, flags, null);
        Check(0x007F, size, flags, null);
    }
}

Require(LcidToLocaleName.TryGet(0x0409, out string defaultName) && defaultName == "en-US",
    "Omitted size/flags must retain the original defaults.");
Console.WriteLine($"PASS: {checks} checks ({RuntimeInformation.ProcessArchitecture}); system={systemName}, user={userName}.");

void Check(uint locale, int size, uint flags, string? expected)
{
    // A separate, single-call native query checks the AHK-derived expectations.
    var buffer = new StringBuilder(85);
    int length = Native.LCIDToLocaleName(locale, buffer, buffer.Capacity, flags);
    Require((length != 0) == (expected is not null) && (length == 0 || buffer.ToString() == expected),
        $"Native expectation mismatch for LCID 0x{locale:X}, flags 0x{flags:X}.");

    string actual = "stale";
    bool success = LcidToLocaleName.TryGet(locale, out actual, size, flags);
    Require(success == (expected is not null) && actual == (expected ?? string.Empty),
        $"LCID 0x{locale:X}, size {size}, flags 0x{flags:X}: success={success}, name=[{actual ?? "<null>"}], expected=[{expected ?? "<null>"}].");
}

void Require(bool condition, string message)
{
    if (!condition)
        throw new InvalidOperationException(message);
    checks++;
}

internal static class Native
{
    [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    internal static extern int LCIDToLocaleName(uint locale, StringBuilder name, int cchName, uint flags);

    [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    internal static extern int GetSystemDefaultLocaleName(StringBuilder name, int cchName);

    [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    internal static extern int GetUserDefaultLocaleName(StringBuilder name, int cchName);
}
