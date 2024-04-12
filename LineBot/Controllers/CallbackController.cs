using isRock.LineBot;
using LineBot.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Web;
using static System.Net.Mime.MediaTypeNames;

namespace LineBot.Controllers
{
    public class CallbackController : Controller
    {
        private readonly Bot _bot;
        private readonly HttpContext _context;
        private readonly string _webUrl;
        private readonly IGoogleSheets _googleSheets;


        public CallbackController(Bot bot, IHttpContextAccessor contextAccessor, string webUrl, IGoogleSheets googleSheets)
        {
            _bot = bot;
            _context = contextAccessor.HttpContext!;
            _webUrl = webUrl;
            _googleSheets = googleSheets;
        }


        [HttpPost]
        public async Task<IActionResult> Index()
        {
            StreamReader sr = new StreamReader(_context.Request.Body);
            string reqBody = await sr.ReadToEndAsync();
            ReceivedMessage rcvMsg = isRock.LineBot.Utility.Parsing(reqBody);

            foreach (Event e in rcvMsg.events)
            {
                if (!string.IsNullOrEmpty(e.message?.text))
                {
                    string replyToken = e.replyToken;
                    string userId = e.source.userId;
                    string userName = _bot.GetUserInfo(userId).displayName;
                    string userMsg = e.message.text;

                    if (userMsg == "預約")
                    {
                        // 處理按鈕事件
                        HandleButtonEvent(replyToken, userId, "LongTermCare", "Create", "預約", "Reserve.jpg");
                    }
                    else if(userMsg == "查詢")
                    {
                        HandleButtonEvent(replyToken, userId , "LongTermCare", "Search", "查詢", "Search.jpg");
                    }
                    else if (userMsg == "建立司機資料")
                    {
                        HandleButtonEvent(replyToken, userId, "Driver", "Create", "建立司機資料", "Search.jpg");
                    }
                }
            }
            return Ok();
        }


        /// <summary>
        /// 圖文回覆
        /// </summary>
        /// <param name="replyToken"></param>
        /// <param name="userId"></param>
        /// <param name="action"></param>
        /// <param name="actionText"></param>
        private void HandleButtonEvent(string replyToken, string userId, string controller, string action, string actionText, string img)
        {
            string queryString = $"?LineId={HttpUtility.UrlEncode(userId)}";

            ButtonsTemplate btnTmpl = new ButtonsTemplate();
            btnTmpl.altText = "請用手機檢視";
            btnTmpl.text = $"點擊網址進行{actionText}";
            btnTmpl.title = $"{actionText}";
            btnTmpl.thumbnailImageUrl = new Uri(_webUrl + $"/img/{img}");
            btnTmpl.actions = new List<TemplateActionBase>() {
                new UriAction()
                {
                    label = $"點我{actionText}",
                    uri = new Uri($"{_webUrl}/{controller}/{action}{queryString}")
                }
            };

            _bot.ReplyMessage(replyToken, new TemplateMessage(btnTmpl));
        }


        /// <summary>
        /// 鸚鵡Function
        /// </summary>
        /// <param name="replyToken"></param>
        /// <param name="userName"></param>
        /// <param name="userMsg"></param>
        /// <returns></returns>
        private void HandleTextMessage(string replyToken, string userName, string userMsg)
        {
            // 其他文字訊息處理邏輯
            _bot.ReplyMessage(replyToken, $"[記錄]{userName}說：{userMsg}");
        }

    }
}
