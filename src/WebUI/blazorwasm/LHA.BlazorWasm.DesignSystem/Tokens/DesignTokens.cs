namespace LHA.BlazorWasm.DesignSystem.Tokens;

/// <summary>
/// Unified accessor providing static references to all design token categories.
/// Usage: DesignTokens.Colors.Primary500, DesignTokens.Spacing.Md, etc.
/// </summary>
public static class DesignTokens
{
    /// <summary>Color palette tokens.</summary>
    public static class Colors
    {
        // Primary
        public const string Primary50 = ColorTokens.Primary50;
        public const string Primary500 = ColorTokens.Primary500;
        public const string Primary600 = ColorTokens.Primary600;
        public const string Primary700 = ColorTokens.Primary700;

        // Accent
        public const string Accent500 = ColorTokens.Accent500;
        public const string Accent600 = ColorTokens.Accent600;

        // Neutral
        public const string White = ColorTokens.Neutral0;
        public const string Neutral50 = ColorTokens.Neutral50;
        public const string Neutral100 = ColorTokens.Neutral100;
        public const string Neutral200 = ColorTokens.Neutral200;
        public const string Neutral500 = ColorTokens.Neutral500;
        public const string Neutral700 = ColorTokens.Neutral700;
        public const string Neutral800 = ColorTokens.Neutral800;
        public const string Neutral900 = ColorTokens.Neutral900;
        public const string Neutral950 = ColorTokens.Neutral950;

        // Semantic
        public const string Success = ColorTokens.Success500;
        public const string Warning = ColorTokens.Warning500;
        public const string Danger = ColorTokens.Danger500;
        public const string Info = ColorTokens.Info500;
    }

    /// <summary>Spacing scale tokens.</summary>
    public static class Spacing
    {
        public const string Xs = SpacingTokens.Xs;
        public const string Sm = SpacingTokens.Sm;
        public const string Md = SpacingTokens.Md;
        public const string Base = SpacingTokens.Base;
        public const string Lg = SpacingTokens.Lg;
        public const string Xl = SpacingTokens.Xl;
        public const string Xxl = SpacingTokens.Xxl;
    }

    /// <summary>Typography tokens.</summary>
    public static class Typography
    {
        public const string FontSans = TypographyTokens.FontSans;
        public const string FontMono = TypographyTokens.FontMono;
        public const string TextSm = TypographyTokens.TextSm;
        public const string TextBase = TypographyTokens.TextBase;
        public const string TextLg = TypographyTokens.TextLg;
        public const string TextXl = TypographyTokens.TextXl;
    }

    /// <summary>Border radius tokens.</summary>
    public static class Radius
    {
        public const string Sm = RadiusTokens.Sm;
        public const string Md = RadiusTokens.Md;
        public const string Base = RadiusTokens.Base;
        public const string Lg = RadiusTokens.Lg;
        public const string Xl = RadiusTokens.Xl;
        public const string Full = RadiusTokens.Full;
    }

    /// <summary>Shadow elevation tokens.</summary>
    public static class Shadows
    {
        public const string Sm = ShadowTokens.Sm;
        public const string Md = ShadowTokens.Md;
        public const string Lg = ShadowTokens.Lg;
        public const string Xl = ShadowTokens.Xl;
    }

    /// <summary>Animation/motion tokens.</summary>
    public static class Motion
    {
        public const string Fast = AnimationTokens.DurationFast;
        public const string Normal = AnimationTokens.DurationNormal;
        public const string Slow = AnimationTokens.DurationSlow;
        public const string EaseDefault = AnimationTokens.EaseDefault;
    }
}
