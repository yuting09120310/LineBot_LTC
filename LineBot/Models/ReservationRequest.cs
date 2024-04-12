using LineBot.Attribute;
using System.ComponentModel.DataAnnotations;

namespace LineBot.Models
{
    public class ReservationRequest
    {

        [Display(Name = "訂單編號")]
        public int? Id { get; set; }


        [Display(Name = "用戶Line編號")]
        public string LineId { get; set; }


        [Required(ErrorMessage = "請輸入名字")]
        [Display(Name = "個案大名")]
        public string FullName { get; set; }


        [Required(ErrorMessage = "請輸入預約服務日期")]
        [Display(Name = "預約服務日期")]
        [DataType(DataType.Date)]
        [ReservationDate(ErrorMessage = "預約日期必須至少提前 3 天。")]
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
        [ReservationTime(ErrorMessage = "預約時間不能晚於回程時間。")]
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
        [RegularExpression(@"^((09\d{8})|([0-9]{2,3}-?[0-9]{6,8}))$", ErrorMessage = "請輸入正確的電話格式")]
        public string ContactPhoneNumber { get; set; }


        [Display(Name = "服務項目")]
        public string ServiceType { get; set; }


        [Display(Name = "長照資格")]
        public string LongTermCareQualification { get; set; }


        [Display(Name = "注意事項")]
        public string? Notes { get; set; }


        [Display(Name = "訂單司機")]
        public string? Driver { get; set; }


        [Display(Name = "通知司機")]
        public string? DriverNotify { get; set; }


        [Display(Name = "通知乘客")]
        public string? MemberNotify { get; set; }
    }

}
