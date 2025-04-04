using DahuaSiteBootstrap.Helps;
using DahuaSiteBootstrap.Model;
using DahuaSiteBootstrap.ViewModels;
using DahuaSiteBootstrap.wwwroot.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Org.BouncyCastle.Crypto.Generators;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Data;
using System.Text;
using System.Text.Encodings.Web;

namespace DahuaSiteBootstrap.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private DahuaSiteCopyContext _db = new DahuaSiteCopyContext();
      

        [AllowAnonymous]
        public IActionResult Signin()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult TwoFactorAuth()
        {
            if(HttpContext.Session.GetString("2faCode") == null)
            {
                return RedirectToRoute("authentication");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyAuthCode([FromForm] string enteredCode) {

            Support security = new Support();

            string storedCode = HttpContext.Session.GetInt32("2faCode").ToString();
            var aid = HttpContext.Session.GetInt32("aid");

            if(string.IsNullOrEmpty(storedCode) || aid == null)
            {
                TempData["Message"] = "Сесията е изтекла. Моля влезте отново във профила си";
                return RedirectToRoute("authenticate");
            }

            if(storedCode == enteredCode)
            {
                Admin a = await _db.Admins.FindAsync(aid);

                HttpContext.Session.Remove("2faCode");
                HttpContext.Session.Remove("aid");

                await security.Authenticate(a.AdminName, "Owner", a.Id, HttpContext);
                return RedirectToRoute("ownerpage");

              
            }
            TempData["Message"] = "Грешен код за верификация. Опитайте отново или регенерирайте кода си.";
            return RedirectToAction("2fa");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Signin(AdminViewModel avm)
        {
            Support security = new Support();

            Admin? admin = await _db.Admins.FirstOrDefaultAsync(x => x.AdminName == avm.AdminName);
            if (admin == null)
            {
                TempData["Message"] = "Работникът не е намерен. Опитайте с друго име";
                return RedirectToRoute("authentication");
            }
           
            bool valid_password=BCrypt.Net.BCrypt.Verify(avm.Password,admin.Password);

            if(valid_password)
            {
                string role = admin.Type == "nrm" ? "Admin" : "Owner";
                bool o = admin.Type != "nrm";

                if (!o) { security.Notify(admin.AdminName,true); }
                else { 

                    var (message,code) = security.ConfirmIdentity(admin.AdminName, admin.Email);
                    HttpContext.Session.SetInt32("2faCode", code);
                    HttpContext.Session.SetInt32("aid", admin.Id);

                    TempData["2FA-message"] = message;
                    return RedirectToRoute("2fa");
                }

                await security.Authenticate(admin.AdminName, role, admin.Id, HttpContext);

                return RedirectToRoute($"{role.ToLower()}page");
            }
            security.Notify(admin.AdminName, false);
            TempData["Message"] = "Грешно име или парола. Моля опитайте отново";
            return RedirectToRoute("authentication");
        }
        
        public IActionResult AdminEntry()
        {
            return View();
        }
        public async Task<IActionResult> Ownerpage()
        {
            OwnerData data = new OwnerData();
            data.files = await _db.Dsfiles.ToListAsync();

            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> Adminpage()
        {
            ViewBag.taskUpdate = TempData["tid_update"];

            int aid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ICollection<Dstask> admin_tasks = await _db.Dstasks.Where(t=> t.AdminId == aid).ToListAsync();

            return View(admin_tasks);
        }

        [HttpPost]
        public async Task<IActionResult> Addtask(TaskVM body)
        {
            int aid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Admin admin = await _db.Admins.FindAsync(aid);

            Dstask task = new Dstask()
            {
                Name = body.Name,
                AdminId = admin.Id,
                Description = body.Description,
                Phone = body.Phone,
                Admin = admin
                
            }; // parsing body
            admin.Dstasks.Add(task);

            await _db.Dstasks.AddAsync(task);
            await _db.SaveChangesAsync();

            return RedirectToRoute("adminpage");
        }


        [HttpPost]
        public async Task<IActionResult> Removetask(int tid)
        {
            Dstask task = await _db.Dstasks.FindAsync(tid);

            _db.Dstasks.Remove(task);
            await _db.SaveChangesAsync();

            
            return RedirectToRoute("adminpage");
        }

        [HttpGet]
        public async Task<Dstask> GetTask(int? tid) => await _db.Dstasks.FindAsync(tid);

        //[HttpGet]
        //public async Task<IActionResult> ShowUpdateDlg(int tid)
        //{
        //    TempData["tid_update"] = tid;
        //    return RedirectToRoute("adminpage");
        //}

      
        [HttpPost]
        public async Task<IActionResult> Uploadfile(FileBody body)
        {
            try
            {
                Dsfile file = new Dsfile()
                {
                    
                    Data = await FileSupport.ToBytes(body.Data),
                    DisplayName = body.DisplayName,
                    Name = body.Data.FileName,
                    Category = body.Category.Replace(" ", "_"),
                    Content = $"{body.Data.ContentType},{body.Description}",
                };

                await _db.Dsfiles.AddAsync(file);
                await _db.SaveChangesAsync();

               
            }catch (Exception ex) { 
               return Content("Error: " + ex);
            }
            

            return RedirectToRoute("ownerpage");

        }

        public async Task<IActionResult> Showupdatedialog(int id)
        {
            Dstask task = await _db.Dstasks.FindAsync(id);
            return PartialView("Components/_UpdateTask",task);
        }

        public async Task<IActionResult> Removefile(int fid)
        {
            Dsfile file = await _db.Dsfiles.FindAsync(fid);
            _db.Dsfiles.Remove(file);

            await _db.SaveChangesAsync();

            return RedirectToRoute("ownerpage");
        }

        public async Task<IActionResult> Updatetask(Dstask body)
        {
            int aid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            body.AdminId = aid;

            _db.Update(body);
            await _db.SaveChangesAsync();

            return RedirectToRoute("adminpage");


        }

    }

    [ViewComponent]
    public class UpdateTaskViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Dstask task)
        {
            return View(task); // Pass the task model to the view
        }
    }
}



