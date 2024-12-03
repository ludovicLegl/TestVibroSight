
using System.Globalization;
using System.Windows.Controls;

namespace InterviewTest.App.View.Validation
{
    public class PositiveNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || !double.TryParse(value.ToString(), out double number) || number <= 0)
            {
                return new ValidationResult(false, "Please enter a positive number.");
            }
            return ValidationResult.ValidResult;
        }
    }
}
