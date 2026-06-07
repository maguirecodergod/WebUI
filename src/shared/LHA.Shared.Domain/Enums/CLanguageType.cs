using System.Reflection;

namespace LHA.Shared.Domain
{
    /// <summary>
    /// Các loại ngôn ngữ
    /// </summary>
    public enum CLanguageType
    {
        /// <summary>
        /// 1 - English
        /// </summary>
        [CultureCode("en")]
        English = 1,
        /// <summary>
        /// 2 - Spanish
        /// </summary>
        [CultureCode("es")]
        Spanish = 2,
        /// <summary>
        /// 3 - French
        /// </summary>
        [CultureCode("fr")]
        French = 3,
        /// <summary>
        /// 4 - German
        /// </summary>
        [CultureCode("de")]
        German = 4,
        /// <summary>
        /// 5 - Italian
        /// </summary>
        [CultureCode("it")]
        Italian = 5,
        /// <summary>
        /// 6 - Portuguese
        /// </summary>
        [CultureCode("pt")]
        Portuguese = 6,
        /// <summary>
        /// 7 - Russian
        /// </summary>
        [CultureCode("ru")]
        Russian = 7,
        /// <summary>
        /// 8 - Chinese
        /// </summary>
        [CultureCode("zh")]
        Chinese = 8,
        /// <summary>
        /// 9 - Japanese
        /// </summary>
        [CultureCode("ja")]
        Japanese = 9,
        /// <summary>
        /// 10 - Korean
        /// </summary>
        [CultureCode("ko")]
        Korean = 10,
        /// <summary>
        /// 11 - Arabic
        /// </summary>
        [CultureCode("ar")]
        Arabic = 11,
        /// <summary>
        /// 12 - Hindi
        /// </summary>
        [CultureCode("hi")]
        Hindi = 12,
        /// <summary>
        /// 13 - Bengali
        /// </summary>
        [CultureCode("bn")]
        Bengali = 13,
        /// <summary>
        /// 14 - Urdu
        /// </summary>
        [CultureCode("ur")]
        Urdu = 14,
        /// <summary>
        /// 15 - Turkish
        /// </summary>
        [CultureCode("tr")]
        Turkish = 15,
        /// <summary>
        /// 16 - Vietnamese
        /// </summary>
        [CultureCode("vi")]
        Vietnamese = 16,
        /// <summary>
        /// 17 - Thai
        /// </summary>
        [CultureCode("th")]
        Thai = 17,
        /// <summary>
        /// 18 - Persian
        /// </summary>
        [CultureCode("fa")]
        Persian = 18,
        /// <summary>
        /// 19 - Malay
        /// </summary>
        [CultureCode("ms")]
        Malay = 19,
        /// <summary>
        /// 20 - Indonesian
        /// </summary>
        [CultureCode("id")]
        Indonesian = 20,
        /// <summary>
        /// 21 - Swahili
        /// </summary>
        [CultureCode("sw")]
        Swahili = 21,
        /// <summary>
        /// 22 - Dutch
        /// </summary>
        [CultureCode("nl")]
        Dutch = 22,
        /// <summary>
        /// 23 - Greek
        /// </summary>
        [CultureCode("el")]
        Greek = 23,
        /// <summary>
        /// 24 - Hebrew
        /// </summary>
        [CultureCode("he")]
        Hebrew = 24,
        /// <summary>
        /// 25 - Polish
        /// </summary>
        [CultureCode("pl")]
        Polish = 25,
        /// <summary>
        /// 26 - Czech
        /// </summary>
        [CultureCode("cs")]
        Czech = 26,
        /// <summary>
        /// 27 - Slovak
        /// </summary>
        [CultureCode("sk")]
        Slovak = 27,
        /// <summary>
        /// 28 - Hungarian
        /// </summary>
        [CultureCode("hu")]
        Hungarian = 28,
        /// <summary>
        /// 29 - Romanian
        /// </summary>
        [CultureCode("ro")]
        Romanian = 29,
        /// <summary>
        /// 30 - Bulgarian
        /// </summary>
        [CultureCode("bg")]
        Bulgarian = 30,
        /// <summary>
        /// 31 - Serbian
        /// </summary>
        [CultureCode("sr")]
        Serbian = 31,
        /// <summary>
        /// 32 - Croatian
        /// </summary>
        [CultureCode("hr")]
        Croatian = 32,
        /// <summary>
        /// 33 - Finnish
        /// </summary>
        [CultureCode("fi")]
        Finnish = 33,
        /// <summary>
        /// 34 - Norwegian
        /// </summary>
        [CultureCode("no")]
        Norwegian = 34,
        /// <summary>
        /// 35 - Swedish
        /// </summary>
        [CultureCode("sv")]
        Swedish = 35,
        /// <summary>
        /// 36 - Danish
        /// </summary>
        [CultureCode("da")]
        Danish = 36,
        /// <summary>
        /// 37 - Icelandic
        /// </summary>
        [CultureCode("is")]
        Icelandic = 37,
        /// <summary>
        /// 38 - Filipino
        /// </summary>
        [CultureCode("tl")]
        Filipino = 38,
        /// <summary>
        /// 39 - Tamil
        /// </summary>
        [CultureCode("ta")]
        Tamil = 39,
        /// <summary>
        /// 40 - Telugu
        /// </summary>
        [CultureCode("te")]
        Telugu = 40,
        /// <summary>
        /// 41 - Marathi
        /// </summary>
        [CultureCode("mr")]
        Marathi = 41,
        /// <summary>
        /// 42 - Gujarati
        /// </summary>
        [CultureCode("gu")]
        Gujarati = 42,
        /// <summary>
        /// 43 - Punjabi
        /// </summary>
        [CultureCode("pa")]
        Punjabi = 43,
        /// <summary>
        /// 44 - Kannada
        /// </summary>
        [CultureCode("kn")]
        Kannada = 44,
        /// <summary>
        /// 45 - Malayalam
        /// </summary>
        [CultureCode("ml")]
        Malayalam = 45,
        /// <summary>
        /// 46 - Sinhala
        /// </summary>
        [CultureCode("si")]
        Sinhala = 46,
        /// <summary>
        /// 47 - Nepali
        /// </summary>
        [CultureCode("ne")]
        Nepali = 47,
        /// <summary>
        /// 48 - Burmese
        /// </summary>
        [CultureCode("my")]
        Burmese = 48,
        /// <summary>
        /// 49 - Khmer
        /// </summary>
        [CultureCode("km")]
        Khmer = 49,
        /// <summary>
        /// 50 - Lao
        /// </summary>
        [CultureCode("lo")]
        Lao = 50
    }

    public static class CLanguageTypeExtensions
    {
        public static string ToCultureCode(this CLanguageType language)
        {
            var memberInfo = language.GetType().GetMember(language.ToString()).FirstOrDefault();
            var attribute = memberInfo?.GetCustomAttribute<CultureCodeAttribute>();
            return attribute?.Code ?? "en";
        }
    }
}