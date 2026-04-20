using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Tabs;

/// <summary>
/// Represents the internal model for a registered tab item.
/// Populated by <see cref="TabItem"/> via the parent <see cref="Tabs"/> cascade.
/// </summary>
public sealed class TabDefinition
{
    // ── Identity ──────────────────────────────────────────────
    /// <summary>Unique identifier for this tab (auto-generated or overridden).</summary>
    public string Id { get; init; } = Guid.NewGuid().ToString("N");

    // ── Display ───────────────────────────────────────────────
    /// <summary>Text label shown in the tab header button.</summary>
    public string? Title { get; set; }

    /// <summary>Bootstrap-icon class (e.g. "bi bi-house") shown before the title.</summary>
    public string? Icon { get; set; }

    /// <summary>Optional badge text displayed after the tab label (e.g. count pill).</summary>
    public string? Badge { get; set; }

    // ── State ─────────────────────────────────────────────────
    /// <summary>When true the tab header button is grayed-out and non-interactive.</summary>
    public bool Disabled { get; set; }

    /// <summary>When true a close (×) icon is rendered inside the tab header.</summary>
    public bool Closable { get; set; }

    // ── Rendering ─────────────────────────────────────────────
    /// <summary>
    /// The tab body fragment provided via the &lt;Content&gt; child slot.
    /// </summary>
    public RenderFragment? Content { get; set; }

    /// <summary>
    /// When true, the content panel has already been rendered at least once.
    /// Used to implement "preserve state" — the panel is hidden via CSS rather than removed from the DOM.
    /// </summary>
    public bool HasBeenRendered { get; internal set; }

    // ── Styling ───────────────────────────────────────────────
    /// <summary>Additional CSS classes forwarded to the tab header button.</summary>
    public string? HeaderClass { get; set; }

    /// <summary>Additional CSS classes forwarded to the content panel div.</summary>
    public string? ContentClass { get; set; }

    // ── Order ─────────────────────────────────────────────────
    /// <summary>Insertion order assigned by the parent container — used to keep tabs sorted.</summary>
    internal int Order { get; set; }
}
