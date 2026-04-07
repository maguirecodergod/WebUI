namespace LHA.Shared.Domain
{
    /// <summary>
    /// Attribute used on enum members to associate a standard culture code (e.g. "en", "vi").
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CultureCodeAttribute : Attribute
    {
        public string Code { get; }

        public CultureCodeAttribute(string code)
        {
            Code = code;
        }
    }
}
