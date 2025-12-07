using ElectronNET.API;

namespace ElectronNET.Samples.ElectronHostHook
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseElectron(args);
            builder.Services.AddElectron();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            app.UseStaticFiles();
            app.UseRouting();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            if (HybridSupport.IsElectronActive)
            {
                Task.Run(async () =>
                {
                    var window = await Electron.WindowManager.CreateWindowAsync();
                });
            }

            app.Run();
        }
    }
}
