using System;
using System.ComponentModel.DataAnnotations;

namespace PDR.PatientBooking.Service.Attributes
{
    public class IsDateInPastAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!DateTime.TryParse(value.ToString(), out var dateToCheck))
                return new ValidationResult(ErrorMessage);

            if (dateToCheck <= DateTime.Now) //LPN if this is going international might need to tweak this with the local time zone data
                return new ValidationResult(ErrorMessage);
            
            return ValidationResult.Success;
        }
    }
}
