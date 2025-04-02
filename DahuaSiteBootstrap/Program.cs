
using DahuaSiteBootstrap.Helps;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication("CookieAuth")
            .AddCookie("CookieAuth", options =>
            {
                options.LoginPath = "/signin"; // Redirect to login page
            });

//builder.Services.Configure<FormOptions>(options =>
//{
//    options.MultipartBodyLengthLimit = 200_000_000; // 200 MB limit
//    options.ValueLengthLimit = int.MaxValue;
//    options.MultipartHeadersLengthLimit = 300_000_000;
//});

builder.Services.AddControllersWithViews();


var app = builder.Build();

Routes rts = new Routes(app);

// Configure the HTTP request pipeline.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

rts.AddRoutes();
app.Run();
