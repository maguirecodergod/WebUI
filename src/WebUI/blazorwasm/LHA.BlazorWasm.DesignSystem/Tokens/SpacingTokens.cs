namespace LHA.BlazorWasm.DesignSystem.Tokens;

/// <summary>
/// Spacing tokens based on a 4px grid system.
/// Ensures consistent spacing across all components.
/// </summary>
public static class SpacingTokens
{
    // ── Base unit ─────────────────────────────────────────────────────
    public const int BaseUnit = 4;

    // ── Named scale ───────────────────────────────────────────────────
    public const string None = "0";
    public const string Px = "1px";
    public const string Xxs = "2px";       // 0.5 × base
    public const string Xs = "4px";        // 1 × base
    public const string Sm = "8px";        // 2 × base
    public const string Md = "12px";       // 3 × base
    public const string Base = "16px";     // 4 × base
    public const string Lg = "20px";       // 5 × base
    public const string Xl = "24px";       // 6 × base
    public const string Xxl = "32px";      // 8 × base
    public const string Xxxl = "40px";     // 10 × base
    public const string Xxxxl = "48px";    // 12 × base
    public const string Section = "64px";  // 16 × base
    public const string Page = "80px";     // 20 × base

    // ── Layout-specific spacing ───────────────────────────────────────
    public const string SidebarWidth = "260px";
    public const string SidebarMiniWidth = "68px";
    public const string TopBarHeight = "56px";
    public const string PageMaxWidth = "1280px";
    public const string ContentPaddingX = "32px";
    public const string ContentPaddingY = "24px";
    public const string CardPadding = "24px";

    // ── Gap tokens ────────────────────────────────────────────────────
    public const string GapXs = "4px";
    public const string GapSm = "8px";
    public const string GapMd = "12px";
    public const string GapBase = "16px";
    public const string GapLg = "24px";
    public const string GapXl = "32px";
}
