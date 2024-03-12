using isRock.LineBot;
using LineBot.Interface;
using LineBot.Repository;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();


// �`�JBot
var channelAccessToken = builder.Configuration.GetConnectionString("channelAccessToken");
builder.Services.AddSingleton<Bot>(provider =>
{
    return new Bot(channelAccessToken);
});

// �`�J���}
var webUrl = builder.Configuration.GetConnectionString("WebUrl");
builder.Services.AddSingleton<string>(provider =>
{
    return webUrl;
});


// �`�JGoogle�A��
builder.Services.AddScoped<IGoogleSheets, GoogleSheetsRepository>();


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
