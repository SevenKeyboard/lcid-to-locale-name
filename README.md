# lcid-to-locale-name
Convert an LCID to a locale name on Windows 10+ with .NET 10.

```csharp
if (SevenKeyboard.LcidToLocaleName.TryGet(0x0409, out string name))
    Console.WriteLine(name); // en-US
```

Version 2.0.0 returns success separately from the name: failure returns `false` and
`string.Empty`; the invariant LCID (`0x007F`) succeeds with `""`. Use `SystemDefaultLcid`,
`UserDefaultLcid`, and `AllowNeutralNames` in place of AHK string tokens. Numeric
flags pass through unchanged. Only `cchName == LocaleNameMaxLength` (85) selects
a fixed buffer; every other value queries the required size, as in the AHK original.

Build: `dotnet build src/SevenKeyboard.LcidToLocaleName/SevenKeyboard.LcidToLocaleName.csproj -c Release`.
Smoke: `dotnet run --project tests/SevenKeyboard.LcidToLocaleName.Smoke -c Release`.
