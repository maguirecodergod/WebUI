using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Table;

/// <summary>
/// Metadata for a single DataTable column. Built by <c>DataTableColumn&lt;T&gt;</c>
/// or created programmatically.
/// </summary>
public sealed class ColumnDefinition<TItem>
{
    // ── Identity ─────────────────────────────────────────────
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8];
    public string Field { get; set; } = "";
    public string Title { get; set; } = "";

    // ── Value Access ─────────────────────────────────────────
    public Func<TItem, object?>? ValueSelector { get; set; }
    public string? Format { get; set; }

    // ── Sort / Filter ────────────────────────────────────────
    public bool Sortable { get; set; }
    public bool Filterable { get; set; }
    public FilterType FilterType { get; set; } = FilterType.Text;
    public IReadOnlyList<SelectOption>? FilterOptions { get; set; }

    // ── Visibility & Order ───────────────────────────────────
    public bool Visible { get; set; } = true;
    /// <summary>True when visibility was changed at runtime (column toggle / localStorage restore).
    /// Prevents OnParametersSet from overwriting the user's choice.</summary>
    public bool VisibilityOverridden { get; set; }
    public int Order { get; set; }

    // ── Sizing ───────────────────────────────────────────────
    public string? Width { get; set; }
    public string? MinWidth { get; set; }
    public string? MaxWidth { get; set; }

    // ── Position ─────────────────────────────────────────────
    public FixedPosition Fixed { get; set; } = FixedPosition.None;
    public ColumnAlignment Alignment { get; set; } = ColumnAlignment.Left;

    // ── Styling ──────────────────────────────────────────────
    public bool Highlight { get; set; }
    public Func<TItem, string?>? CellCssClass { get; set; }
    public Func<TItem, string?>? CellStyle { get; set; }

    // ── Templates ────────────────────────────────────────────
    public RenderFragment<TItem>? CellTemplate { get; set; }
    public RenderFragment? HeaderTemplate { get; set; }

    // ═══════════════════════════════════════════════════════════
    // HELPERS
    // ═══════════════════════════════════════════════════════════

    /// <summary>Get formatted display value for a cell.</summary>
    public string GetDisplayValue(TItem item)
    {
        var val = ValueSelector?.Invoke(item);
        if (val is null) return "";
        if (!string.IsNullOrEmpty(Format) && val is IFormattable f)
            return f.ToString(Format, null);
        return val.ToString() ?? "";
    }

    /// <summary>Compute CSS classes for a data cell.</summary>
    public string GetCellCss(TItem item)
    {
        var parts = new List<string>(3);
        var align = Alignment switch
        {
            ColumnAlignment.Center => "dt-text-center",
            ColumnAlignment.Right => "dt-text-right",
            _ => ""
        };
        if (align.Length > 0) parts.Add(align);
        if (Highlight) parts.Add("dt-cell-highlight");
        var fix = GetFixedCss();
        if (fix.Length > 0) parts.Add(fix);
        var custom = CellCssClass?.Invoke(item);
        if (!string.IsNullOrEmpty(custom)) parts.Add(custom);
        return string.Join(' ', parts);
    }

    /// <summary>Compute inline style for a col/th/td element.</summary>
    public string GetColumnStyle()
    {
        var parts = new List<string>(3);
        if (!string.IsNullOrEmpty(Width)) parts.Add($"width:{Width}");
        if (!string.IsNullOrEmpty(MinWidth)) parts.Add($"min-width:{MinWidth}");
        if (!string.IsNullOrEmpty(MaxWidth)) parts.Add($"max-width:{MaxWidth}");
        return string.Join(';', parts);
    }

    /// <summary>CSS class for sticky (fixed) columns.</summary>
    public string GetFixedCss() => Fixed switch
    {
        FixedPosition.Left => "dt-sticky-left",
        FixedPosition.Right => "dt-sticky-right",
        _ => ""
    };

    // ── Expression Utilities ─────────────────────────────────

    /// <summary>Extract property name from an expression tree.</summary>
    public static string ExtractMemberName(Expression<Func<TItem, object?>> expression)
    {
        Expression body = expression.Body;
        if (body is UnaryExpression { NodeType: ExpressionType.Convert } unary)
            body = unary.Operand;
        return body is MemberExpression member ? member.Member.Name : "Unknown";
    }

    /// <summary>Insert spaces before capitals: "FirstName" → "First Name".</summary>
    public static string Humanize(string name) =>
        Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
}
