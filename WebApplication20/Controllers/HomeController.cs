using System;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace WebApplication20.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> RunLongTask()
        {
            int maxcount = 200;

            await Task.Run(async () =>
            {
                HttpContext.Application["t_max"] = maxcount; // prefer using azure storage queue to store this
                for (int i = 1; i <= maxcount; i++)
                {
                    await Task.Delay(100);
                    HttpContext.Application["t_prog"] = i; // prefer using azure storage queue to store this
                }
            });

            return Json(new { status = true, ProgressCurrent = maxcount, ProgressMax = maxcount, ProgressPercent = 100 });
        }

        public ActionResult CheckTaskProgress()
        {
            var progressCurrent = HttpContext.Application["t_prog"]; // prefer using azure storage queue to store this
            var progressMax = HttpContext.Application["t_max"]; // prefer using azure storage queue to store this
            decimal progressPercent = (Convert.ToDecimal(progressCurrent) / Convert.ToDecimal(progressMax)) * 100M;

            return Json(new
            {
                ProgressCurrent = progressCurrent,
                ProgressMax = progressMax,
                ProgressPercent = Convert.ToInt32(progressPercent)
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
