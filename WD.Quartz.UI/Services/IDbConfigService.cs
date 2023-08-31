using WD.Quartz.UI.Models.BO;
using WD.Quartz.UI.Models.Output;

namespace WD.Quartz.UI.Services
{
    public interface IDbConfigService
    {
        /// <summary>
        /// 添加数据库配置
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<RspCommon> Add(DbOption req);


        /// <summary>
        /// 获取数据库配置
        /// </summary>
        /// <returns></returns>
        Task<RspCommon<DbOption>> Get();

    }
}
