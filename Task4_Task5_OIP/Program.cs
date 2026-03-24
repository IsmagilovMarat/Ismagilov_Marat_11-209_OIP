using Task5_DEMO_OIP;
using Task5_DEMO_OIP.Services;

namespace Task4_Task5_OIP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string task1OIP = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", ".."));
            string task4OIP = Path.Combine(task1OIP, "Task4OIP", "bin", "Debug", "net8.0");
            string docsDirectory= Path.Combine(task4OIP, "TF_IDF_Леммы");

            builder.Services.AddSingleton<SearchService>(sp => new SearchService(docsDirectory));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
            lifetime.ApplicationStarted.Register(async () =>
            {
                try
                {
                    var searchService = app.Services.GetRequiredService<SearchService>();
                    await searchService.LoadIndexAsync();
                }
                catch 
                {
                }
            });

            app.Run();
        }

    }
}
