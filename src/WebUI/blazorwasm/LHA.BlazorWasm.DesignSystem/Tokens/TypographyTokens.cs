namespace LHA.BlazorWasm.DesignSystem.Tokens;

/// <summary>
/// Typography tokens defining the type scale, weights, and line heights.
/// Uses Inter/Outfit font stack for premium SaaS aesthetics.
/// </summary>
public static class TypographyTokens
{
    // ── Font Families ─────────────────────────────────────────────────
    public const string FontSans = "'Inter', 'Outfit', -apple-system, BlinkMacSystemFont, 'Segoe UI', system-ui, sans-serif";
    public const string FontMono = "'JetBrains Mono', 'Fira Code', 'SF Mono', 'Cascadia Code', monospace";

    // ── Font Sizes (modular scale 1.2) ────────────────────────────────
    public const string TextXxs = "0.625rem";    // 10px
    public const string TextXs = "0.75rem";      // 12px
    public const string TextSm = "0.8125rem";    // 13px
    public const string TextBase = "0.875rem";    // 14px  — body default
    public const string TextMd = "0.9375rem";     // 15px
    public const string TextLg = "1rem";          // 16px
    public const string TextXl = "1.125rem";      // 18px
    public const string Text2xl = "1.25rem";      // 20px
    public const string Text3xl = "1.5rem";       // 24px
    public const string Text4xl = "1.875rem";     // 30px
    public const string Text5xl = "2.25rem";      // 36px

    // ── Font Weights ──────────────────────────────────────────────────
    public const string WeightNormal = "400";
    public const string WeightMedium = "500";
    public const string WeightSemibold = "600";
    public const string WeightBold = "700";
    public const string WeightExtrabold = "800";

    // ── Line Heights ──────────────────────────────────────────────────
    public const string LeadingNone = "1";
    public const string LeadingTight = "1.25";
    public const string LeadingSnug = "1.375";
    public const string LeadingNormal = "1.5";
    public const string LeadingRelaxed = "1.625";
    public const string LeadingLoose = "2";

    // ── Letter Spacing ────────────────────────────────────────────────
    public const string TrackingTighter = "-0.05em";
    public const string TrackingTight = "-0.025em";
    public const string TrackingNormal = "0em";
    public const string TrackingWide = "0.025em";
    public const string TrackingWider = "0.05em";
    public const string TrackingWidest = "0.1em";
}
