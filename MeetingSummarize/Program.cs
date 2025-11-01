using Microsoft.EntityFrameworkCore;

namespace MeetingSummarize
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();

            // EF Core
            builder.Services.AddDbContext<Data.AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Services DI
            builder.Services.AddHttpClient<Services.Implementations.MediasoupService>();
            builder.Services.AddHttpClient<Services.Implementations.TranscriptionService>();
            builder.Services.AddHttpClient<Services.Implementations.SummarizationService>();
            builder.Services.AddScoped<Services.Interfaces.IMediasoupService, Services.Implementations.MediasoupService>();
            builder.Services.AddScoped<Services.Interfaces.ITranscriptionService, Services.Implementations.TranscriptionService>();
            builder.Services.AddScoped<Services.Interfaces.ISummarizationService, Services.Implementations.SummarizationService>();
            builder.Services.AddScoped<Services.Interfaces.ICalendarService, Services.Implementations.CalendarService>();

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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Enable attribute-routed API controllers (e.g., /api/transcription/...)
            app.MapControllers();

            app.MapHub<Hubs.MeetingHub>("/hubs/meeting");

            app.Run();
        }
    }
}
