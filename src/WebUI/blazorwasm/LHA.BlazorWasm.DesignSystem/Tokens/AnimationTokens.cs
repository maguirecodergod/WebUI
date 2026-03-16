namespace LHA.BlazorWasm.DesignSystem.Tokens;

/// <summary>
/// Animation tokens for consistent motion design.
/// Following material design motion principles.
/// </summary>
public static class AnimationTokens
{
    // ── Durations ─────────────────────────────────────────────────────
    public const string DurationInstant = "50ms";
    public const string DurationFast = "100ms";
    public const string DurationNormal = "150ms";
    public const string DurationModerate = "200ms";
    public const string DurationSlow = "300ms";
    public const string DurationSlower = "400ms";
    public const string DurationSlowest = "500ms";

    // ── Easing Curves ─────────────────────────────────────────────────
    public const string EaseDefault = "cubic-bezier(0.4, 0, 0.2, 1)";
    public const string EaseIn = "cubic-bezier(0.4, 0, 1, 1)";
    public const string EaseOut = "cubic-bezier(0, 0, 0.2, 1)";
    public const string EaseInOut = "cubic-bezier(0.4, 0, 0.2, 1)";
    public const string EaseSpring = "cubic-bezier(0.34, 1.56, 0.64, 1)";
    public const string EaseBounce = "cubic-bezier(0.68, -0.55, 0.265, 1.55)";

    // ── Composite Transitions ─────────────────────────────────────────
    public const string TransitionColors = "color 150ms ease, background-color 150ms ease, border-color 150ms ease";
    public const string TransitionOpacity = "opacity 150ms ease";
    public const string TransitionTransform = "transform 200ms cubic-bezier(0.4, 0, 0.2, 1)";
    public const string TransitionAll = "all 150ms cubic-bezier(0.4, 0, 0.2, 1)";
    public const string TransitionShadow = "box-shadow 200ms ease";
}
