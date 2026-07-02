using LHA.BlazorWasm.Services.Auth;
using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Components.Dialog;

public partial class SecurityRevocationDialog : LHAComponentBase, IDisposable
{
    [Inject] private ISecurityRevocationUiService RevocationUi { get; set; } = default!;

    protected override void OnInitialized()
    {
        RevocationUi.State.OnChange += StateHasChanged;
    }

    private Task HandleLogoutAsync() => RevocationUi.ConfirmLogoutAsync();

    public override void Dispose()
    {
        RevocationUi.State.OnChange -= StateHasChanged;
    }
}
