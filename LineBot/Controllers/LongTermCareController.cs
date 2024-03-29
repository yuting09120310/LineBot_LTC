﻿using isRock.LineBot;
using LineBot.Interface;
using LineBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

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
                _googleSheets.CreateData(reservationRequest);

                _bot.PushMessage(reservationRequest.UserId,
                    $@"親愛的 {reservationRequest.FullName} 您好，
您的預約已經確認完成，預計服務日期為 {reservationRequest.ServiceDate.ToString("yyyy-MM-dd")}，
感謝您的預約，若有任何問題請隨時聯絡我們。");

                // 使用 TempData 儲存成功訊息
                TempData["Message"] = "預約成功";

                // 跳轉到ReservationResult
                return RedirectToAction("ReservationResult");
            }
            else
            {
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

            List<ReservationRequest> LstReservationRequests = _googleSheets.ListReadData(userId);

            return View(LstReservationRequests);
        }


        public IActionResult Edit(int id)
        {
            GetSelectListItem();
            ReservationRequest reservationRequests = _googleSheets.ReadData(id);

            return View(reservationRequests);
        }


        [HttpPost]
        public IActionResult Edit(ReservationRequest reservationRequest)
        {
            GetSelectListItem();

            if (ModelState.IsValid)
            {
                _googleSheets.UpdateData(reservationRequest);

                // 使用 TempData 儲存成功訊息
                TempData["Message"] = "訂單修改成功";

                // 跳轉到ReservationResult
                return RedirectToAction("ReservationResult");
            }
            else
            {
                return View(reservationRequest);
            }
        }


        [HttpPost]
        public IActionResult Delete(int Id)
        {
            _googleSheets.DeleteData(Id);

            // 使用 TempData 儲存成功訊息
            TempData["Message"] = "訂單刪除成功";

            // 跳轉到ReservationResult
            return RedirectToAction("ReservationResult");
        }
        

        /// <summary>
        /// 跳轉結果
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
            ViewBag.MedicalPurpose = new List<SelectListItem>
            {
                new SelectListItem { Value = "看診", Text = "看診" },
                new SelectListItem { Value = "抽血", Text = "抽血" },
                new SelectListItem { Value = "復健", Text = "復健" },
                new SelectListItem { Value = "洗腎", Text = "洗腎" },
                new SelectListItem { Value = "其他", Text = "其他" },
            };


            ViewBag.ServiceType = new List<SelectListItem>
            {
                new SelectListItem { Value = "爬梯機", Text = "爬梯機" },
                new SelectListItem { Value = "輪椅車", Text = "輪椅車" },
                new SelectListItem { Value = "一般車", Text = "一般車" },
            };


            // 設定 LongTermCareQualification 的選項
            ViewBag.LongTermCareQualification = new List<SelectListItem>
            {
                new SelectListItem { Value = "有", Text = "有" },
                new SelectListItem { Value = "沒有", Text = "沒有" }
            };
        }
    }
}
