namespace JSONAdminEditor.Application.Models.Configuration;

public class AfterHours
{
    public ExceptionOfValidation? ExceptionOfValidation { get; set; }
    public required RestrictedHoursPeriod RestrictedHoursPeriod { get; set; }
}
