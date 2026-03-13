namespace LHA.BlazorWasm.Services.Localization;

/// <summary>
/// Static provider bridging the LanguageCode enum to the rich LanguageOption UI model (Culture, Name, Flag).
/// </summary>
public static class LanguageProvider
{
    private static readonly Dictionary<LanguageCode, LanguageOption> _languages = new()
    {
        { LanguageCode.EN, new LanguageOption { Culture = "en", Name = "English", Flag = "gb" } },
        { LanguageCode.VI, new LanguageOption { Culture = "vi", Name = "Tiếng Việt", Flag = "vn" } },
        { LanguageCode.FR, new LanguageOption { Culture = "fr", Name = "Français", Flag = "fr" } },
        { LanguageCode.ES, new LanguageOption { Culture = "es", Name = "Español", Flag = "es" } },
        { LanguageCode.DE, new LanguageOption { Culture = "de", Name = "Deutsch", Flag = "de" } },
        { LanguageCode.IT, new LanguageOption { Culture = "it", Name = "Italiano", Flag = "it" } },
        { LanguageCode.PT, new LanguageOption { Culture = "pt", Name = "Português", Flag = "pt" } },
        { LanguageCode.RU, new LanguageOption { Culture = "ru", Name = "Русский", Flag = "ru" } },
        { LanguageCode.ZH, new LanguageOption { Culture = "zh", Name = "中文", Flag = "cn" } },
        { LanguageCode.JA, new LanguageOption { Culture = "ja", Name = "日本語", Flag = "jp" } },
        { LanguageCode.KO, new LanguageOption { Culture = "ko", Name = "한국어", Flag = "kr" } },
        { LanguageCode.AR, new LanguageOption { Culture = "ar", Name = "العربية", Flag = "sa" } },
        { LanguageCode.HI, new LanguageOption { Culture = "hi", Name = "हिन्दी", Flag = "in" } },
        { LanguageCode.BN, new LanguageOption { Culture = "bn", Name = "বাংলা", Flag = "bd" } },
        { LanguageCode.PA, new LanguageOption { Culture = "pa", Name = "ਪੰਜਾਬੀ", Flag = "in" } },
        { LanguageCode.JV, new LanguageOption { Culture = "jv", Name = "Basa Jawa", Flag = "id" } },
        { LanguageCode.MS, new LanguageOption { Culture = "ms", Name = "Bahasa Melayu", Flag = "my" } },
        { LanguageCode.ID, new LanguageOption { Culture = "id", Name = "Bahasa Indonesia", Flag = "id" } },
        { LanguageCode.TR, new LanguageOption { Culture = "tr", Name = "Türkçe", Flag = "tr" } },
        { LanguageCode.TH, new LanguageOption { Culture = "th", Name = "ไทย", Flag = "th" } },
        { LanguageCode.PL, new LanguageOption { Culture = "pl", Name = "Polski", Flag = "pl" } },
        { LanguageCode.UK, new LanguageOption { Culture = "uk", Name = "Українська", Flag = "ua" } },
        { LanguageCode.NL, new LanguageOption { Culture = "nl", Name = "Nederlands", Flag = "nl" } },
        { LanguageCode.SV, new LanguageOption { Culture = "sv", Name = "Svenska", Flag = "se" } },
        { LanguageCode.FI, new LanguageOption { Culture = "fi", Name = "Suomi", Flag = "fi" } },
        { LanguageCode.DA, new LanguageOption { Culture = "da", Name = "Dansk", Flag = "dk" } },
        { LanguageCode.NO, new LanguageOption { Culture = "no", Name = "Norsk", Flag = "no" } },
        { LanguageCode.CS, new LanguageOption { Culture = "cs", Name = "Čeština", Flag = "cz" } },
        { LanguageCode.SK, new LanguageOption { Culture = "sk", Name = "Slovenčina", Flag = "sk" } },
        { LanguageCode.HU, new LanguageOption { Culture = "hu", Name = "Magyar", Flag = "hu" } },
        { LanguageCode.RO, new LanguageOption { Culture = "ro", Name = "Română", Flag = "ro" } },
        { LanguageCode.BG, new LanguageOption { Culture = "bg", Name = "Български", Flag = "bg" } },
        { LanguageCode.EL, new LanguageOption { Culture = "el", Name = "Ελληνικά", Flag = "gr" } },
        { LanguageCode.HE, new LanguageOption { Culture = "he", Name = "עברית", Flag = "il" } },
        { LanguageCode.UR, new LanguageOption { Culture = "ur", Name = "اردو", Flag = "pk" } },
        { LanguageCode.FA, new LanguageOption { Culture = "fa", Name = "فارسی", Flag = "ir" } },
        { LanguageCode.TA, new LanguageOption { Culture = "ta", Name = "தமிழ்", Flag = "in" } },
        { LanguageCode.TE, new LanguageOption { Culture = "te", Name = "తెలుగు", Flag = "in" } },
        { LanguageCode.MR, new LanguageOption { Culture = "mr", Name = "मराठी", Flag = "in" } },
        { LanguageCode.GU, new LanguageOption { Culture = "gu", Name = "ગુજરાતી", Flag = "in" } },
        { LanguageCode.KN, new LanguageOption { Culture = "kn", Name = "ಕನ್ನಡ", Flag = "in" } },
        { LanguageCode.ML, new LanguageOption { Culture = "ml", Name = "മലയാളം", Flag = "in" } },
        { LanguageCode.SW, new LanguageOption { Culture = "sw", Name = "Kiswahili", Flag = "ke" } },
        { LanguageCode.AM, new LanguageOption { Culture = "am", Name = "አማርኛ", Flag = "et" } },
        { LanguageCode.YO, new LanguageOption { Culture = "yo", Name = "Yorùbá", Flag = "ng" } },
        { LanguageCode.IG, new LanguageOption { Culture = "ig", Name = "Igbo", Flag = "ng" } },
        { LanguageCode.ZU, new LanguageOption { Culture = "zu", Name = "isiZulu", Flag = "za" } },
        { LanguageCode.XH, new LanguageOption { Culture = "xh", Name = "isiXhosa", Flag = "za" } },
        { LanguageCode.AF, new LanguageOption { Culture = "af", Name = "Afrikaans", Flag = "za" } },
        { LanguageCode.SQ, new LanguageOption { Culture = "sq", Name = "Shqip", Flag = "al" } },
        { LanguageCode.HY, new LanguageOption { Culture = "hy", Name = "Հայերեն", Flag = "am" } },
        { LanguageCode.AZ, new LanguageOption { Culture = "az", Name = "Azərbaycan", Flag = "az" } },
        { LanguageCode.EU, new LanguageOption { Culture = "eu", Name = "Euskara", Flag = "es" } },
        { LanguageCode.BE, new LanguageOption { Culture = "be", Name = "Беларуская", Flag = "by" } },
        { LanguageCode.BS, new LanguageOption { Culture = "bs", Name = "Bosanski", Flag = "ba" } },
        { LanguageCode.CA, new LanguageOption { Culture = "ca", Name = "Català", Flag = "es" } },
        { LanguageCode.HR, new LanguageOption { Culture = "hr", Name = "Hrvatski", Flag = "hr" } },
        { LanguageCode.ET, new LanguageOption { Culture = "et", Name = "Eesti", Flag = "ee" } },
        { LanguageCode.TL, new LanguageOption { Culture = "tl", Name = "Tagalog", Flag = "ph" } },
        { LanguageCode.GL, new LanguageOption { Culture = "gl", Name = "Galego", Flag = "es" } },
        { LanguageCode.KA, new LanguageOption { Culture = "ka", Name = "ქართული", Flag = "ge" } },
        { LanguageCode.HT, new LanguageOption { Culture = "ht", Name = "Kreyòl ayisyen", Flag = "ht" } },
        { LanguageCode.IS, new LanguageOption { Culture = "is", Name = "Íslenska", Flag = "is" } },
        { LanguageCode.GA, new LanguageOption { Culture = "ga", Name = "Gaeilge", Flag = "ie" } },
        { LanguageCode.KK, new LanguageOption { Culture = "kk", Name = "Қазақ", Flag = "kz" } },
        { LanguageCode.KM, new LanguageOption { Culture = "km", Name = "ខ្មែរ", Flag = "kh" } },
        { LanguageCode.LO, new LanguageOption { Culture = "lo", Name = "ລາວ", Flag = "la" } },
        { LanguageCode.LV, new LanguageOption { Culture = "lv", Name = "Latviešu", Flag = "lv" } },
        { LanguageCode.LT, new LanguageOption { Culture = "lt", Name = "Lietuvių", Flag = "lt" } },
        { LanguageCode.MK, new LanguageOption { Culture = "mk", Name = "Македонски", Flag = "mk" } },
        { LanguageCode.MT, new LanguageOption { Culture = "mt", Name = "Malti", Flag = "mt" } },
        { LanguageCode.MN, new LanguageOption { Culture = "mn", Name = "Монгол", Flag = "mn" } },
        { LanguageCode.NE, new LanguageOption { Culture = "ne", Name = "नेपाली", Flag = "np" } },
        { LanguageCode.SR, new LanguageOption { Culture = "sr", Name = "Српски", Flag = "rs" } },
        { LanguageCode.SI, new LanguageOption { Culture = "si", Name = "සිංහල", Flag = "lk" } },
        { LanguageCode.SL, new LanguageOption { Culture = "sl", Name = "Slovenščina", Flag = "si" } },
        { LanguageCode.TG, new LanguageOption { Culture = "tg", Name = "Тоҷикӣ", Flag = "tj" } },
        { LanguageCode.UZ, new LanguageOption { Culture = "uz", Name = "Oʻzbek", Flag = "uz" } },
        { LanguageCode.CY, new LanguageOption { Culture = "cy", Name = "Cymraeg", Flag = "gb" } },
        { LanguageCode.YI, new LanguageOption { Culture = "yi", Name = "ייִדיש", Flag = "il" } }
    };

    /// <summary>
    /// Gets the rich LanguageOption definition for a given LanguageCode enum.
    /// </summary>
    public static LanguageOption GetOption(LanguageCode code)
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
    public static List<LanguageOption> GetOptions(IEnumerable<LanguageCode> codes)
    {
        var list = new List<LanguageOption>();
        foreach (var code in codes)
        {
            list.Add(GetOption(code));
        }
        return list;
    }
}
