using LHA.BlazorWasm.Components.Sidebar.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LHA.BlazorWasm.Components.Sidebar;

/// <summary>
/// Recursive sub-component that renders a single sidebar navigation item
/// and its nested children. Used internally by <see cref="Sidebar"/>.
/// </summary>
public partial class SidebarItem : LhaComponentBase
{
    #region ── Parameters ──

    /// <summary>
    /// The data model for this sidebar item node.
    /// </summary>
    [Parameter, EditorRequired]
    public SidebarItemModel Item { get; set; } = default!;

    /// <summary>
    /// The current nesting depth (0 = root level). Used for indentation.
    /// </summary>
    [Parameter]
    public int Depth { get; set; }

    /// <summary>
    /// The current visual state of the parent sidebar.
    /// Controls whether labels are shown (Mini mode hides them).
    /// </summary>
    [Parameter]
    public CSidebarState CurrentState { get; set; }

    /// <summary>
    /// The set of currently expanded item IDs, managed by the parent <see cref="Sidebar"/>.
    /// </summary>
    [Parameter]
    public HashSet<string> ExpandedIds { get; set; } = new();

    /// <summary>
    /// The ID of the currently active (route-matched) item.
    /// </summary>
    [Parameter]
    public string? ActiveItemId { get; set; }

    /// <summary>
    /// Callback to toggle the expand/collapse state of a parent node.
    /// </summary>
    [Parameter]
    public EventCallback<string> OnToggleExpand { get; set; }

    /// <summary>
    /// Callback when a leaf item is clicked for navigation.
    /// </summary>
    [Parameter]
    public EventCallback<SidebarItemModel> OnItemClicked { get; set; }

    #endregion

    #region ── Computed Properties ──

    /// <summary>Whether this item's children container is expanded.</summary>
    private bool IsExpanded => ExpandedIds.Contains(Item.Id);

    /// <summary>Whether this item is the active route.</summary>
    private bool IsActive => ActiveItemId == Item.Id;

    private string DepthClass => $"lha-sidebar-item--depth-{Math.Min(Depth, 5)}";

    private string ActiveClass => IsActive ? "lha-sidebar-item--active" : "";

    private string DisabledClass => Item.IsDisabled ? "lha-sidebar-item--disabled" : "";

    #endregion

    #region ── Event Handlers ──

    private async Task HandleClick()
    {
        if (Item.IsDisabled) return;

        if (Item.HasChildren)
        {
            // Toggle expand/collapse
            await OnToggleExpand.InvokeAsync(Item.Id);
        }

        if (!string.IsNullOrEmpty(Item.Href))
        {
            // Navigate
            if (!string.IsNullOrEmpty(Item.Target))
            {
                // External link: open in new tab etc.
                await JS.InvokeVoidAsync("open", Item.Href, Item.Target);
            }
            else
            {
                Navigation.NavigateTo(Item.Href);
            }

            await OnItemClicked.InvokeAsync(Item);
        }
        else if (!Item.HasChildren)
        {
            // Leaf item without Href (action-only item)
            await OnItemClicked.InvokeAsync(Item);
        }
    }

    /// <summary>
    /// Returns the tooltip text. In Mini mode, always show the translated title.
    /// In Expanded mode, only show tooltip if it differs from the visible label.
    /// </summary>
    private string GetTooltip()
    {
        return CurrentState == CSidebarState.Mini
            ? Localizer.L(Item.TitleKey)
            : "";
    }

    #endregion
}
