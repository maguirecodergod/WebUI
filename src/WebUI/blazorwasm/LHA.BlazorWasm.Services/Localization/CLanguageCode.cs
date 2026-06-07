namespace LHA.BlazorWasm.Services.Localization;

/// <summary>
/// A comprehensive enumeration of world languages (ISO 639-1 basis).
/// Used by application modules to pick which languages to display in the selector.
/// </summary>
public enum CLanguageCode
{
    /// <summary>
    /// 0 - EN
    /// </summary>
    EN, // English
    /// <summary>
    /// 1 - VI
    /// </summary>
    VI, // Vietnamese
    /// <summary>
    /// 2 - FR
    /// </summary>
    FR, // French
    /// <summary>
    /// 3 - ES
    /// </summary>
    ES, // Spanish
    /// <summary>
    /// 4 - DE
    /// </summary>
    DE, // German
    /// <summary>
    /// 5 - IT
    /// </summary>
    IT, // Italian
    /// <summary>
    /// 6 - PT
    /// </summary>
    PT, // Portuguese
    /// <summary>
    /// 7 - RU
    /// </summary>
    RU, // Russian
    /// <summary>
    /// 8 - ZH
    /// </summary>
    ZH, // Chinese
    /// <summary>
    /// 9 - JA
    /// </summary>
    JA, // Japanese
    /// <summary>
    /// 10 - KO
    /// </summary>
    KO, // Korean
    /// <summary>
    /// 11 - AR
    /// </summary>
    AR, // Arabic
    /// <summary>
    /// 12 - HI
    /// </summary>
    HI, // Hindi
    /// <summary>
    /// 13 - BN
    /// </summary>
    BN, // Bengali
    /// <summary>
    /// 14 - PA
    /// </summary>
    PA, // Punjabi
    /// <summary>
    /// 15 - JV
    /// </summary>
    JV, // Javanese
    /// <summary>
    /// 16 - MS
    /// </summary>
    MS, // Malay
    /// <summary>
    /// 17 - ID
    /// </summary>
    ID, // Indonesian
    /// <summary>
    /// 18 - TR
    /// </summary>
    TR, // Turkish
    /// <summary>
    /// 19 - TH
    /// </summary>
    TH, // Thai
    /// <summary>
    /// 20 - PL
    /// </summary>
    PL, // Polish
    /// <summary>
    /// 21 - UK
    /// </summary>
    UK, // Ukrainian
    /// <summary>
    /// 22 - NL
    /// </summary>
    NL, // Dutch
    /// <summary>
    /// 23 - SV
    /// </summary>
    SV, // Swedish
    /// <summary>
    /// 24 - FI
    /// </summary>
    FI, // Finnish
    /// <summary>
    /// 25 - DA
    /// </summary>
    DA, // Danish
    /// <summary>
    /// 26 - NO
    /// </summary>
    NO, // Norwegian
    /// <summary>
    /// 27 - CS
    /// </summary>
    CS, // Czech
    /// <summary>
    /// 28 - SK
    /// </summary>
    SK, // Slovak
    /// <summary>
    /// 29 - HU
    /// </summary>
    HU, // Hungarian
    /// <summary>
    /// 30 - RO
    /// </summary>
    RO, // Romanian
    /// <summary>
    /// 31 - BG
    /// </summary>
    BG, // Bulgarian
    /// <summary>
    /// 32 - EL
    /// </summary>
    EL, // Greek
    /// <summary>
    /// 33 - HE
    /// </summary>
    HE, // Hebrew
    /// <summary>
    /// 34 - UR
    /// </summary>
    UR, // Urdu
    /// <summary>
    /// 35 - FA
    /// </summary>
    FA, // Persian
    /// <summary>
    /// 36 - TA
    /// </summary>
    TA, // Tamil
    /// <summary>
    /// 37 - TE
    /// </summary>
    TE, // Telugu
    /// <summary>
    /// 38 - MR
    /// </summary>
    MR, // Marathi
    /// <summary>
    /// 39 - GU
    /// </summary>
    GU, // Gujarati
    /// <summary>
    /// 40 - KN
    /// </summary>
    KN, // Kannada
    /// <summary>
    /// 41 - ML
    /// </summary>
    ML, // Malayalam
    /// <summary>
    /// 42 - SW
    /// </summary>
    SW, // Swahili
    /// <summary>
    /// 43 - AM
    /// </summary>
    AM, // Amharic
    /// <summary>
    /// 44 - YO
    /// </summary>
    YO, // Yoruba
    /// <summary>
    /// 45 - IG
    /// </summary>
    IG, // Igbo
    /// <summary>
    /// 46 - ZU
    /// </summary>
    ZU, // Zulu
    /// <summary>
    /// 47 - XH
    /// </summary>
    XH, // Xhosa
    /// <summary>
    /// 48 - AF
    /// </summary>
    AF, // Afrikaans
    /// <summary>
    /// 49 - SQ
    /// </summary>
    SQ, // Albanian
    /// <summary>
    /// 50 - HY
    /// </summary>
    HY, // Armenian
    /// <summary>
    /// 51 - AZ
    /// </summary>
    AZ, // Azerbaijani
    /// <summary>
    /// 52 - EU
    /// </summary>
    EU, // Basque
    /// <summary>
    /// 53 - BE
    /// </summary>
    BE, // Belarusian
    /// <summary>
    /// 54 - BS
    /// </summary>
    BS, // Bosnian
    /// <summary>
    /// 55 - CA
    /// </summary>
    CA, // Catalan
    /// <summary>
    /// 56 - HR
    /// </summary>
    HR, // Croatian
    /// <summary>
    /// 57 - ET
    /// </summary>
    ET, // Estonian
    /// <summary>
    /// 58 - TL
    /// </summary>
    TL, // Filipino
    /// <summary>
    /// 59 - GL
    /// </summary>
    GL, // Galician
    /// <summary>
    /// 60 - KA
    /// </summary>
    KA, // Georgian
    /// <summary>
    /// 61 - HT
    /// </summary>
    HT, // Haitian Creole
    /// <summary>
    /// 62 - IS
    /// </summary>
    IS, // Icelandic
    /// <summary>
    /// 63 - GA
    /// </summary>
    GA, // Irish
    /// <summary>
    /// 64 - KK
    /// </summary>
    KK, // Kazakh
    /// <summary>
    /// 65 - KM
    /// </summary>
    KM, // Khmer
    /// <summary>
    /// 66 - LO
    /// </summary>
    LO, // Lao
    /// <summary>
    /// 67 - LV
    /// </summary>
    LV, // Latvian
    /// <summary>
    /// 68 - LT
    /// </summary>
    LT, // Lithuanian
    /// <summary>
    /// 69 - MK
    /// </summary>
    MK, // Macedonian
    /// <summary>
    /// 70 - MT
    /// </summary>
    MT, // Maltese
    /// <summary>
    /// 71 - MN
    /// </summary>
    MN, // Mongolian
    /// <summary>
    /// 72 - NE
    /// </summary>
    NE, // Nepali
    /// <summary>
    /// 73 - SR
    /// </summary>
    SR, // Serbian
    /// <summary>
    /// 74 - SI
    /// </summary>
    SI, // Sinhala
    /// <summary>
    /// 75 - SL
    /// </summary>
    SL, // Slovenian
    /// <summary>
    /// 76 - TG
    /// </summary>
    TG, // Tajik
    /// <summary>
    /// 77 - UZ
    /// </summary>
    UZ, // Uzbek
    /// <summary>
    /// 78 - CY
    /// </summary>
    CY, // Welsh
    /// <summary>
    /// 79 - YI
    /// </summary>
    YI, // Yiddish
}
