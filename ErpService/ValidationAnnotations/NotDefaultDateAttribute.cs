using System.ComponentModel.DataAnnotations;

namespace ErpService.ValidationAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NotDefaultDateAttribute : ValidationAttribute
    {
        public NotDefaultDateAttribute()
        {
            ErrorMessage = "The date must be set and cannot be default.";
        }

        public override bool IsValid(object? value)
        {
            if (value is DateTime dt)
            {
                return dt != default; // ensures it's not 0001-01-01
            }
            return false;
        }
    }
}

