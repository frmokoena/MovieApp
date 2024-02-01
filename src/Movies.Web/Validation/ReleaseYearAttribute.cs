using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Movies.Web.Validation;


public class ReleaseYearAttribute(int year = 1900) : ValidationAttribute, IClientModelValidator
{
    private readonly int _year = year;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is int releaseYear || (value is string s && int.TryParse(s, out releaseYear)))
        {
            var currentYear = DateTime.Now.Year;

            if (releaseYear >= _year && releaseYear <= currentYear)
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
        MergeAttribute(context.Attributes, "data-val-release", GetErrorMessage());

        var year = _year.ToString(CultureInfo.InvariantCulture);
        MergeAttribute(context.Attributes, "data-val-release-year", year);
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

    private string GetErrorMessage()
    {
        var error = $"Release year must between {_year} and the current year";
        return error;
    }
}