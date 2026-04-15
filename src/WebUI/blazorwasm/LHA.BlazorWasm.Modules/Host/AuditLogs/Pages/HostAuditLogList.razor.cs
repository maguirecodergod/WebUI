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

        private List<SelectOption<string>> _httpMethodOptions => Enum.GetValues<CHttpMethodType>()
            .Select(x => new SelectOption<string>
            {
                Value = x.ToString(),
                Label = char.ToUpper(x.ToString().ToLower()[0]) + x.ToString().ToLower().Substring(1)
            })
            .ToList();

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

                var result = await AuditLogService.GetHostListAsync(_input);
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

            // Map sort state from DataTable column headers
            if (request.Sorts is { Count: > 0 })
            {
                var sort = request.Sorts[0]; // Primary sort
                _input.SorterKey = sort.Field;
                _input.SorterIsAsc = sort.Direction == SortDirection.Ascending;
            }
            else
            {
                // Reset to default sort when user clears sorting
                _input.SorterKey = nameof(AuditLogDto.ExecutionTime);
                _input.SorterIsAsc = false;
            }

            // Note: Sidebar filters are already bound to _input via @bind-Value in FilterTemplates.
            await LoadLogsAsync();
            return new DataTableResponse<AuditLogDto> { Items = _logs, TotalCount = (int)_totalCount };
        }

        private CHttpMethodType GetMethodEnum(string? method) => method?.ToUpper() switch
        {
            "GET" => CHttpMethodType.GET,
            "POST" => CHttpMethodType.POST,
            "PUT" => CHttpMethodType.PUT,
            "DELETE" => CHttpMethodType.DELETE,
            "PATCH" => CHttpMethodType.PATCH,
            "HEAD" => CHttpMethodType.HEAD,
            "OPTIONS" => CHttpMethodType.OPTIONS,
            "TRACE" => CHttpMethodType.TRACE,
            "CONNECT" => CHttpMethodType.CONNECT,
            _ => CHttpMethodType.Other
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