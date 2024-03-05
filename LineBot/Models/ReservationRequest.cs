using System.ComponentModel.DataAnnotations;

namespace LineBot.Models
{
    public class ReservationRequest
    {

        [Display(Name = "訂單編號")]
        public int? Id { get; set; }


        [Display(Name = "使用者編號")]
        public string UserId { get; set; }


        [Required(ErrorMessage = "請輸入名字")]
        [Display(Name = "個案大名")]
        public string FullName { get; set; }


        [Required(ErrorMessage = "請輸入預約服務日期")]
        [Display(Name = "預約服務日期")]
        public DateTime ServiceDate { get; set; }


        [Required(ErrorMessage = "請輸入預約服務時間")]
        [Display(Name = "預約服務時間")]
        public TimeSpan ServiceTime { get; set; }


        [Required(ErrorMessage = "請輸入上車地點")]
        [Display(Name = "上車地點")]
        public string PickupLocation { get; set; }


        [Required(ErrorMessage = "請輸入下車地點")]
        [Display(Name = "下車地點")]
        public string DropOffLocation { get; set; }


        [Display(Name = "回程服務時間")]
        public TimeSpan? ReturnServiceTime { get; set; }


        [Required(ErrorMessage = "請輸入就醫目的")]
        [Display(Name = "就醫目的")]
        public string MedicalPurpose { get; set; }


        [Display(Name = "陪同人數")]
        public int AccompanyingPersons { get; set; }


        [Required(ErrorMessage = "請輸入聯絡人稱謂")]
        [Display(Name = "聯絡人稱謂")]
        public string ContactTitle { get; set; }


        [Required(ErrorMessage = "請輸入行動電話")]
        [Display(Name = "行動電話")]
        [RegularExpression(@"^09\d{8}$", ErrorMessage = "請輸入正確的行動電話格式，例如：0912345678")]
        public string ContactPhoneNumber { get; set; }


        [Display(Name = "服務項目")]
        public string ServiceType { get; set; }


        [Display(Name = "長照資格")]
        public string LongTermCareQualification { get; set; }


        [Required(ErrorMessage = "請輸入注意事項")]
        [Display(Name = "注意事項")]
        public string Notes { get; set; }
    }

}
