using isRock.LineBot;
using LineBot.Interface;
using LineBot.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LineBot.Controllers
{
    public class DriverController : Controller
    {
        private readonly ILogger<DriverController> _logger;
        private readonly Bot _bot;
        private readonly IGoogleSheets _googleSheets;


        public DriverController(ILogger<DriverController> logger, Bot bot, IGoogleSheets googleSheets)
        {
            _logger = logger;
            _bot = bot;
            _googleSheets = googleSheets;
        }


        public ActionResult Create()
        {
            string lineId = HttpContext.Request.Query["LineId"];

            Driver driver = new Driver
            {
                LineId = lineId,
            };

            return View(driver);
        }


        [HttpPost]
        public ActionResult Create(Driver driver)
        {
            if (ModelState.IsValid)
            {
                _googleSheets.CreateDriverInfo(driver);

                _bot.PushMessage(driver.LineId,
                    $@"恭喜您資料已新增成功。");

                // 使用 TempData 儲存成功訊息
                TempData["Message"] = "新增成功";

                // DriverResult
                return RedirectToAction("DriverResult");
            }
            else
            {
                return View(driver);
            }
        }

    }
}
