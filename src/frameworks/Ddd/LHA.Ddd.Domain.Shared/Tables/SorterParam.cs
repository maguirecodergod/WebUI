namespace LHA.Ddd.Domain;

/// <summary>
/// Sorting parameters.
/// </summary>
public class SorterParam
{
    /// <summary>Name of the property to sort by.</summary>
    public string KeyName { get; set; } = string.Empty;
    
    /// <summary>True for ascending, false for descending.</summary>
    public bool IsASC { get; set; } = true;
}
