using Google.Apis.Sheets.v4.Data;
using isRock.LineBot;
using LineBot.Interface;
using LineBot.Models;
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
                if ((request.ServiceDate - DateTime.UtcNow.AddHours(8).Date.AddHours(0)).TotalDays < 3)
                {
                    DateTime oneDayBefore = request.ServiceDate.AddDays(-1);
                    DateTime today = DateTime.UtcNow.AddHours(8).Date.AddHours(0);

                    if (today == oneDayBefore)
                    {
                        // 發送通知
                        if (today == oneDayBefore)
                        {
                            _bot.PushMessage(_userId,
                                @$"明天有一個重要的預約，請留意：
訂單時間：{request.ServiceDate.ToString("yyyy-MM-dd")} {request.ServiceTime},
個案大名：{request.FullName},
上車地點：{request.PickupLocation},
下車地點：{request.DropOffLocation},
聯絡電話：{request.ContactPhoneNumber},
服務項目：{request.ServiceType},
備註：{request.Notes}");

                            _bot.PushMessage(_admin,
                                @$"明天有一個重要的預約，請留意：
訂單時間：{request.ServiceDate.ToString("yyyy-MM-dd")} {request.ServiceTime},
個案大名：{request.FullName},
上車地點：{request.PickupLocation},
下車地點：{request.DropOffLocation},
聯絡電話：{request.ContactPhoneNumber},
服務項目：{request.ServiceType},
備註：{request.Notes}");
                        }
                    }

                }
            }

            return Ok();
        }


        /// <summary>
        /// 手動通知
        /// </summary>
        /// <returns></returns>
        public IActionResult ManualNotification(int Id)
        {
            ReservationRequest reservationRequests = _googleSheets.GetReservation(Id);

            //通知乘客
            if(reservationRequests.MemberNotify.Length > 0)
            {
                _bot.PushMessage(_admin,
                        @$"早安您好：
您預約{reservationRequests.ServiceDate}早上{reservationRequests.ServiceTime}到{reservationRequests.DropOffLocation}11:00回程
已為您調配司機
司機資訊如下 ：
隊編：68611
姓名：{reservationRequests.Driver}
車型:納智捷V7白色
車號:TDW-9237
電話:0983502815
若您時間上有提早或延後 麻煩提早半小時與司機聯繫
我們儘量配合您時間");
            }


            //通知司機
            if(reservationRequests.DriverNotify.Length > 0)
            {

            }

            return View(reservationRequests);
        }
    }
}
