using LineBot.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LineBot.Controllers
{
    public class DriverController : Controller
    {

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Create(Driver driver)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

    }
}
