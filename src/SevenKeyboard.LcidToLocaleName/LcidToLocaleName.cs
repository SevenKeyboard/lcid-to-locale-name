//==============================================================
// LcidToLocaleName — Convert an LCID (or system/user default LCID) to a locale name string (e.g. "en-US")
//
// GitHub: https://github.com/SevenKeyboard/lcid-to-locale-name
// Author: SevenKeyboard Ltd. (2026)
// License: The Unlicense
//
// Documentation / References:
//   LCIDToLocaleName function (winnls.h)
//     https://learn.microsoft.com/en-us/windows/win32/api/winnls/nf-winnls-lcidtolocalename
//   GetSystemDefaultLocaleName function (winnls.h)
//     https://learn.microsoft.com/en-us/windows/win32/api/winnls/nf-winnls-getsystemdefaultlocalename
//   GetUserDefaultLocaleName function (winnls.h)
//     https://learn.microsoft.com/en-us/windows/win32/api/winnls/nf-winnls-getuserdefaultlocalename
//==============================================================
using System.Runtime.InteropServices;

[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32)]

namespace SevenKeyboard;

public static partial class LcidToLocaleName
{
    public const uint SystemDefaultLcid = 0x0800;
    public const uint UserDefaultLcid = 0x0400;
    public const uint AllowNeutralNames = 0x08000000;
    public const int LocaleNameMaxLength = 85;

    /// <summary>Converts an LCID to a locale name; returns false and an empty string on native failure.</summary>
    /// <remarks>Only cchName == 85 selects a fixed buffer; all other values query the required size.</remarks>
    public static unsafe bool TryGet(uint locale, out string name, int cchName = 0, uint flags = 0)
    {
        name = string.Empty;
        int bufferSize = cchName == LocaleNameMaxLength
            ? LocaleNameMaxLength
            : NativeLCIDToLocaleName(locale, null, 0, flags);

        if (bufferSize <= 0)
            return false;

        char* buffer = stackalloc char[bufferSize];
        int length = NativeLCIDToLocaleName(locale, buffer, bufferSize, flags);
        if (length == 0)
            return false;

        name = new string(buffer, 0, length - 1);
        return true;
    }

    [LibraryImport("kernel32.dll", EntryPoint = "LCIDToLocaleName")]
    private static unsafe partial int NativeLCIDToLocaleName(uint locale, char* name, int cchName, uint flags);
}
