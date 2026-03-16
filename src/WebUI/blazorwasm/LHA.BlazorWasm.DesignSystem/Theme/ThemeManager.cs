using System.Reflection;
using LHA.BlazorWasm.DesignSystem.Tokens;

namespace LHA.BlazorWasm.DesignSystem.Theme;

/// <summary>
/// Manages theme definitions and generates CSS custom properties for runtime theming.
/// </summary>
public sealed class ThemeManager
{
    private static readonly LightTheme LightThemeInstance = new();
    private static readonly DarkTheme DarkThemeInstance = new();

    public IThemeDefinition Light => LightThemeInstance;
    public IThemeDefinition Dark => DarkThemeInstance;

    /// <summary>
    /// Generates CSS custom property declarations for a given theme.
    /// These are injected as :root or [data-theme] CSS variables.
    /// </summary>
    public static string GenerateThemeCssVariables(IThemeDefinition theme)
    {
        return $"""
        /* Surface */
        --ds-color-bg: {theme.Background};
        --ds-color-bg-subtle: {theme.BackgroundSubtle};
        --ds-color-surface: {theme.Surface};
        --ds-color-surface-raised: {theme.SurfaceRaised};
        --ds-color-surface-overlay: {theme.SurfaceOverlay};

        /* Border */
        --ds-color-border: {theme.Border};
        --ds-color-border-subtle: {theme.BorderSubtle};
        --ds-color-border-strong: {theme.BorderStrong};

        /* Text */
        --ds-color-text: {theme.TextPrimary};
        --ds-color-text-secondary: {theme.TextSecondary};
        --ds-color-text-tertiary: {theme.TextTertiary};
        --ds-color-text-disabled: {theme.TextDisabled};
        --ds-color-text-on-primary: {theme.TextOnPrimary};
        --ds-color-text-on-danger: {theme.TextOnDanger};

        /* Primary */
        --ds-color-primary: {theme.PrimaryDefault};
        --ds-color-primary-hover: {theme.PrimaryHover};
        --ds-color-primary-active: {theme.PrimaryActive};
        --ds-color-primary-soft: {theme.PrimarySoft};
        --ds-color-primary-soft-hover: {theme.PrimarySoftHover};

        /* Semantic */
        --ds-color-success: {theme.Success};
        --ds-color-success-soft: {theme.SuccessSoft};
        --ds-color-warning: {theme.Warning};
        --ds-color-warning-soft: {theme.WarningSoft};
        --ds-color-danger: {theme.Danger};
        --ds-color-danger-soft: {theme.DangerSoft};
        --ds-color-info: {theme.Info};
        --ds-color-info-soft: {theme.InfoSoft};

        /* Sidebar */
        --ds-sidebar-bg: {theme.SidebarBg};
        --ds-sidebar-border: {theme.SidebarBorder};
        --ds-sidebar-text: {theme.SidebarText};
        --ds-sidebar-text-muted: {theme.SidebarTextMuted};
        --ds-sidebar-text-active: {theme.SidebarTextActive};
        --ds-sidebar-hover: {theme.SidebarHover};
        --ds-sidebar-active-indicator: {theme.SidebarActiveIndicator};

        /* Shadows */
        --ds-shadow-sm: {theme.ShadowSm};
        --ds-shadow-md: {theme.ShadowMd};
        --ds-shadow-lg: {theme.ShadowLg};

        /* Input */
        --ds-input-bg: {theme.InputBg};
        --ds-input-border: {theme.InputBorder};
        --ds-input-border-focus: {theme.InputBorderFocus};
        --ds-input-placeholder: {theme.InputPlaceholder};

        /* Misc */
        --ds-focus-ring: {theme.FocusRing};
        --ds-divider: {theme.Divider};
        --ds-skeleton-base: {theme.SkeletonBase};
        --ds-skeleton-shimmer: {theme.SkeletonShimmer};
        --ds-scrollbar-track: {theme.ScrollbarTrack};
        --ds-scrollbar-thumb: {theme.ScrollbarThumb};
        """;
    }

    /// <summary>
    /// Generates the invariant (non-theme-dependent) CSS custom properties
    /// for spacing, typography, radius, and animation tokens.
    /// </summary>
    public static string GenerateInvariantCssVariables()
    {
        return $"""
        /* ── Typography ── */
        --ds-font-sans: {TypographyTokens.FontSans};
        --ds-font-mono: {TypographyTokens.FontMono};
        --ds-text-xxs: {TypographyTokens.TextXxs};
        --ds-text-xs: {TypographyTokens.TextXs};
        --ds-text-sm: {TypographyTokens.TextSm};
        --ds-text-base: {TypographyTokens.TextBase};
        --ds-text-md: {TypographyTokens.TextMd};
        --ds-text-lg: {TypographyTokens.TextLg};
        --ds-text-xl: {TypographyTokens.TextXl};
        --ds-text-2xl: {TypographyTokens.Text2xl};
        --ds-text-3xl: {TypographyTokens.Text3xl};
        --ds-text-4xl: {TypographyTokens.Text4xl};
        --ds-text-5xl: {TypographyTokens.Text5xl};
        --ds-weight-normal: {TypographyTokens.WeightNormal};
        --ds-weight-medium: {TypographyTokens.WeightMedium};
        --ds-weight-semibold: {TypographyTokens.WeightSemibold};
        --ds-weight-bold: {TypographyTokens.WeightBold};
        --ds-weight-extrabold: {TypographyTokens.WeightExtrabold};
        --ds-leading-tight: {TypographyTokens.LeadingTight};
        --ds-leading-snug: {TypographyTokens.LeadingSnug};
        --ds-leading-normal: {TypographyTokens.LeadingNormal};
        --ds-leading-relaxed: {TypographyTokens.LeadingRelaxed};

        /* ── Spacing ── */
        --ds-space-xxs: {SpacingTokens.Xxs};
        --ds-space-xs: {SpacingTokens.Xs};
        --ds-space-sm: {SpacingTokens.Sm};
        --ds-space-md: {SpacingTokens.Md};
        --ds-space-base: {SpacingTokens.Base};
        --ds-space-lg: {SpacingTokens.Lg};
        --ds-space-xl: {SpacingTokens.Xl};
        --ds-space-2xl: {SpacingTokens.Xxl};
        --ds-space-3xl: {SpacingTokens.Xxxl};
        --ds-space-4xl: {SpacingTokens.Xxxxl};
        --ds-space-section: {SpacingTokens.Section};
        --ds-space-page: {SpacingTokens.Page};

        /* ── Layout ── */
        --ds-sidebar-width: {SpacingTokens.SidebarWidth};
        --ds-sidebar-mini-width: {SpacingTokens.SidebarMiniWidth};
        --ds-topbar-height: {SpacingTokens.TopBarHeight};
        --ds-page-max-width: {SpacingTokens.PageMaxWidth};
        --ds-content-px: {SpacingTokens.ContentPaddingX};
        --ds-content-py: {SpacingTokens.ContentPaddingY};
        --ds-card-padding: {SpacingTokens.CardPadding};

        /* ── Radius ── */
        --ds-radius-xs: {RadiusTokens.Xs};
        --ds-radius-sm: {RadiusTokens.Sm};
        --ds-radius-md: {RadiusTokens.Md};
        --ds-radius-base: {RadiusTokens.Base};
        --ds-radius-lg: {RadiusTokens.Lg};
        --ds-radius-xl: {RadiusTokens.Xl};
        --ds-radius-2xl: {RadiusTokens.Xxl};
        --ds-radius-3xl: {RadiusTokens.Xxxl};
        --ds-radius-full: {RadiusTokens.Full};

        /* ── Motion ── */
        --ds-duration-instant: {AnimationTokens.DurationInstant};
        --ds-duration-fast: {AnimationTokens.DurationFast};
        --ds-duration-normal: {AnimationTokens.DurationNormal};
        --ds-duration-moderate: {AnimationTokens.DurationModerate};
        --ds-duration-slow: {AnimationTokens.DurationSlow};
        --ds-duration-slower: {AnimationTokens.DurationSlower};
        --ds-ease-default: {AnimationTokens.EaseDefault};
        --ds-ease-in: {AnimationTokens.EaseIn};
        --ds-ease-out: {AnimationTokens.EaseOut};
        --ds-ease-in-out: {AnimationTokens.EaseInOut};
        --ds-ease-spring: {AnimationTokens.EaseSpring};
        --ds-transition-colors: {AnimationTokens.TransitionColors};
        --ds-transition-all: {AnimationTokens.TransitionAll};
        """;
    }
}
