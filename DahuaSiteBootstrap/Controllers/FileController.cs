using DahuaSiteBootstrap.Model;
using DahuaSiteBootstrap.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DahuaSiteBootstrap.Controllers
{
    public class FileController : Controller
    {
        private DahuaSiteCopyContext _db = new DahuaSiteCopyContext();
        
        public IActionResult GetLightboxImage(int fid)
        {
            var file = _db.Dsfiles.Find(fid);
       
            ViewBag.ext = Path.GetExtension(file.Name);
            string contentType = file.Content.Split(",")[0];
            return File(file.Data, contentType ?? "image/jpg");
        }
    }
}
