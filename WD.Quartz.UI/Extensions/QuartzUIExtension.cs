using WD.Quartz.UI.Helpers;
using WD.Quartz.UI.Services.Imp;
using WD.Quartz.UI.Services;
using WD.Quartz.UI.Handle.Imp;
using WD.Quartz.UI.Handle;
using Quartz.Spi;
using Quartz.Impl;
using Quartz;
using WD.Quartz.UI.BaseJobs;
using System.Reflection;
using WD.Quartz.UI.Factory;
using WD.Quartz.UI.Middleware;
using WD.Quartz.UI.Models.BO;

namespace WD.Quartz.UI.Extensions;

public static class QuartzUIExtension
{
    public static IServiceCollection AddQuartzUI(this IServiceCollection services, DbOption option = null)
    {
        services.AddRazorPages();
        services.AddHttpClient();
        services.AddHttpContextAccessor();

        if (option != null)
        {
            services.AddFreeSql(option);
            services.AddScoped<IQuartzLogService, DbQuartzLogService>();
            services.AddScoped<IQuartzService, DbQuartzService>();

        }
        else
        {
            services.AddScoped<QuartzFileHelper>();
            services.AddScoped<IQuartzLogService, FileQuartzLogService>();
            services.AddScoped<IQuartzService, FileQuartzService>();

        }
        services.AddScoped<HttpJob>();
        services.AddScoped<DLLJob>();
        services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
        services.AddSingleton<IJobFactory, IOCJobFactory>();
        services.AddScoped<IQuartzHandle, QuartzHandle>();
        return services;
    }

    /// <summary>
    /// 自动注入定时任务类
    /// </summary>
    public static IServiceCollection AddQuartzClass(this IServiceCollection services)
    {
        var baseType = typeof(IJobService);
        var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
        var files = Directory.GetFiles(path, "*.dll");
        List<Type> typeList = new List<Type>();
        foreach (var item in files)
        {
            try
            {
                var assembly = Assembly.LoadFrom(item);
                Type[] type = assembly.GetTypes();
                typeList.AddRange(type.ToList());
            }
            catch (Exception)
            {
                continue;
            }
        }
        var types = typeList.Where(x => x != baseType && baseType.IsAssignableFrom(x)).ToArray();
        var implementTypes = types.Where(x => x.IsClass).ToArray();
        foreach (var implementType in implementTypes)
        {
            var interfaceType = implementType.GetInterfaces().First();
            services.AddScoped(interfaceType, implementType);
            ClassJobFactory.Add(implementType.Name);
        }
        return services;
    }


    public static IApplicationBuilder UseQuartz(this IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseStaticFiles();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
        });
        IServiceProvider services = app.ApplicationServices;
        using (var serviceScope = services.CreateScope())
        {
            var handle = serviceScope.ServiceProvider.GetService<IQuartzHandle>();
            handle.InitJobs().GetAwaiter().GetResult();
        }
        //var service = app.ApplicationServices.GetRequiredService<IQuartzHandle>();
        //service.InitJobs().GetAwaiter().GetResult();
        return app;
    }


    /// <summary>
    /// QuartzUI登录
    /// </summary>
    public static IApplicationBuilder UseQuartzUILogin(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<QuartzUILoginMiddleware>();
    }
}
