using DahuaSiteBootstrap.Models;
using DahuaSiteBootstrap.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
//using System.Net;
//using System.Net.Mail;
using MailKit.Net.Smtp;
using MimeKit;
using DahuaSiteBootstrap.Helps;
using DahuaSiteBootstrap.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace DahuaSiteBootstrap.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private DahuaSiteCopyContext _db = new DahuaSiteCopyContext();
        //private FileSupport fs;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var viewfiles = await _db.Dsfiles.ToListAsync();
            return View(viewfiles);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendMail(MailModel mail)
        {
            string msg;

            try
            {
                Support sitesupport = new Support();
                string result = sitesupport.SendEmail(mail);

                // Return proper JSON response
                return Json(new
                {
                    success = true,
                    message = result.Replace("ERR", ""),
                    messageType = "sent"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error",
                    messageType = "error"
                });
            }
        }

        //public IActionResult GetLightboxImage(int fid)
        //{

        //    var file = _db.Dsfiles.Find(fid);
        //    return File(file.Data, file.Content.Split(",")[0]);
        //}

        public async Task<ActionResult> Download(int fid)
        {
            var file = await _db.Dsfiles.FindAsync(fid);

            string[] contentAndDescription = file.Content.Split(",");

            return File(file.Data, contentAndDescription[0],file.Name);
        }
    }
}