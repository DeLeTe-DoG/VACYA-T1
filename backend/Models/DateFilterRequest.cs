namespace backend.Models;

public class DateFilterRequest
{
  private DateTime _dateFrom;
  private DateTime _dateTo;

  public DateTime DateFrom
  {
    get => _dateFrom;
    set => _dateFrom = DateTime.SpecifyKind(value, DateTimeKind.Utc);
  }

  public DateTime DateTo
  {
    get => _dateTo;
    set => _dateTo = DateTime.SpecifyKind(value, DateTimeKind.Utc);
  }
}