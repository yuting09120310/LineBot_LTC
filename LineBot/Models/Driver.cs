using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LineBot.Models
{
    public class Driver
    {
        [DisplayName("司機編號")]
        public int DriverId { get; set; }

        [DisplayName("隊編")]
        public string TeamId { get; set; }

        [DisplayName("司機Line編號")]
        public string LineId { get; set; }

        [DisplayName("姓名")]
        public string Name { get; set; }

        [DisplayName("車型")]
        public string CarModel { get; set; }

        [DisplayName("車號")]
        public string CarNumber { get; set; }

        [DisplayName("聯絡電話")]
        public string ContactNumber { get; set; }
    }

}
