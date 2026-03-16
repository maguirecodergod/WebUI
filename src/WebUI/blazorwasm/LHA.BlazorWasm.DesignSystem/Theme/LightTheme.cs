using LHA.BlazorWasm.DesignSystem.Tokens;

namespace LHA.BlazorWasm.DesignSystem.Theme;

/// <summary>
/// Light theme — clean, spacious, professional.
/// Inspired by Linear, Stripe, and Vercel design.
/// </summary>
public sealed class LightTheme : IThemeDefinition
{
    public string Name => "light";

    // ── Surface Colors ────────────────────────────────────────────────
    public string Background => ColorTokens.Neutral0;
    public string BackgroundSubtle => ColorTokens.Neutral50;
    public string Surface => ColorTokens.Neutral0;
    public string SurfaceRaised => ColorTokens.Neutral0;
    public string SurfaceOverlay => "rgba(255, 255, 255, 0.95)";

    // ── Border Colors ─────────────────────────────────────────────────
    public string Border => ColorTokens.Neutral200;
    public string BorderSubtle => ColorTokens.Neutral150;
    public string BorderStrong => ColorTokens.Neutral300;

    // ── Text Colors ───────────────────────────────────────────────────
    public string TextPrimary => ColorTokens.Neutral900;
    public string TextSecondary => ColorTokens.Neutral600;
    public string TextTertiary => ColorTokens.Neutral400;
    public string TextDisabled => ColorTokens.Neutral300;
    public string TextOnPrimary => ColorTokens.Neutral0;
    public string TextOnDanger => ColorTokens.Neutral0;

    // ── Interactive ───────────────────────────────────────────────────
    public string PrimaryDefault => ColorTokens.Primary500;
    public string PrimaryHover => ColorTokens.Primary600;
    public string PrimaryActive => ColorTokens.Primary700;
    public string PrimarySoft => ColorTokens.Primary50;
    public string PrimarySoftHover => ColorTokens.Primary100;

    // ── Semantic ──────────────────────────────────────────────────────
    public string Success => ColorTokens.Success500;
    public string SuccessSoft => ColorTokens.Success50;
    public string Warning => ColorTokens.Warning500;
    public string WarningSoft => ColorTokens.Warning50;
    public string Danger => ColorTokens.Danger500;
    public string DangerSoft => ColorTokens.Danger50;
    public string Info => ColorTokens.Info500;
    public string InfoSoft => ColorTokens.Info50;

    // ── Sidebar ───────────────────────────────────────────────────────
    public string SidebarBg => ColorTokens.Neutral50;
    public string SidebarBorder => ColorTokens.Neutral200;
    public string SidebarText => ColorTokens.Neutral700;
    public string SidebarTextMuted => ColorTokens.Neutral400;
    public string SidebarTextActive => ColorTokens.Primary600;
    public string SidebarHover => ColorTokens.Neutral100;
    public string SidebarActiveIndicator => ColorTokens.Primary500;

    // ── Shadows ───────────────────────────────────────────────────────
    public string ShadowSm => ShadowTokens.Sm;
    public string ShadowMd => ShadowTokens.Md;
    public string ShadowLg => ShadowTokens.Lg;

    // ── Input ─────────────────────────────────────────────────────────
    public string InputBg => ColorTokens.Neutral0;
    public string InputBorder => ColorTokens.Neutral200;
    public string InputBorderFocus => ColorTokens.Primary500;
    public string InputPlaceholder => ColorTokens.Neutral400;

    // ── Misc ──────────────────────────────────────────────────────────
    public string FocusRing => "0 0 0 2px " + ColorTokens.Neutral0 + ", 0 0 0 4px " + ColorTokens.Primary500;
    public string Divider => ColorTokens.Neutral150;
    public string SkeletonBase => ColorTokens.Neutral150;
    public string SkeletonShimmer => ColorTokens.Neutral100;
    public string ScrollbarTrack => "transparent";
    public string ScrollbarThumb => ColorTokens.Neutral300;
}
