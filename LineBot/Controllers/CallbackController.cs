using isRock.LineBot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Web;

namespace LineBot.Controllers
{
    public class CallbackController : Controller
    {
        private readonly Bot _bot;
        private readonly HttpContext _context;
        private readonly string _webUrl;


        public CallbackController(Bot bot, IHttpContextAccessor contextAccessor, string webUrl)
        {
            _bot = bot;
            _context = contextAccessor.HttpContext!;
            _webUrl = webUrl;
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
                        HandleButtonEvent(replyToken, userId);
                    }
                    else
                    {
                        // 其他訊息處理
                        HandleTextMessage(replyToken, userName, userMsg);
                    }
                }
            }
            return Ok();
        }


        private void HandleButtonEvent(string replyToken, string userId)
        {
            string queryString = $"?UserId={HttpUtility.UrlEncode(userId)}";

            ButtonsTemplate btnTmpl = new ButtonsTemplate()
            {
                altText = "請用手機檢視",
                text = "點擊網址進行預約",
                title = "長照計程車預約",
                thumbnailImageUrl = new Uri(_webUrl + "/img/OIG2.jpg"),
                actions = new List<TemplateActionBase>() {
                new UriAction() {
                    label = "點我預約",
                    uri = new Uri($"{_webUrl}/LongTermCare/Create{queryString}")
                }
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
