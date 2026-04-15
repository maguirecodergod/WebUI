using LHA.BlazorWasm.Components;
using LHA.BlazorWasm.Components.Pickers.Core;
using LHA.BlazorWasm.Components.Select;
using LHA.BlazorWasm.Components.Table;
using LHA.BlazorWasm.Shared;
using LHA.Shared.Contracts.AuditLog;
using Microsoft.AspNetCore.Components;

namespace LHA.BlazorWasm.Modules.Host.AuditLogs.Pages
{
    public partial class HostAuditLogList : LhaComponentBase
    {
        [Inject] private IAuditLogAppService AuditLogService { get; set; } = default!;
        private List<AuditLogDto> _logs = new();
        private long _totalCount;
        private DateRange<DateTimeOffset?> _executionTimeRange = new();

        private List<SelectOption<string>> _httpMethodOptions = new()
    {
        new () { Value = "GET", Label = "GET" },
        new () { Value = "POST", Label = "POST" },
        new () { Value = "PUT", Label = "PUT" },
        new () { Value = "DELETE", Label = "DELETE" }
    };

        private GetAuditLogsInput _input = new()
        {
            PageSize = 10,
            SorterKey = nameof(AuditLogDto.ExecutionTime),
            SorterIsAsc = false
        };

        protected override async Task OnInitializedAsync()
        {
            await LoadLogsAsync();
        }

        private async Task LoadLogsAsync()
        {
            try
            {
                _input.StartTime = _executionTimeRange.Start?.ToUniversalTime();
                _input.EndTime = _executionTimeRange.End?.ToUniversalTime();

                var result = await AuditLogService.GetListAsync(_input);
                _logs = result.Items.ToList();
                _totalCount = result.TotalCount;
            }
            catch (Exception ex)
            {
                ToastNotification.Error(L("AuditLog.LoadError") + ": " + ex.Message);
            }
        }

        private async Task<DataTableResponse<AuditLogDto>> GetDataTableResponseAsync(DataTableRequest request)
        {
            _input.PageNumber = request.PageNumber;
            _input.PageSize = request.PageSize;

            // Map global search term
            _input.SearchQuery = string.IsNullOrEmpty(request.SearchTerm) ? null : request.SearchTerm;

            // Note: Sidebar filters are already bound to _input via @bind-Value in FilterTemplates.
            await LoadLogsAsync();
            return new DataTableResponse<AuditLogDto> { Items = _logs, TotalCount = (int)_totalCount };
        }

        private async Task RefreshAsync() => await LoadLogsAsync();

        // Internal Enums for StatusBadge mapping
        public enum AuditMethod { GET, POST, PUT, DELETE, Other }
        public enum AuditStatus { Success, Warning, Error, Processing }

        private AuditMethod GetMethodEnum(string? method) => method?.ToUpper() switch
        {
            "GET" => AuditMethod.GET,
            "POST" => AuditMethod.POST,
            "PUT" => AuditMethod.PUT,
            "DELETE" => AuditMethod.DELETE,
            _ => AuditMethod.Other
        };

        private CHttpStatusCodeType GetStatusEnum(int? code)
        {
            if (code is null) return CHttpStatusCodeType.None;

            if (Enum.IsDefined(typeof(CHttpStatusCodeType), code))
                return (CHttpStatusCodeType)code;

            return CHttpStatusCodeType.None;
        }

        private void ViewDetails(AuditLogDto log)
        {

        }

        private async Task ShowDeleteDialog()
        {

        }
    }
}