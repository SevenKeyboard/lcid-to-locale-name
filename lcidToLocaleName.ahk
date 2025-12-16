#Requires AutoHotkey v1.1.0+
;==============================================================
; lcidToLocaleName — Convert an LCID (or system/user default LCID) to a locale name string (e.g. "en-US")
;
; GitHub: https://github.com/SevenKeyboard/lcid-to-locale-name
; Author: SevenKeyboard Ltd. (2025)
; License: The Unlicense
;
; Documentation / References:
;   LCIDToLocaleName function (winnls.h)
;     https://learn.microsoft.com/en-us/windows/win32/api/winnls/nf-winnls-lcidtolocalename
;   GetSystemDefaultLocaleName function (winnls.h)
;     https://learn.microsoft.com/en-us/windows/win32/api/winnls/nf-winnls-getsystemdefaultlocalename
;   GetUserDefaultLocaleName function (winnls.h)
;     https://learn.microsoft.com/en-us/windows/win32/api/winnls/nf-winnls-getuserdefaultlocalename
;==============================================================
class VersionManager_lcidToLocaleName
{
    static _ := VersionManager_lcidToLocaleName._init()
    _init()    {
        global
        LCIDTOLOCALENAME_VERSION := "1.0.0"
    }
}
lcidToLocaleName(Locale, cchName:=0, dwFlags:=0)    {
    static LOCALE_SYSTEM_DEFAULT:=0x0800, LOCALE_USER_DEFAULT:=0x0400
            ,LOCALE_NAME_MAX_LENGTH:=85
            ,LOCALE_ALLOW_NEUTRAL_NAMES:=0x08000000
    if (Locale="LOCALE_SYSTEM_DEFAULT"||Locale="LOCALE_USER_DEFAULT")
        Locale:=%Locale%
    if (dwFlags="LOCALE_ALLOW_NEUTRAL_NAMES")
        dwFlags:=%dwFlags%
    bufferSize:=(cchName="LOCALE_NAME_MAX_LENGTH"||cchName==LOCALE_NAME_MAX_LENGTH)
                ?LOCALE_NAME_MAX_LENGTH
                :dllCall("Kernel32.dll\LCIDToLocaleName", "UInt",Locale, "Ptr",0, "Int",0, "UInt",dwFlags, "Int")
    varSetCapacity(lpName, bufferSize*A_PtrSize, 0)
    length:=dllCall("Kernel32.dll\LCIDToLocaleName", "UInt",Locale, "Ptr",&lpName, "Int",cchName:=bufferSize, "UInt",dwFlags, "Int")
    return strGet(&lpName, length, "UTF-16")
}