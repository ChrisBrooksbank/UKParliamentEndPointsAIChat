using UKParliamentEndPointsAIChat.Ui.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<Config>(config =>
{
    builder.Configuration.GetSection("Config").Bind(config);
    config.CoachAndFocusLLMApiKey = Environment.GetEnvironmentVariable("CoachAndFocusLLMApiKey");
    config.CoachAndFocusLLMEndpoint = Environment.GetEnvironmentVariable("CoachAndFocusLLMEndpoint");
    config.CoachAndFocusLLMApiKey2 = Environment.GetEnvironmentVariable("CoachAndFocusLLMApiKey2");
    config.CoachAndFocusLLMEndpoint2 = Environment.GetEnvironmentVariable("CoachAndFocusLLMEndpoint2");
});

var app = builder.Build();

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

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
