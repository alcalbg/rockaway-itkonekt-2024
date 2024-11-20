using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Rockaway.WebApp.Data;
using Rockaway.WebApp.Services;
using Rockaway.WebApp.Components;
using Mjml.Net;
using RazorEngineCore;
using Rockaway.WebApp.Services.Mail;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options => options.Conventions.AuthorizeAreaFolder("admin", "/"));
builder.Services.AddControllersWithViews(options => {
	options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});
builder.Services.AddSingleton<IClock>(SystemClock.Instance);

builder.Services.AddRazorPages();

var logger = CreateAdHocLogger<Program>();
logger.LogInformation("Rockaway running in {environment} environment", builder.Environment.EnvironmentName);
logger.LogInformation("Using Sqlite database");
var sqliteConnection = new SqliteConnection("Data Source=:memory:");
sqliteConnection.Open();
builder.Services.AddDbContext<RockawayDbContext>(options => options.UseSqlite(sqliteConnection));
builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<RockawayDbContext>();
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddTransient<IStatusReporter, StatusReporter>();

#if DEBUG
builder.Services.AddSassCompiler();
#endif


builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();


builder.Services.AddSingleton<IMailTemplateProvider>(new ResourceMailTemplateProvider());
builder.Services.AddSingleton<IMailBodyRenderer, MailBodyRenderer>();
builder.Services.AddSingleton<IRazorEngine, RazorEngine>();
builder.Services.AddSingleton<IMjmlRenderer, MjmlRenderer>();

builder.Services.AddSingleton<IMailSender, SmtpMailSender>();
var smtpSettings = new SmtpSettings();
builder.Configuration.Bind("Smtp", smtpSettings);
builder.Services.AddSingleton(smtpSettings);
builder.Services.AddSingleton<ISmtpRelay, SmtpRelay>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

using (var scope = app.Services.CreateScope()) {
	using var db = scope.ServiceProvider.GetService<RockawayDbContext>()!;
	db.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapAreaControllerRoute(
	name: "admin",
	areaName: "Admin",
	pattern: "Admin/{controller=Home}/{action=Index}/{id?}"
).RequireAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

app.MapGet("/status", (IStatusReporter reporter) => reporter.GetStatus());

app.MapRazorComponents<TicketPicker>()
	.AddInteractiveServerRenderMode();


app.Run();
ILogger<T> CreateAdHocLogger<T>() => LoggerFactory.Create(lb => lb.AddConsole()).CreateLogger<T>();