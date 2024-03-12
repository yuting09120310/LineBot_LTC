using isRock.LineBot;
using LineBot.Interface;
using LineBot.Models;
using Microsoft.AspNetCore.Mvc;

namespace LineBot.Controllers
{
    public class DailyOrderCheckController : Controller
    {
        private readonly Bot _bot;
        private readonly IGoogleSheets _googleSheets;
        private string _userId = "Ua4a1c1f2d4fd6403641b6e9107269859";

        public DailyOrderCheckController(Bot bot, IGoogleSheets googleSheets)
        {
            _bot = bot;
            _googleSheets = googleSheets;
        }


        public IActionResult Index()
        {
            List<ReservationRequest> AllReservationRequests = _googleSheets.ListReadData();

            foreach (ReservationRequest request in AllReservationRequests)
            {
                if ((request.ServiceDate - DateTime.Now).TotalDays < 3)
                {
                    DateTime oneDayBefore = request.ServiceDate.AddDays(-1);
                    DateTime threeDaysBefore = request.ServiceDate.AddDays(-3);

                    DateTime today = DateTime.Today;

                    if (today == oneDayBefore || today == threeDaysBefore)
                    {
                        // 發送通知
                        if (today == oneDayBefore)
                        {
                            _bot.PushMessage("Ua4a1c1f2d4fd6403641b6e9107269859",
                                @$"明天有一個重要的預約，請留意：
訂單時間：{request.ServiceDate.ToString("yyyy-MM-dd")} {request.ServiceTime},
個案大名：{request.FullName},
上車地點：{request.PickupLocation},
下車地點：{request.DropOffLocation},
聯絡電話：{request.ContactPhoneNumber},
服務項目：{request.ServiceType},
備註：{request.Notes}");
                        }
                        else if (today == threeDaysBefore)
                        {
                            _bot.PushMessage("Ua4a1c1f2d4fd6403641b6e9107269859", 
                                @$"距離預約時間還有三天，請務必留意：
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

            return View();
        }
    }
}
