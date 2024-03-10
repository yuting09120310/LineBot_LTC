using System.ComponentModel.DataAnnotations;

namespace LineBot.Attribute
{
    public class ReservationDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime reservationDate = (DateTime)value;
            DateTime minDate = DateTime.Now.AddDays(3);

            if (reservationDate < minDate)
            {
                return new ValidationResult("預約日期必須至少提前 3 天。");
            }

            return ValidationResult.Success;
        }
    }

}
