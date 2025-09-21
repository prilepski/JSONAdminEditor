namespace JSONAdminEditor.Application.Models;

public class AfterHours
{
    public ExceptionOfValidation? ExceptionOfValidation { get; set; }
    public List<DayOfWeek>? RestrictedDays { get; set; }
    public required RestrictedHoursPeriod RestrictedHoursPeriod { get; set; }
}
