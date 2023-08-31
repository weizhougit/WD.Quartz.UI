using WD.Quartz.UI.Extensions;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
//builder.Services.AddQuartzUI(config.GetSection("QuartzUI:Db").Get<DbOption>());
builder.Services.AddQuartzUI();
builder.Services.AddQuartzClass();

var app = builder.Build();
app.UseQuartzUILogin();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Hello World!1212121");
    });
});
app.UseQuartz(); //添加这行代码
app.Run();
