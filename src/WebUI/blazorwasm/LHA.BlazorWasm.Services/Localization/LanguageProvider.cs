namespace LHA.BlazorWasm.Services.Localization;

/// <summary>
/// Static provider bridging the CLanguageCode enum to the rich LanguageOption UI model (Culture, Name, Flag).
/// </summary>
public static class LanguageProvider
{
    private static readonly Dictionary<CLanguageCode, LanguageOption> _languages = new()
    {
        { CLanguageCode.EN, new LanguageOption { Culture = "en", Name = "English", Flag = "gb" } },
        { CLanguageCode.VI, new LanguageOption { Culture = "vi", Name = "Tiếng Việt", Flag = "vn" } },
        { CLanguageCode.FR, new LanguageOption { Culture = "fr", Name = "Français", Flag = "fr" } },
        { CLanguageCode.ES, new LanguageOption { Culture = "es", Name = "Español", Flag = "es" } },
        { CLanguageCode.DE, new LanguageOption { Culture = "de", Name = "Deutsch", Flag = "de" } },
        { CLanguageCode.IT, new LanguageOption { Culture = "it", Name = "Italiano", Flag = "it" } },
        { CLanguageCode.PT, new LanguageOption { Culture = "pt", Name = "Português", Flag = "pt" } },
        { CLanguageCode.RU, new LanguageOption { Culture = "ru", Name = "Русский", Flag = "ru" } },
        { CLanguageCode.ZH, new LanguageOption { Culture = "zh", Name = "中文", Flag = "cn" } },
        { CLanguageCode.JA, new LanguageOption { Culture = "ja", Name = "日本語", Flag = "jp" } },
        { CLanguageCode.KO, new LanguageOption { Culture = "ko", Name = "한국어", Flag = "kr" } },
        { CLanguageCode.AR, new LanguageOption { Culture = "ar", Name = "العربية", Flag = "sa" } },
        { CLanguageCode.HI, new LanguageOption { Culture = "hi", Name = "हिन्दी", Flag = "in" } },
        { CLanguageCode.BN, new LanguageOption { Culture = "bn", Name = "বাংলা", Flag = "bd" } },
        { CLanguageCode.PA, new LanguageOption { Culture = "pa", Name = "ਪੰਜਾਬੀ", Flag = "in" } },
        { CLanguageCode.JV, new LanguageOption { Culture = "jv", Name = "Basa Jawa", Flag = "id" } },
        { CLanguageCode.MS, new LanguageOption { Culture = "ms", Name = "Bahasa Melayu", Flag = "my" } },
        { CLanguageCode.ID, new LanguageOption { Culture = "id", Name = "Bahasa Indonesia", Flag = "id" } },
        { CLanguageCode.TR, new LanguageOption { Culture = "tr", Name = "Türkçe", Flag = "tr" } },
        { CLanguageCode.TH, new LanguageOption { Culture = "th", Name = "ไทย", Flag = "th" } },
        { CLanguageCode.PL, new LanguageOption { Culture = "pl", Name = "Polski", Flag = "pl" } },
        { CLanguageCode.UK, new LanguageOption { Culture = "uk", Name = "Українська", Flag = "ua" } },
        { CLanguageCode.NL, new LanguageOption { Culture = "nl", Name = "Nederlands", Flag = "nl" } },
        { CLanguageCode.SV, new LanguageOption { Culture = "sv", Name = "Svenska", Flag = "se" } },
        { CLanguageCode.FI, new LanguageOption { Culture = "fi", Name = "Suomi", Flag = "fi" } },
        { CLanguageCode.DA, new LanguageOption { Culture = "da", Name = "Dansk", Flag = "dk" } },
        { CLanguageCode.NO, new LanguageOption { Culture = "no", Name = "Norsk", Flag = "no" } },
        { CLanguageCode.CS, new LanguageOption { Culture = "cs", Name = "Čeština", Flag = "cz" } },
        { CLanguageCode.SK, new LanguageOption { Culture = "sk", Name = "Slovenčina", Flag = "sk" } },
        { CLanguageCode.HU, new LanguageOption { Culture = "hu", Name = "Magyar", Flag = "hu" } },
        { CLanguageCode.RO, new LanguageOption { Culture = "ro", Name = "Română", Flag = "ro" } },
        { CLanguageCode.BG, new LanguageOption { Culture = "bg", Name = "Български", Flag = "bg" } },
        { CLanguageCode.EL, new LanguageOption { Culture = "el", Name = "Ελληνικά", Flag = "gr" } },
        { CLanguageCode.HE, new LanguageOption { Culture = "he", Name = "עברית", Flag = "il" } },
        { CLanguageCode.UR, new LanguageOption { Culture = "ur", Name = "اردو", Flag = "pk" } },
        { CLanguageCode.FA, new LanguageOption { Culture = "fa", Name = "فارسی", Flag = "ir" } },
        { CLanguageCode.TA, new LanguageOption { Culture = "ta", Name = "தமிழ்", Flag = "in" } },
        { CLanguageCode.TE, new LanguageOption { Culture = "te", Name = "తెలుగు", Flag = "in" } },
        { CLanguageCode.MR, new LanguageOption { Culture = "mr", Name = "मराठी", Flag = "in" } },
        { CLanguageCode.GU, new LanguageOption { Culture = "gu", Name = "ગુજરાતી", Flag = "in" } },
        { CLanguageCode.KN, new LanguageOption { Culture = "kn", Name = "ಕನ್ನಡ", Flag = "in" } },
        { CLanguageCode.ML, new LanguageOption { Culture = "ml", Name = "മലയാളം", Flag = "in" } },
        { CLanguageCode.SW, new LanguageOption { Culture = "sw", Name = "Kiswahili", Flag = "ke" } },
        { CLanguageCode.AM, new LanguageOption { Culture = "am", Name = "አማርኛ", Flag = "et" } },
        { CLanguageCode.YO, new LanguageOption { Culture = "yo", Name = "Yorùbá", Flag = "ng" } },
        { CLanguageCode.IG, new LanguageOption { Culture = "ig", Name = "Igbo", Flag = "ng" } },
        { CLanguageCode.ZU, new LanguageOption { Culture = "zu", Name = "isiZulu", Flag = "za" } },
        { CLanguageCode.XH, new LanguageOption { Culture = "xh", Name = "isiXhosa", Flag = "za" } },
        { CLanguageCode.AF, new LanguageOption { Culture = "af", Name = "Afrikaans", Flag = "za" } },
        { CLanguageCode.SQ, new LanguageOption { Culture = "sq", Name = "Shqip", Flag = "al" } },
        { CLanguageCode.HY, new LanguageOption { Culture = "hy", Name = "Հայերեն", Flag = "am" } },
        { CLanguageCode.AZ, new LanguageOption { Culture = "az", Name = "Azərbaycan", Flag = "az" } },
        { CLanguageCode.EU, new LanguageOption { Culture = "eu", Name = "Euskara", Flag = "es" } },
        { CLanguageCode.BE, new LanguageOption { Culture = "be", Name = "Беларуская", Flag = "by" } },
        { CLanguageCode.BS, new LanguageOption { Culture = "bs", Name = "Bosanski", Flag = "ba" } },
        { CLanguageCode.CA, new LanguageOption { Culture = "ca", Name = "Català", Flag = "es" } },
        { CLanguageCode.HR, new LanguageOption { Culture = "hr", Name = "Hrvatski", Flag = "hr" } },
        { CLanguageCode.ET, new LanguageOption { Culture = "et", Name = "Eesti", Flag = "ee" } },
        { CLanguageCode.TL, new LanguageOption { Culture = "tl", Name = "Tagalog", Flag = "ph" } },
        { CLanguageCode.GL, new LanguageOption { Culture = "gl", Name = "Galego", Flag = "es" } },
        { CLanguageCode.KA, new LanguageOption { Culture = "ka", Name = "ქართული", Flag = "ge" } },
        { CLanguageCode.HT, new LanguageOption { Culture = "ht", Name = "Kreyòl ayisyen", Flag = "ht" } },
        { CLanguageCode.IS, new LanguageOption { Culture = "is", Name = "Íslenska", Flag = "is" } },
        { CLanguageCode.GA, new LanguageOption { Culture = "ga", Name = "Gaeilge", Flag = "ie" } },
        { CLanguageCode.KK, new LanguageOption { Culture = "kk", Name = "Қазақ", Flag = "kz" } },
        { CLanguageCode.KM, new LanguageOption { Culture = "km", Name = "ខ្មែរ", Flag = "kh" } },
        { CLanguageCode.LO, new LanguageOption { Culture = "lo", Name = "ລາວ", Flag = "la" } },
        { CLanguageCode.LV, new LanguageOption { Culture = "lv", Name = "Latviešu", Flag = "lv" } },
        { CLanguageCode.LT, new LanguageOption { Culture = "lt", Name = "Lietuvių", Flag = "lt" } },
        { CLanguageCode.MK, new LanguageOption { Culture = "mk", Name = "Македонски", Flag = "mk" } },
        { CLanguageCode.MT, new LanguageOption { Culture = "mt", Name = "Malti", Flag = "mt" } },
        { CLanguageCode.MN, new LanguageOption { Culture = "mn", Name = "Монгол", Flag = "mn" } },
        { CLanguageCode.NE, new LanguageOption { Culture = "ne", Name = "नेपाली", Flag = "np" } },
        { CLanguageCode.SR, new LanguageOption { Culture = "sr", Name = "Српски", Flag = "rs" } },
        { CLanguageCode.SI, new LanguageOption { Culture = "si", Name = "සිංහල", Flag = "lk" } },
        { CLanguageCode.SL, new LanguageOption { Culture = "sl", Name = "Slovenščina", Flag = "si" } },
        { CLanguageCode.TG, new LanguageOption { Culture = "tg", Name = "Тоҷикӣ", Flag = "tj" } },
        { CLanguageCode.UZ, new LanguageOption { Culture = "uz", Name = "Oʻzbek", Flag = "uz" } },
        { CLanguageCode.CY, new LanguageOption { Culture = "cy", Name = "Cymraeg", Flag = "gb" } },
        { CLanguageCode.YI, new LanguageOption { Culture = "yi", Name = "ייִדיש", Flag = "il" } }
    };

    /// <summary>
    /// Gets the rich LanguageOption definition for a given CLanguageCode enum.
    /// </summary>
    public static LanguageOption GetOption(CLanguageCode code)
    {
        if (_languages.TryGetValue(code, out var option))
        {
            return option;
        }

        // Fallback safety
        return new LanguageOption { Culture = code.ToString().ToLowerInvariant(), Name = code.ToString(), Flag = "un" }; // UN flag default
    }

    /// <summary>
    /// Builds a list of LanguageOptions from a provided list of enums.
    /// </summary>
    public static List<LanguageOption> GetOptions(IEnumerable<CLanguageCode> codes)
    {
        var list = new List<LanguageOption>();
        foreach (var code in codes)
        {
            list.Add(GetOption(code));
        }
        return list;
    }
}
