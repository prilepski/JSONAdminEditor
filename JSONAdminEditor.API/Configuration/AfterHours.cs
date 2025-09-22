namespace JSONAdminEditor.Application.Models.Structure;

public class AfterHours
{
    public ExceptionOfValidation? ExceptionOfValidation { get; set; }
    public required RestrictedHoursPeriod RestrictedHoursPeriod { get; set; }
}
