using System.ComponentModel.DataAnnotations;

namespace ErpService.ValidationAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NotEmptyGuidAttribute : ValidationAttribute
    {
        public NotEmptyGuidAttribute()
        {
            ErrorMessage = "Guid cannot be empty.";
        }

        public override bool IsValid(object? value)
        {
            if (value is Guid guidValue)
            {
                return guidValue != Guid.Empty;
            }
            return false;
        }
    }
}
