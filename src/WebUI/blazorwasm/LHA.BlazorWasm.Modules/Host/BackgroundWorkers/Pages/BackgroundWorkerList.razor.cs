using LHA.BlazorWasm.Components;
using LHA.BlazorWasm.Components.Table;
using LHA.BlazorWasm.HttpApi.Client.Clients.BackgroundJobs;
using LHA.Shared.Contracts.BackgroundJobs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LHA.BlazorWasm.Modules.Host.BackgroundWorkers.Pages;

public partial class BackgroundWorkerList : LHAComponentBase
{
    [Inject] private BackgroundJobApiClient BackgroundJobApiClient { get; set; } = default!;

    private DataTable<RecurringJobDto> _dataTable = default!;
    private List<RecurringJobDto> _recurringJobs = [];
    private bool _isLoading;

    private List<LHA.BlazorWasm.Components.Breadcrumb.BreadcrumbItemModel> _breadcrumbItems =>
    [
        new() { Text = L("Menu.Home"), Href = "/", Icon = "bi bi-house" },
        new() { Text = L("BackgroundWorkers.Title"), Icon = "bi bi-gear" }
    ];



    private StatisticsDto? _statistics;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _statistics = await BackgroundJobApiClient.GetStatisticsAsync();
        }
        catch (Exception ex)
        {
            ToastNotification.Error($"{L("Common.Error")}: {ex.Message}");
        }
    }

    private async Task<DataTableResponse<RecurringJobDto>> GetDataTableResponseAsync(DataTableRequest request)
    {
        _isLoading = true;
        try
        {
            var jobs = await BackgroundJobApiClient.GetRecurringJobsAsync();
            if (jobs != null)
            {
                _recurringJobs = jobs;
            }
        }
        catch (Exception ex)
        {
            ToastNotification.Error($"{L("Common.Error")}: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
        }

        var pagedItems = _recurringJobs
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new DataTableResponse<RecurringJobDto>
        {
            Items = pagedItems,
            TotalCount = _recurringJobs.Count
        };
    }

    private async Task ReloadTableAsync()
    {
        if (_dataTable != null)
        {
            await _dataTable.RefreshAsync();
        }
    }

    private async Task TriggerJobAsync(string jobId)
    {
        if (await JS.InvokeAsync<bool>("confirm", L("BackgroundWorkers.TriggerConfirmation")))
        {
            try
            {
                _isLoading = true;
                var success = await BackgroundJobApiClient.TriggerRecurringJobAsync(jobId);
                if (success)
                {
                    ToastNotification.Success(L("BackgroundWorkers.TriggerSuccess"));
                    await ReloadTableAsync();
                }
            }
            catch (Exception ex)
            {
                ToastNotification.Error($"{L("Common.Error")}: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }
    }

    private async Task DeleteJobAsync(string jobId)
    {
        if (await JS.InvokeAsync<bool>("confirm", L("Common.DeleteConfirmation")))
        {
            try
            {
                _isLoading = true;
                var success = await BackgroundJobApiClient.DeleteRecurringJobAsync(jobId);
                if (success)
                {
                    ToastNotification.Success(L("Common.DeleteSuccess"));
                    await ReloadTableAsync();
                }
            }
            catch (Exception ex)
            {
                ToastNotification.Error($"{L("Common.Error")}: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }
    }

    private string GetCronDescription(string cronExpression)
    {
        if (string.IsNullOrWhiteSpace(cronExpression)) return "-";
        try
        {
            var options = new CronExpressionDescriptor.Options
            {
                Locale = Localizer.State.CurrentCulture,
                DayOfWeekStartIndexZero = false,
                Use24HourTimeFormat = true,
                Verbose = true
            };
            return CronExpressionDescriptor.ExpressionDescriptor.GetDescription(cronExpression, options);
        }
        catch
        {
            return cronExpression;
        }
    }
}
