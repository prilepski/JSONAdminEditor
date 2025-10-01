using JSONAdminEditor.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace JSONAdminEditor.Validation;

public static class ModelValidationExtensions
{
    public static bool IsValid(this AfterHours afterHours, out List<ValidationResult> validationResults)
    {
        validationResults = new List<ValidationResult>();
        var context = new ValidationContext(afterHours);
        return Validator.TryValidateObject(afterHours, context, validationResults, true);
    }

    public static bool IsValid(this EventMapping eventMapping, out List<ValidationResult> validationResults)
    {
        validationResults = new List<ValidationResult>();
        var context = new ValidationContext(eventMapping);
        return Validator.TryValidateObject(eventMapping, context, validationResults, true);
    }

    public static bool IsValid(this NotificationMapping notificationMapping, out List<ValidationResult> validationResults)
    {
        validationResults = new List<ValidationResult>();
        var context = new ValidationContext(notificationMapping);
        return Validator.TryValidateObject(notificationMapping, context, validationResults, true);
    }
}