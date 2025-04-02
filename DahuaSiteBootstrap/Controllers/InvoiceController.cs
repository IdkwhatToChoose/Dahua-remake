//using Microsoft.AspNetCore.Mvc;
//using DahuaSiteBootstrap.Model;
//using DahuaSiteBootstrap.ViewModels;
//using Microsoft.AspNetCore.Authorization;
//using DahuaSiteBootstrap.Helps;
//using DahuaSiteBootstrap.wwwroot.ViewModel;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Mvc.Rendering;

//namespace DahuaSiteBootstrap.Controllers
//{
//    public class InvoiceController : Controller
//    {
//        private DahuaSiteContext db = new DahuaSiteContext();
//        private readonly IClientMapper clMapper;
//        //public string pass = "";
//        public InvoiceController(IClientMapper _clMapper)
//        {
//            clMapper = _clMapper;
//        }
//        [HttpGet]
//        public IActionResult AddInvoice()
//        {
//            var model = new IACIViewModel
//            {
//                ClientList = GetClientNames() // Fetch list of client names from DB
//            };
//            return View("Views/Admin/ICOS/AddInvoice.cshtml",model);
//        }
//        [HttpGet]
//        [ActionName("AddInvoiceByName")]
//        public IActionResult AddInvoiceByName(string SelectedClientName)
//        {
//            var model = new IACIViewModel
//            {
//                ClientList = GetClientNames() // Populate the dropdown list
//            };

//            if (!string.IsNullOrEmpty(SelectedClientName))
//            {
//                var invoiceAndClientInfo = db.InvoiceAndClientInfos
//                                             .FirstOrDefault(x => x.RecieverName == SelectedClientName);

//                if (invoiceAndClientInfo != null)
//                {
//                    model.RecieverName = invoiceAndClientInfo.RecieverName;
//                    model.RecieverCity = invoiceAndClientInfo.RecieverCity;
//                    model.RecieverEik = invoiceAndClientInfo.RecieverEik;
//                    model.RecieverAddress = invoiceAndClientInfo.RecieverAddress;
//                    model.RintroughDds = invoiceAndClientInfo.RintroughDds;
//                    model.SelectedClientName = SelectedClientName; // Preserve the selected client name
//                }
//            }

//            return View("Views/Admin/ICOS/AddInvoice.cshtml", model);
//        }


//        public IActionResult AddClient()
//        {
//            return View("Views/Admin/ICOS/AddClient.cshtml");
//        }
//        [Authorize]
//        [HttpPost]
//        public IActionResult AddClient(InvoiceAndClientInfo iaci)
//        {
//            InvoiceAndClientInfo invoiceAndClientInfo = new InvoiceAndClientInfo();

//            invoiceAndClientInfo.RecieverName = iaci.RecieverName;
//            invoiceAndClientInfo.RecieverAddress = iaci.RecieverAddress;
//            invoiceAndClientInfo.RecieverEik = iaci.RecieverEik;
//            invoiceAndClientInfo.RecieverCity = iaci.RecieverCity;
//            invoiceAndClientInfo.RintroughDds = iaci.RintroughDds;
            
//            db.InvoiceAndClientInfos.Add(invoiceAndClientInfo);
//            db.SaveChanges();
//            return RedirectToAction("Invoice", "AdminICO");
//        }
//        public ActionResult ShowClients() {

//            List<InvoiceAndClientInfo> obekts = db.InvoiceAndClientInfos.ToList();
//            List<IACIViewModel> model = clMapper.ListClientToListEstateVm(obekts);

//            return View("Views/Admin/ICOS/ShowClients.cshtml",model);

//        }
//        [HttpGet]
//        public IActionResult EditClient(int obektID)
//        {
//            IACIViewModel obektViewModel = new IACIViewModel();
//            InvoiceAndClientInfo? estate = db.InvoiceAndClientInfos.FirstOrDefault(x => x.Id == obektID);

//            if (estate != null)
//            {
//                obektViewModel = clMapper.ClientICOToClientVM(estate);
//            }

//            return View("Views/Admin/ICOS/EditClient.cshtml",obektViewModel);
//        }
//        [Authorize]
//        [HttpPost]
//        public ActionResult EditClient(IACIViewModel iaci)
//        {
//            InvoiceAndClientInfo? estate = db.InvoiceAndClientInfos.FirstOrDefault(x => x.Id == iaci.Id);

//            if (estate != null)
//            {
//                estate.RecieverName = iaci.RecieverName;
//               estate.RecieverEik = iaci.RecieverEik;
//                estate.RecieverCity = iaci.RecieverCity;
//                estate.RintroughDds = iaci.RintroughDds;
//                estate.RecieverAddress = iaci.RecieverAddress;
//                db.SaveChanges();
//            }
            
//            return RedirectToAction("ShowClients", "Invoice");
//        }
//        [Authorize]
//        [HttpGet]
//        public IActionResult DeleteClient(string obektName)
//        {
//            InvoiceAndClientInfo? obekt = db.InvoiceAndClientInfos.FirstOrDefault(x => x.RecieverName == obektName);
//            if (obekt != null)
//            {

//                db.InvoiceAndClientInfos.Remove(obekt);
                
//                try
//                {
//                    db.SaveChanges();
//                }
//                catch (Exception ex)
//                {

//                }

//            }
//            return RedirectToAction("ShowClients", "Invoice");
//        }

//        private List<SelectListItem> GetClientNames()
//        {
//            List<SelectListItem> clientList = db.InvoiceAndClientInfos.AsNoTracking().Select(c => new SelectListItem
//             {
//                Text = c.RecieverName,
//                Value = c.RecieverName 

//             }).ToList();
//            return clientList;
//        }

//        private InvoiceAndClientInfo GetClientDetails(string clientName)
//        {
//            return db.InvoiceAndClientInfos.FirstOrDefault(c => c.RecieverName == clientName);
//        }

//    }
//}
