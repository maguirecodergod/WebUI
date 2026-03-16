namespace LHA.BlazorWasm.DesignSystem.Tokens;

/// <summary>
/// Shadow tokens defining elevation levels.
/// Uses layered shadows for realistic depth perception.
/// </summary>
public static class ShadowTokens
{
    public const string None = "none";

    public const string Xs = "0 1px 2px 0 rgba(0, 0, 0, 0.05)";

    public const string Sm =
        "0 1px 3px 0 rgba(0, 0, 0, 0.1), " +
        "0 1px 2px -1px rgba(0, 0, 0, 0.1)";

    public const string Md =
        "0 4px 6px -1px rgba(0, 0, 0, 0.1), " +
        "0 2px 4px -2px rgba(0, 0, 0, 0.1)";

    public const string Lg =
        "0 10px 15px -3px rgba(0, 0, 0, 0.1), " +
        "0 4px 6px -4px rgba(0, 0, 0, 0.1)";

    public const string Xl =
        "0 20px 25px -5px rgba(0, 0, 0, 0.1), " +
        "0 8px 10px -6px rgba(0, 0, 0, 0.1)";

    public const string Xxl = "0 25px 50px -12px rgba(0, 0, 0, 0.25)";

    // ── Ring shadow (focus indicators) ────────────────────────────────
    public const string Ring = "0 0 0 2px var(--ds-color-primary-500)";
    public const string RingOffset = "0 0 0 2px var(--ds-color-bg), 0 0 0 4px var(--ds-color-primary-500)";

    // ── Dark mode shadows ─────────────────────────────────────────────
    public const string DarkSm =
        "0 1px 3px 0 rgba(0, 0, 0, 0.3), " +
        "0 1px 2px -1px rgba(0, 0, 0, 0.3)";

    public const string DarkMd =
        "0 4px 6px -1px rgba(0, 0, 0, 0.35), " +
        "0 2px 4px -2px rgba(0, 0, 0, 0.3)";

    public const string DarkLg =
        "0 10px 15px -3px rgba(0, 0, 0, 0.4), " +
        "0 4px 6px -4px rgba(0, 0, 0, 0.35)";
}
