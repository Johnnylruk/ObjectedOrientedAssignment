using System.Globalization;
using System.Reflection;
using EVotingSystem_SBMM.Data;
using EVotingSystem_SBMM.Helper;
using EVotingSystem_SBMM.Repository;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
namespace EVotingSystem_SBMM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
                builder.Services.AddSingleton<LanguageService>();  
                builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");  
                builder.Services.AddMvc()  
                .AddViewLocalization()  
                .AddDataAnnotationsLocalization(options =>  
                {  
                    options.DataAnnotationLocalizerProvider = (type, factory) =>  
                    {  
                        var assemblyName = new AssemblyName(typeof(ShareResource).GetTypeInfo().Assembly.FullName);  
                        return factory.Create("ShareResource", assemblyName.Name);  
                    };  
                });  
  
                builder.Services.Configure<RequestLocalizationOptions>(  
                options =>  
                {  
                    var supportedCultures = new List<CultureInfo>  
                    {                             
                        new CultureInfo("en-US"),  
                        new CultureInfo("pt-BR")  
                    };  
                    
                    options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");  
                    options.SupportedCultures = supportedCultures;  
                    options.SupportedUICultures = supportedCultures;  
                    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());                     
                });  
                
            builder.Services
                .AddDbContext<EVotingSystemDB>(
                    options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database"))
                );
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential 
                // cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;

                options.MinimumSameSitePolicy = SameSiteMode.None;

            });

            //Injecting Dependencies
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddScoped<IVotersRepository, VotersRepository>();
            builder.Services.AddScoped<IUsersRepository, UsersRepository>();
            builder.Services.AddScoped<IUserSession, UserSession>();
            builder.Services.AddScoped<IEmail, Email>();
            builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
            builder.Services.AddScoped<ILoginRepository, LoginRepository>();
            builder.Services.AddScoped<IVoteRepository, VoteRepository>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<IPasswordHandle, PasswordHandle>();

            //Session Mounting
            builder.Services.AddSession(o =>
            {
                o.Cookie.HttpOnly =  true;
                o.Cookie.IsEssential = true;
            });
            
        var app = builder.Build();
        
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);
            
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            
            //Applying session
            app.UseSession();
            //GDPR
            app.Use(async (context, next) =>
            {
                var culture = context.Request.Query["culture"];
                if (!string.IsNullOrEmpty(culture))
                {
                    context.Response.Cookies.Append(
                        CookieRequestCultureProvider.DefaultCookieName,
                        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
                }

                // Call the next middleware in the pipeline
                await next();
            });
            app.UseCookiePolicy();
            
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id?}");

            app.Run();
        }
    }
}