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
            using var sr = new StreamReader(_context.Request.Body);
            var reqBody = await sr.ReadToEndAsync();
            var rcvMsg = isRock.LineBot.Utility.Parsing(reqBody);

            foreach (var e in rcvMsg.events)
            {
                if (!string.IsNullOrEmpty(e.message?.text))
                {
                    var replyToken = e.replyToken;
                    var userId = e.source.userId;
                    var userName = _bot.GetUserInfo(userId).displayName;
                    var userMsg = e.message.text;

                    if (userMsg == "/buttons")
                    {
                        // 處理按鈕事件
                        HandleButtonEvent(replyToken, userId);
                        _bot.PushMessage("Ua4a1c1f2d4fd6403641b6e9107269859", "有人發訊息喔");
                    }
                    else if (userMsg.Contains("個案大名"))
                    {
                        HandleReservationEvent(userId, userMsg);
                    }
                    else
                    {
                        // 其他訊息處理
                        await HandleTextMessage(replyToken, userName, userMsg);
                    }
                }
            }


            return Ok();
        }

        private void HandleReservationEvent(string userId, string userMsg)
        {
            try
            {
                _bot.PushMessage(userId, @$"目前已接收到您的預定，相關資料如下
{userMsg}");
            }
            catch (Exception ex)
            {

            }
        }


        private void HandleButtonEvent(string replyToken, string userId)
        {
            var queryString = $"?UserId={HttpUtility.UrlEncode(userId)}";

            var btnTmpl = new ButtonsTemplate()
            {
                altText = "請用手機檢視",
                text = "請選擇",
                title = "敲指令管 Azure，你選哪一種？",
                thumbnailImageUrl = new Uri("https://picsum.photos/420/236"),
                actions = new List<TemplateActionBase>() {
                new UriAction() {
                    label = "看文章",
                    uri = new Uri($"{_webUrl}/LongTermCare/Create{queryString}")
                }
            }
            };

            _bot.ReplyMessage(replyToken, new TemplateMessage(btnTmpl));
        }


        private async Task HandleTextMessage(string replyToken, string userName, string userMsg)
        {
            // 其他文字訊息處理邏輯
            _bot.ReplyMessage(replyToken, $"[記錄]{userName}說：{userMsg}");
        }

    }
}
