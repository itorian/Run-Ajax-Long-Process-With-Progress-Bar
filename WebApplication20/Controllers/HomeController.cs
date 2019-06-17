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
            string id = "task_id";  //should be unique for every task
            int maxcount = 200;

            AsyncManager.OutstandingOperations.Increment();
            await Task.Factory.StartNew(taskId =>
            {
                HttpContext.Application["t_max_" + taskId] = maxcount;
                for (int i = 0; i < maxcount; i++)
                {
                    //Thread.Sleep(200);
                    HttpContext.Application["t_prog_" + taskId] = i;
                }
                AsyncManager.OutstandingOperations.Decrement();
            }, id);

            return Json(new { status = true, ProgressCurrent = maxcount, ProgressMax = maxcount, ProgressPercent = 100 });
        }

        public ActionResult SomeTaskProgress()
        {
            string id = "task_id";  //should be unique for every task

            var progressCurrent = HttpContext.Application["t_prog_" + id];
            var progressMax = HttpContext.Application["t_max_" + id];
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