namespace LHA.MultiTenancy;

/// <summary>
/// Apply to entities or services that should bypass multi-tenancy filtering.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
public sealed class IgnoreMultiTenancyAttribute : Attribute;
