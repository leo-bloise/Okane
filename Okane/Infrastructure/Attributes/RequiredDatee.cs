using System.ComponentModel.DataAnnotations;

namespace Okane.Infrastructure.Attributes;

public class RequiredDate : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if(value is DateTime date)
        {
            return !date.Equals(DateTime.MinValue);
        }

        return false;
    }
}
