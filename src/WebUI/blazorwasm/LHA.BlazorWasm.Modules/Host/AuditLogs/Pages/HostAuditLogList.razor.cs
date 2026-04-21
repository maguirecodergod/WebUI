using LHA.Auditing;
using LHA.BlazorWasm.Components;
using LHA.BlazorWasm.Components.Pickers.Core;
using LHA.BlazorWasm.Components.Select;
using LHA.BlazorWasm.Components.Table;
using LHA.BlazorWasm.Shared;
using LHA.BlazorWasm.Shared.Models.StatusBadge;
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
        private DataTable<AuditLogDto>? _dataTable;
        private List<AuditLogDto> _selectedLogs = new();
        private bool _isDeleteDialogVisible;
        private AuditLogDto? _itemToDelete;
        private bool _canReadHostLogs;
        private bool _canDelete; // We'll derive this from HostRead for now since no specific delete permission exists
        private bool _isDetailsDialogVisible;
        private AuditLogDto? _viewingLog;

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
            _canReadHostLogs = PermissionService.HasPermission(AuditLogPermissions.AuditLogs.HostRead);
            _canDelete = _canReadHostLogs; // Restrict delete to Host-level access by default
            await LoadLogsAsync();
        }

        private async Task LoadLogsAsync()
        {
            try
            {
                _input.StartTime = _executionTimeRange.Start?.ToUniversalTime();
                _input.EndTime = _executionTimeRange.End?.ToUniversalTime();

                var result = _canReadHostLogs
                    ? await AuditLogService.GetHostListAsync(_input)
                    : await AuditLogService.GetListAsync(_input);

                _logs = result.Items.ToList();
                _totalCount = result.TotalCount;
                StateHasChanged();
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
                _input.SorterIsAsc = sort.Direction == SortDirection.Ascending;
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
            if (_selectedLogs.Any())
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
                    await AuditLogService.DeleteAsync(_itemToDelete.Id);
                    ToastNotification.Success(L("Common.DeletedSuccessfully"));
                }
                else if (_selectedLogs.Any())
                {
                    var deleteTasks = _selectedLogs.Select(log => AuditLogService.DeleteAsync(log.Id));
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
            else if (lowerAgent.Contains("edg/")) details.Browser = CBrowserType.Edge;
            else if (lowerAgent.Contains("chrome") && lowerAgent.Contains("safari")) details.Browser = CBrowserType.Chrome;
            else if (lowerAgent.Contains("firefox")) details.Browser = CBrowserType.Firefox;
            else if (lowerAgent.Contains("safari") && !lowerAgent.Contains("chrome")) details.Browser = CBrowserType.Safari;
            else if (lowerAgent.Contains("opera") || lowerAgent.Contains("opr/")) details.Browser = CBrowserType.Opera;

            return details;
        }

        public class AppBrowserDetails
        {
            public CBrowserType Browser { get; set; } = CBrowserType.Unknown;
            public COperatingSystem OS { get; set; } = COperatingSystem.Unknown;

            public bool IsSvgIcon => Browser is CBrowserType.Postman or CBrowserType.Bruno or CBrowserType.Curl;

            public string BrowserIcon => Browser switch
            {
                CBrowserType.Chrome => "bi bi-google",
                CBrowserType.Edge => "bi bi-browser-edge",
                CBrowserType.Firefox => "bi bi-browser-firefox",
                CBrowserType.Safari => "bi bi-browser-safari",
                CBrowserType.Opera => "bi bi-browser-opera",
                _ => "bi bi-browser-chrome"
            };

            public string OSIcon => OS switch
            {
                COperatingSystem.Windows => "bi bi-windows",
                COperatingSystem.Android => "bi bi-android2",
                COperatingSystem.iOS => "bi bi-apple",
                COperatingSystem.MacOS => "bi bi-apple",
                COperatingSystem.Linux => "bi bi-ubuntu",
                _ => "bi bi-laptop"
            };

            public string BrowserName => Browser.ToString();
            public string OSName => OS.ToString();
        }

        public enum CBrowserType
        {
            Unknown,
            Chrome,
            Edge,
            Firefox,
            Safari,
            Opera,
            Postman,
            Bruno,
            Curl,
            Terminal,
            Other
        }
        public enum COperatingSystem { Unknown, Windows, MacOS, Linux, Android, iOS }

        #endregion
    }
}