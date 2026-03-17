using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Topbar;

public partial class TopbarItem : LhaComponentBase
{
    [Parameter] public TopbarItemModel Item { get; set; } = new();
    [Parameter] public string? Class { get; set; }
    [Parameter] public bool ShowLabel { get; set; } = true;
    [Parameter] public EventCallback<TopbarItemModel> OnClick { get; set; }

    protected async Task OnClickInternal()
    {
        if (Item.OnClick != null)
        {
            Item.OnClick();
        }

        if (OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync(Item);
        }

        if (Item.ActionType == TopbarActionType.Link && !string.IsNullOrEmpty(Item.Href))
        {
            Navigation.NavigateTo(Item.Href);
        }
    }
}
