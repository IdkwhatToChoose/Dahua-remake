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
using Microsoft.VisualBasic;

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

        //public async Task<IActionResult> CrtAdmin(AdminViewModel body)
        //{
        //    Admin admin = new Admin()
        //    {
        //        AdminName = body.AdminName,
        //        Password = BCrypt.Net.BCrypt.HashPassword(body.Password),
        //    };

        //    await _db.Admins.AddAsync(admin);
        //    await _db.SaveChangesAsync();

        //    return Content($"Admin {admin.AdminName} saved!");
        //}


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Signin(AdminViewModel avm)
        {
            Admin? admin = await _db.Admins.FirstOrDefaultAsync(x => x.AdminName == avm.AdminName);
            if (admin == null)
            {
                return RedirectToRoute("authentication");
            }
           
            bool valid_password=BCrypt.Net.BCrypt.Verify(avm.Password,admin.Password);

            if(valid_password)
            {
                string role = admin.Type == "nrm" ? "Admin" : "Owner";
                string initial = admin.AdminName.First().
                                 ToString().
                                 ToUpper();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,admin.Id.ToString()),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.Upn,initial)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.Now.AddMinutes(30),
                    
                };

                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToRoute($"{role.ToLower()}page");
            }
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



