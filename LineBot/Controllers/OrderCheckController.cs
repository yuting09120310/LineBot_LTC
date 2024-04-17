using Google.Apis.Sheets.v4.Data;
using isRock.LineBot;
using LineBot.Interface;
using LineBot.Models;
using LineBot.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace LineBot.Controllers
{
    public class OrderCheckController : Controller
    {
        private readonly Bot _bot;
        private readonly IGoogleSheets _googleSheets;
        private string _userId = "U0040d9605949cdceadab64168d00c335";
        private string _admin = "U41f5db0d177d113e24385ef4a8aba148";

        public OrderCheckController(Bot bot, IGoogleSheets googleSheets)
        {
            _bot = bot;
            _googleSheets = googleSheets;
        }

        /// <summary>
        /// 每日通知  由電腦排程自動通知
        /// </summary>
        /// <returns></returns>
        public IActionResult DailyOrderCheck()
        {
            List<ReservationRequest> AllReservationRequests = _googleSheets.GetAllReservation();

            foreach (ReservationRequest request in AllReservationRequests)
            {
                // 檢查前三天訂單並通知相應人員
                DateTime threeDaysBefore = DateTime.UtcNow.AddHours(8).Date.AddDays(3); // 取得三天前的日期
                if (request.ServiceDate.Date == threeDaysBefore)
                {
                    // 發送通知
                    _bot.PushMessage(_userId,
                        @$"距離預約時間還有三天，請務必留意：
訂單編號：{request.Id}
訂單時間：{request.ServiceDate.ToString("yyyy-MM-dd")} {request.ServiceTime},
個案大名：{request.FullName},
上車地點：{request.PickupLocation},
下車地點：{request.DropOffLocation},
聯絡電話：{request.ContactPhoneNumber},
服務項目：{request.ServiceType},
備註：{request.Notes}");

                    _bot.PushMessage(_admin,
                        @$"距離預約時間還有三天，請留意：
訂單編號：{request.Id}
訂單時間：{request.ServiceDate.ToString("yyyy-MM-dd")} {request.ServiceTime},
個案大名：{request.FullName},
上車地點：{request.PickupLocation},
下車地點：{request.DropOffLocation},
聯絡電話：{request.ContactPhoneNumber},
服務項目：{request.ServiceType},
備註：{request.Notes}");
                }

            }

            return Ok();
        }


        /// <summary>
        /// 手動通知
        /// </summary>
        /// <returns></returns>
        public IActionResult ManualNotification(int id)
        {
            // 取得訂單資訊
            ReservationRequest reservationRequests = _googleSheets.GetReservation(id);

            // 取得司機資訊
            Driver driver = _googleSheets.GetDriverInfo(reservationRequests.Driver);


            // 通知乘客
            if (reservationRequests.MemberNotify.Length > 0)
            {
                string message = string.Empty;
                message = @$"早安您好：
您預約{reservationRequests.ServiceDate.ToString("MM/dd")}早上{reservationRequests.ServiceTime.ToString(@"hh\:mm")}到{reservationRequests.DropOffLocation}";

                if(reservationRequests.ReturnServiceTime!= null)
                {
                    message += $"{reservationRequests.ReturnServiceTime.ToString()}回程";
                }

                message += @$"已為您調配司機
司機資訊如下 ：
隊編：{driver.TeamId}
姓名：{driver.Name}
車型:{driver.CarModel}
車號:{driver.CarNumber}
電話:{driver.ContactNumber}
若您時間上有提早或延後 麻煩提早半小時與司機聯繫
我們儘量配合您時間";


                _bot.PushMessage(reservationRequests.LineId,message);
            }


            // 通知司機
            if(reservationRequests.DriverNotify.Length > 0)
            {
                string message = string.Empty;
                message += $@"{reservationRequests.FullName}
{reservationRequests.ContactPhoneNumber}
{reservationRequests.PickupLocation}
{reservationRequests.ServiceDate.ToString("MM/dd")}   {reservationRequests.ServiceTime.ToString(@"hh\:mm")}去{reservationRequests.DropOffLocation}{reservationRequests.MedicalPurpose}";

                if (reservationRequests.ReturnServiceTime != null)
                {
                    message += $"{reservationRequests.ReturnServiceTime?.ToString(@"hh\:mm")}回程";
                }

                _bot.PushMessage(driver.LineId, message);
            }


            // 將整理出來的資料封裝進viewmodel 帶到view顯示
            NotificationViewModel viewModel = new NotificationViewModel
            {
                ReservationRequest = reservationRequests,
                Driver = driver
            };


            return View(viewModel);
        }
    }
}
