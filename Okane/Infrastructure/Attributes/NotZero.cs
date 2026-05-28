using System.ComponentModel.DataAnnotations;

namespace Okane.Infrastructure.Attributes;

public class NotZero : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if(value is decimal decimalValue)
        {
            return decimalValue != 0;
        }

        return false;
    }
}
