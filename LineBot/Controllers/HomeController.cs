using isRock.LineBot;
using LineBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace LineBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Bot _bot;


        public HomeController(ILogger<HomeController> logger, Bot bot)
        {
            _logger = logger;
            _bot = bot;
        }


        public IActionResult Index()
        {
            GetSelectListItem();
            return View();
        }


        [HttpPost]
        public IActionResult Index(ReservationRequest reservationRequest)
        {
            if(ModelState.IsValid)
            {
                _bot.PushMessage(reservationRequest.UserId, reservationRequest.FullName);
                return View();
            }
            else
            {
                GetSelectListItem();
                return View();
            }
        }


        /// <summary>
        /// 取得下拉選單
        /// </summary>
        private void GetSelectListItem()
        {
            // 設定 LongTermCareQualification 的選項
            ViewBag.LongTermCareQualification = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "有" },
                new SelectListItem { Value = "false", Text = "沒有" }
            };
        }
    }
}