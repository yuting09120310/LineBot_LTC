using LineBot.Models;
using System.ComponentModel.DataAnnotations;

namespace LineBot.Attribute
{
    public class ReservationTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (ReservationRequest)validationContext.ObjectInstance;

            if (model.ReturnServiceTime.HasValue && model.ServiceTime > model.ReturnServiceTime)
            {
                return new ValidationResult("預約時間不能晚於回程時間。");
            }

            return ValidationResult.Success;
        }
    }

}
