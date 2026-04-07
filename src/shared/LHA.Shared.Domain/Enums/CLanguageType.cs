using System.Linq;
using System.Reflection;

namespace LHA.Shared.Domain
{
    /// <summary>
    /// Các loại ngôn ngữ
    /// </summary>
    public enum CLanguageType
    {
        [CultureCode("en")]
        English = 1,
        [CultureCode("es")]
        Spanish = 2,
        [CultureCode("fr")]
        French = 3,
        [CultureCode("de")]
        German = 4,
        [CultureCode("it")]
        Italian = 5,
        [CultureCode("pt")]
        Portuguese = 6,
        [CultureCode("ru")]
        Russian = 7,
        [CultureCode("zh")]
        Chinese = 8,
        [CultureCode("ja")]
        Japanese = 9,
        [CultureCode("ko")]
        Korean = 10,
        [CultureCode("ar")]
        Arabic = 11,
        [CultureCode("hi")]
        Hindi = 12,
        [CultureCode("bn")]
        Bengali = 13,
        [CultureCode("ur")]
        Urdu = 14,
        [CultureCode("tr")]
        Turkish = 15,
        [CultureCode("vi")]
        Vietnamese = 16,
        [CultureCode("th")]
        Thai = 17,
        [CultureCode("fa")]
        Persian = 18,
        [CultureCode("ms")]
        Malay = 19,
        [CultureCode("id")]
        Indonesian = 20,
        [CultureCode("sw")]
        Swahili = 21,
        [CultureCode("nl")]
        Dutch = 22,
        [CultureCode("el")]
        Greek = 23,
        [CultureCode("he")]
        Hebrew = 24,
        [CultureCode("pl")]
        Polish = 25,
        [CultureCode("cs")]
        Czech = 26,
        [CultureCode("sk")]
        Slovak = 27,
        [CultureCode("hu")]
        Hungarian = 28,
        [CultureCode("ro")]
        Romanian = 29,
        [CultureCode("bg")]
        Bulgarian = 30,
        [CultureCode("sr")]
        Serbian = 31,
        [CultureCode("hr")]
        Croatian = 32,
        [CultureCode("fi")]
        Finnish = 33,
        [CultureCode("no")]
        Norwegian = 34,
        [CultureCode("sv")]
        Swedish = 35,
        [CultureCode("da")]
        Danish = 36,
        [CultureCode("is")]
        Icelandic = 37,
        [CultureCode("tl")]
        Filipino = 38,
        [CultureCode("ta")]
        Tamil = 39,
        [CultureCode("te")]
        Telugu = 40,
        [CultureCode("mr")]
        Marathi = 41,
        [CultureCode("gu")]
        Gujarati = 42,
        [CultureCode("pa")]
        Punjabi = 43,
        [CultureCode("kn")]
        Kannada = 44,
        [CultureCode("ml")]
        Malayalam = 45,
        [CultureCode("si")]
        Sinhala = 46,
        [CultureCode("ne")]
        Nepali = 47,
        [CultureCode("my")]
        Burmese = 48,
        [CultureCode("km")]
        Khmer = 49,
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