using LHA.BlazorWasm.DesignSystem.Tokens;

namespace LHA.BlazorWasm.DesignSystem.Theme;

/// <summary>
/// Dark theme — immersive, sleek, reduced eye strain.
/// Inspired by Linear, GitHub Dark, and Vercel dark mode.
/// </summary>
public sealed class DarkTheme : IThemeDefinition
{
    public string Name => "dark";

    // ── Surface Colors ────────────────────────────────────────────────
    public string Background => ColorTokens.Neutral950;
    public string BackgroundSubtle => ColorTokens.Neutral900;
    public string Surface => ColorTokens.Neutral900;
    public string SurfaceRaised => ColorTokens.Neutral850;
    public string SurfaceOverlay => "rgba(10, 15, 30, 0.95)";

    // ── Border Colors ─────────────────────────────────────────────────
    public string Border => "hsla(210, 14%, 22%, 0.8)";
    public string BorderSubtle => "hsla(210, 14%, 18%, 0.6)";
    public string BorderStrong => ColorTokens.Neutral600;

    // ── Text Colors ───────────────────────────────────────────────────
    public string TextPrimary => "hsl(210, 20%, 93%)";
    public string TextSecondary => ColorTokens.Neutral400;
    public string TextTertiary => ColorTokens.Neutral500;
    public string TextDisabled => ColorTokens.Neutral600;
    public string TextOnPrimary => ColorTokens.Neutral0;
    public string TextOnDanger => ColorTokens.Neutral0;

    // ── Interactive ───────────────────────────────────────────────────
    public string PrimaryDefault => ColorTokens.Primary400;
    public string PrimaryHover => ColorTokens.Primary300;
    public string PrimaryActive => ColorTokens.Primary500;
    public string PrimarySoft => "hsla(225, 85%, 55%, 0.12)";
    public string PrimarySoftHover => "hsla(225, 85%, 55%, 0.2)";

    // ── Semantic ──────────────────────────────────────────────────────
    public string Success => ColorTokens.Success500;
    public string SuccessSoft => "hsla(150, 60%, 42%, 0.12)";
    public string Warning => ColorTokens.Warning500;
    public string WarningSoft => "hsla(40, 85%, 50%, 0.12)";
    public string Danger => ColorTokens.Danger500;
    public string DangerSoft => "hsla(0, 72%, 51%, 0.12)";
    public string Info => ColorTokens.Info500;
    public string InfoSoft => "hsla(195, 70%, 46%, 0.12)";

    // ── Sidebar ───────────────────────────────────────────────────────
    public string SidebarBg => ColorTokens.Neutral950;
    public string SidebarBorder => "hsla(210, 14%, 22%, 0.6)";
    public string SidebarText => ColorTokens.Neutral300;
    public string SidebarTextMuted => ColorTokens.Neutral500;
    public string SidebarTextActive => ColorTokens.Primary400;
    public string SidebarHover => "hsla(210, 14%, 22%, 0.5)";
    public string SidebarActiveIndicator => ColorTokens.Primary400;

    // ── Shadows ───────────────────────────────────────────────────────
    public string ShadowSm => ShadowTokens.DarkSm;
    public string ShadowMd => ShadowTokens.DarkMd;
    public string ShadowLg => ShadowTokens.DarkLg;

    // ── Input ─────────────────────────────────────────────────────────
    public string InputBg => ColorTokens.Neutral900;
    public string InputBorder => "hsla(210, 14%, 22%, 0.8)";
    public string InputBorderFocus => ColorTokens.Primary400;
    public string InputPlaceholder => ColorTokens.Neutral500;

    // ── Misc ──────────────────────────────────────────────────────────
    public string FocusRing => "0 0 0 2px " + ColorTokens.Neutral950 + ", 0 0 0 4px " + ColorTokens.Primary400;
    public string Divider => "hsla(210, 14%, 20%, 0.5)";
    public string SkeletonBase => ColorTokens.Neutral850;
    public string SkeletonShimmer => ColorTokens.Neutral800;
    public string ScrollbarTrack => "transparent";
    public string ScrollbarThumb => ColorTokens.Neutral700;
}
