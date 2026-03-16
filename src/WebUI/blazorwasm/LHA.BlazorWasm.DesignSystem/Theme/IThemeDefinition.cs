namespace LHA.BlazorWasm.DesignSystem.Theme;

/// <summary>
/// Defines a complete theme with all semantic color mappings.
/// </summary>
public interface IThemeDefinition
{
    string Name { get; }

    // ── Surface Colors ────────────────────────────────────────────────
    string Background { get; }
    string BackgroundSubtle { get; }
    string Surface { get; }
    string SurfaceRaised { get; }
    string SurfaceOverlay { get; }

    // ── Border Colors ─────────────────────────────────────────────────
    string Border { get; }
    string BorderSubtle { get; }
    string BorderStrong { get; }

    // ── Text Colors ───────────────────────────────────────────────────
    string TextPrimary { get; }
    string TextSecondary { get; }
    string TextTertiary { get; }
    string TextDisabled { get; }
    string TextOnPrimary { get; }
    string TextOnDanger { get; }

    // ── Interactive ───────────────────────────────────────────────────
    string PrimaryDefault { get; }
    string PrimaryHover { get; }
    string PrimaryActive { get; }
    string PrimarySoft { get; }
    string PrimarySoftHover { get; }

    // ── Semantic ──────────────────────────────────────────────────────
    string Success { get; }
    string SuccessSoft { get; }
    string Warning { get; }
    string WarningSoft { get; }
    string Danger { get; }
    string DangerSoft { get; }
    string Info { get; }
    string InfoSoft { get; }

    // ── Sidebar ───────────────────────────────────────────────────────
    string SidebarBg { get; }
    string SidebarBorder { get; }
    string SidebarText { get; }
    string SidebarTextMuted { get; }
    string SidebarTextActive { get; }
    string SidebarHover { get; }
    string SidebarActiveIndicator { get; }

    // ── Shadows ───────────────────────────────────────────────────────
    string ShadowSm { get; }
    string ShadowMd { get; }
    string ShadowLg { get; }

    // ── Input ─────────────────────────────────────────────────────────
    string InputBg { get; }
    string InputBorder { get; }
    string InputBorderFocus { get; }
    string InputPlaceholder { get; }

    // ── Misc ──────────────────────────────────────────────────────────
    string FocusRing { get; }
    string Divider { get; }
    string SkeletonBase { get; }
    string SkeletonShimmer { get; }
    string ScrollbarTrack { get; }
    string ScrollbarThumb { get; }
}
