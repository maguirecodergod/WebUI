namespace LHA.Notification.Application.Contracts;

public record DeviceListDto(
    List<DeviceDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    bool HasNextPage);
