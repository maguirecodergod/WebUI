namespace LHA.BlazorWasm.Components.Toast;

/// <summary>
/// Root-level Application DOM host anchoring absolute notifications.
/// Re-renders iteratively via strict implicit service Event bindings.
/// </summary>
public partial class ToastContainer : LhaComponentBase, IDisposable
{
    protected override void OnInitialized()
    {
        ToastNotification.State.OnChange += StateHasChanged;
    }

    public void Dispose()
    {
        ToastNotification.State.OnChange -= StateHasChanged;
    }
}
