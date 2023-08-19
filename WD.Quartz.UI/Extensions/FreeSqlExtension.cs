using FreeSql;
using WD.Quartz.UI.Models.BO;

namespace WD.Quartz.UI.Extensions
{
    public static class FreeSqlExtension
    {
        public static void AddFreeSql(this IServiceCollection services, DbOption option)
        {
            services.AddSingleton(x =>
            {
                var fsql = new FreeSqlBuilder()
                   .UseConnectionString(option.DataType, option.ConnectionString)
                   .UseAutoSyncStructure(true)
                   .UseNoneCommandParameter(true)
                   .Build();
                return fsql;
            });
            //services.AddScoped(typeof(IBaseRepository<>), typeof(GuidRepository<>));
            //services.AddScoped(typeof(IBaseRepository<,>), typeof(DefaultRepository<,>));
            //services.AddScoped(typeof(BaseRepository<>), typeof(GuidRepository<>));
            //services.AddScoped(typeof(BaseRepository<,>), typeof(DefaultRepository<,>));
            //services.AddScoped<UnitOfWorkManager>();
        }
    }
}
