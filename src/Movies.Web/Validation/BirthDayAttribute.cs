using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Movies.Web.Validation;
public class BirthDayAttribute : ValidationAttribute, IClientModelValidator
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime dt || (value is string s && DateTime.TryParse(s, CultureInfo.InvariantCulture, out dt)))
        {
            var currentYear = DateTime.Now;

            if (dt <= currentYear)
            {
                return ValidationResult.Success;
            }
        }

        return new ValidationResult(GetErrorMessage());
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-birthday", GetErrorMessage());
    }

    private static bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
    {
        if (attributes.ContainsKey(key))
        {
            return false;
        }

        attributes.Add(key, value);
        return true;
    }

    private static string GetErrorMessage()
    {
        var error = $"Birthday can never be in the future";
        return error;
    }
}