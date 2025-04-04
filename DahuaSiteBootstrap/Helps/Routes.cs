using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;


namespace DahuaSiteBootstrap.Helps
{
    public class Routes
    {
        public WebApplication _app { get; set; }

        public Routes(WebApplication app) { 
        
         _app = app;

        }

        public void AddRoutes()
        {
            _app.MapControllerRoute(
                name: "authentication",
                pattern: "signin",
                defaults: new
                {
                    controller = "Admin",
                    action = "Signin"
                }
               
            );
            _app.MapControllerRoute(
                name: "2fa",
                pattern: "verificationcode",
                defaults: new
                {
                    controller = "Admin",
                    action = "TwoFactorAuth"
                }

            );
            _app.MapControllerRoute(
                name:"adminpage",
                pattern:"admin/adminpage",
                defaults: new
                {
                    controller = "Admin",
                    action = "Adminpage"
                }
            );
            _app.MapControllerRoute(
               name: "ownerpage",
               pattern: "admin/sitedata",
               defaults: new
               {
                   controller = "Admin",
                   action = "Ownerpage"
               }
           );
            _app.MapControllerRoute(
               name: "updt",
               pattern: "admin/update/{id?}",
               defaults: new
               {
                   controller = "Admin",
                   action = "Showupdatedialog"

               }
           );
            

        }
    }
}
