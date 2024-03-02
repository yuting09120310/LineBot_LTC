using isRock.LineBot;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();


// ª`¤JBot
var channelAccessToken = builder.Configuration.GetConnectionString("channelAccessToken");
builder.Services.AddSingleton<Bot>(provider =>
{
    return new Bot(channelAccessToken);
});

// ª`¤Jºô§}
var webUrl = builder.Configuration.GetConnectionString("WebUrl");
builder.Services.AddSingleton<string>(provider =>
{
    return webUrl;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
