namespace LHA.Notification.Application.Contracts;

public record NotificationPagedResultDto<TDto>(
    List<TDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    bool HasNextPage);
