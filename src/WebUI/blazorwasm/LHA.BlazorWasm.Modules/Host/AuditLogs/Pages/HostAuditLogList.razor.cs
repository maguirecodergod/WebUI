using LHA.Auditing;
using LHA.BlazorWasm.Components;
using LHA.BlazorWasm.Components.Pickers.Core;
using LHA.BlazorWasm.Components.Table;
using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.BlazorWasm.Shared;
using LHA.Shared.Contracts.AuditLog;
using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Shared.Models;
using LHA.BlazorWasm.Shared.Helpers;
using LHA.BlazorWasm.Shared.Models.Select;
using LHA.Shared.Domain.AuditLogs;
using LHA.BlazorWasm.HttpApi.Client.Clients;

namespace LHA.BlazorWasm.Modules.Host.AuditLogs.Pages
{
    public partial class HostAuditLogList : LHAComponentBase
    {
        [Inject] private AuditLogApiClient AuditLogService { get; set; } = default!;
        private List<AuditLogDto> _logs = new();
        private long _totalCount;
        private DateRange<DateTimeOffset?> _executionTimeRange = new();
        private DataTable<AuditLogDto>? _dataTable;
        private List<AuditLogDto> _selectedLogs = new();
        private bool _isDeleteDialogVisible;
        private AuditLogDto? _itemToDelete;
        private bool _canReadHostLogs;
        private bool _canDelete; // We'll derive this from HostRead for now since no specific delete permission exists
        private bool _isDetailsDialogVisible;
        private AuditLogDto? _viewingLog;

        private CServiceType _selectedService = CServiceType.Account;

        private List<SelectOption<CServiceType>> _serviceOptions => Localizer.ToSelectOptions<CServiceType>();

        private List<SelectOption<string>> _httpMethodOptions => Enum.GetValues<CHttpMethodType>()
            .Select(x => new SelectOption<string>
            {
                Value = x.ToString(),
                Label = char.ToUpper(x.ToString().ToLower()[0]) + x.ToString().ToLower().Substring(1)
            })
            .ToList();

        private List<SelectOption<CRequestType?>> _requestTypeOptions => Enum.GetValues<CRequestType>()
            .Select(x => new SelectOption<CRequestType?>
            {
                Value = x,
                Label = L($"AuditLog.RequestTypeEnum.{x}")
            })
            .ToList();

        private AuditLogPagedRequest _input = new()
        {
            PageSize = 10,
            SorterKey = nameof(AuditLogDto.ExecutionTime),
            SorterIsAsc = false,
        };

        private AuditLogFilter _filter = new();

        protected override async Task OnInitializedAsync()
        {
            _canReadHostLogs = PermissionService.HasPermission(AuditLogPermissions.AuditLogs.Read);
            _canDelete = PermissionService.HasPermission(AuditLogPermissions.AuditLogs.Delete);
            await LoadLogsAsync();
        }

        private async Task LoadLogsAsync()
        {
            try
            {
                _input.Filter = _filter;
                _input.Filter.StartTime = _executionTimeRange.Start?.ToUniversalTime();
                _input.Filter.EndTime = _executionTimeRange.End?.ToUniversalTime();
                _input.DisableTenantFilter = _canReadHostLogs;

                var result = await AuditLogService.GetListAsync(_input, _selectedService);

                _logs = result.Items.ToList();
                _totalCount = result.TotalCount;
                StateHasChanged();
            }
            catch (ApiException)
            {
                throw;
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

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                _input.SearchQuery = request.SearchTerm;
            }

            if (request.Sorts is { Count: > 0 })
            {
                var sort = request.Sorts[0]; // Primary sort
                _input.SorterKey = sort.Field;
                _input.SorterIsAsc = sort.Direction == CSortDirection.Ascending;
            }
            else
            {
                _input.SorterKey = nameof(AuditLogDto.ExecutionTime);
                _input.SorterIsAsc = false;
            }

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
            _viewingLog = log;
            _isDetailsDialogVisible = true;
            StateHasChanged();
        }

        private CBadgeSemantic GetChangeTypeStatus(CEntityChangeType type) => type switch
        {
            CEntityChangeType.Created => CBadgeSemantic.Completed,
            CEntityChangeType.Updated => CBadgeSemantic.Processing,
            CEntityChangeType.Deleted => CBadgeSemantic.Deleted,
            _ => CBadgeSemantic.Unknown
        };

        private string GetEntityFriendlyName(string? fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return "Unknown Entity";
            var parts = fullName.Split('.');
            return parts.Last();
        }

        private string FormatValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) || value == "null") return "-";
            if (value.Length > 200) return value[..200] + "...";
            return value;
        }

        private string? GetActiveParameters()
        {
            return _viewingLog?.Actions.FirstOrDefault()?.Parameters;
        }

        private void ShowSingleDeleteDialog(AuditLogDto log)
        {
            _itemToDelete = log;
            _isDeleteDialogVisible = true;
        }

        private void ShowBulkDeleteDialog()
        {
            _itemToDelete = null;
            if (_selectedLogs.Any() || (_dataTable?.TotalSelectedCount > 0))
            {
                _isDeleteDialogVisible = true;
            }
        }

        private async Task ExecuteDeleteAsync()
        {
            try
            {
                if (_itemToDelete != null)
                {
                    await AuditLogService.DeleteAsync(_itemToDelete.Id, _selectedService);
                    ToastNotification.Success(L("Common.DeletedSuccessfully"));
                }
                else if (_dataTable != null && _dataTable.IsAllEntireDatasetSelected)
                {
                    int originalPageSize = _input.PageSize;
                    int originalPageNumber = _input.PageNumber;

                    _input.PageSize = 100;
                    _input.PageNumber = 1;
                    _input.DisableTenantFilter = _canReadHostLogs;

                    int deletedCount = 0;
                    bool hasMore = true;

                    while (hasMore)
                    {
                        var result = await AuditLogService.GetListAsync(_input, _selectedService);

                        if (result.Items == null || !result.Items.Any())
                        {
                            hasMore = false;
                            break;
                        }

                        var deleteTasks = result.Items.Select(log => AuditLogService.DeleteAsync(log.Id, _selectedService));
                        await Task.WhenAll(deleteTasks);
                        deletedCount += result.Items.Count;
                    }

                    _input.PageSize = originalPageSize;
                    _input.PageNumber = originalPageNumber;

                    ToastNotification.Success(L("AuditLog.BulkDeletedSuccessfully", deletedCount));
                }
                else if (_selectedLogs.Any())
                {
                    var deleteTasks = _selectedLogs.Select(log => AuditLogService.DeleteAsync(log.Id, _selectedService));
                    await Task.WhenAll(deleteTasks);
                    ToastNotification.Success(L("AuditLog.BulkDeletedSuccessfully", _selectedLogs.Count));
                }

                _isDeleteDialogVisible = false;
                _itemToDelete = null;

                if (_dataTable != null)
                {
                    await _dataTable.ClearSelectionAsync();
                    await _dataTable.RefreshAsync();
                }
                else
                {
                    _selectedLogs.Clear();
                    await LoadLogsAsync();
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ToastNotification.Error(L("Common.DeleteFailed") + ": " + ex.Message);
            }
        }

        #region Browser Info Helpers

        private AppBrowserDetails ParseBrowserInfo(string? userAgent)
        {
            var details = new AppBrowserDetails();
            if (string.IsNullOrEmpty(userAgent)) return details;

            var lowerAgent = userAgent.ToLower();

            // OS Detection
            if (lowerAgent.Contains("windows")) details.OS = COperatingSystem.Windows;
            else if (lowerAgent.Contains("android")) details.OS = COperatingSystem.Android;
            else if (lowerAgent.Contains("iphone") || lowerAgent.Contains("ipad") || lowerAgent.Contains("ipod")) details.OS = COperatingSystem.iOS;
            else if (lowerAgent.Contains("mac os")) details.OS = COperatingSystem.MacOS;
            else if (lowerAgent.Contains("linux")) details.OS = COperatingSystem.Linux;

            // Browser/Client Detection
            if (lowerAgent.Contains("postman") || lowerAgent.Contains("postmanruntime")) details.Browser = CBrowserType.Postman;
            else if (lowerAgent.Contains("bruno")) details.Browser = CBrowserType.Bruno;
            else if (lowerAgent.Contains("curl")) details.Browser = CBrowserType.Curl;
            else if (lowerAgent.Contains("grpc")) details.Browser = CBrowserType.Grpc;
            else if (lowerAgent.Contains("message") || lowerAgent.Contains("queue") || lowerAgent.Contains("mq")) details.Browser = CBrowserType.MessageQueue;
            else if (lowerAgent.Contains("worker") || lowerAgent.Contains("job") || lowerAgent.Contains("background")) details.Browser = CBrowserType.BackgroundJob;
            else if (lowerAgent.Contains("edg/")) details.Browser = CBrowserType.Edge;
            else if (lowerAgent.Contains("chrome") && lowerAgent.Contains("safari")) details.Browser = CBrowserType.Chrome;
            else if (lowerAgent.Contains("firefox")) details.Browser = CBrowserType.Firefox;
            else if (lowerAgent.Contains("safari") && !lowerAgent.Contains("chrome")) details.Browser = CBrowserType.Safari;
            else if (lowerAgent.Contains("opera") || lowerAgent.Contains("opr/")) details.Browser = CBrowserType.Opera;

            return details;
        }
        #endregion
    }
}