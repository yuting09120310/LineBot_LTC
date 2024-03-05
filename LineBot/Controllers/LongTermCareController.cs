using isRock.LineBot;
using LineBot.Interface;
using LineBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LineBot.Controllers
{
    public class LongTermCareController : Controller
    {
        private readonly ILogger<LongTermCareController> _logger;
        private readonly Bot _bot;
        private readonly IGoogleSheets _googleSheets;


        public LongTermCareController(ILogger<LongTermCareController> logger, Bot bot, IGoogleSheets googleSheets)
        {
            _logger = logger;
            _bot = bot;
            _googleSheets = googleSheets;
        }


        public IActionResult Create()
        {
            GetSelectListItem();

            string userId = HttpContext.Request.Query["UserId"];
            if (userId == null)
            {
                TempData["Message"] = "預約異常，請透過Line重新開啟此網頁";
                return RedirectToAction("ReservationResult");
            }

            ReservationRequest reservationRequest = new ReservationRequest()
            {
                UserId = userId!,
                ServiceDate = DateTime.UtcNow,
            };

            return View(reservationRequest);
        }


        /// <summary>
        /// 送出預約資料處理
        /// </summary>
        /// <param name="reservationRequest">客戶資料</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(ReservationRequest reservationRequest)
        {
            GetSelectListItem();

            if (ModelState.IsValid)
            {
                _bot.PushMessage(reservationRequest.UserId,
                    $@"預約完成，資料如下

個案大名：{reservationRequest.FullName}
預約服務日期：{reservationRequest.ServiceDate}
預約服務時間：{reservationRequest.ServiceTime}
上車地點：{reservationRequest.PickupLocation}
下車地點：{reservationRequest.DropOffLocation}
回程需求：{reservationRequest.ReturnServiceTime}
就醫目的：{reservationRequest.MedicalPurpose}
陪同人數：{reservationRequest.AccompanyingPersons}
聯絡人稱謂：{reservationRequest.ContactTitle}
聯絡電話：{reservationRequest.ContactPhoneNumber}
服務項目：{reservationRequest.ServiceType}
長照資格：{reservationRequest.LongTermCareQualification}
注意事項：{reservationRequest.Notes}");


                _googleSheets.CreateGoogleSheet(reservationRequest);

                // 使用 TempData 儲存成功訊息
                TempData["Message"] = "預約成功";

                // 跳轉到ReservationResult
                return RedirectToAction("ReservationResult");
            }
            else
            {
                GetSelectListItem();
                return View(reservationRequest);
            }
        }


        public IActionResult Search()
        {
            string userId = HttpContext.Request.Query["UserId"];
            if (userId == null)
            {
                TempData["Message"] = "預約異常，請透過Line重新開啟此網頁";
                return RedirectToAction("ReservationResult");
            }

            List<ReservationRequest> LstReservationRequests = _googleSheets.ListReadGoogleSheet(userId);

            return View(LstReservationRequests);
        }


        public IActionResult Edit(int id)
        {
            GetSelectListItem();
            ReservationRequest reservationRequests = _googleSheets.ReadGoogleSheet(id);

            return View(reservationRequests);
        }


        [HttpPost]
        public IActionResult Edit(ReservationRequest reservationRequest)
        {
            GetSelectListItem();
            _googleSheets.UpdateGoogleSheet(reservationRequest);

            return View();
        }

        /// <summary>
        /// 預約完成
        /// </summary>
        /// <returns></returns>
        public IActionResult ReservationResult()
        {
            return View();
        }


        /// <summary>
        /// 取得下拉選單
        /// </summary>
        private void GetSelectListItem()
        {
            // 設定 LongTermCareQualification 的選項
            ViewBag.LongTermCareQualification = new List<SelectListItem>
            {
                new SelectListItem { Value = "有", Text = "有" },
                new SelectListItem { Value = "沒有", Text = "沒有" }
            };
        }
    }
}
