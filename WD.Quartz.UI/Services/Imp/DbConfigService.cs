using WD.Quartz.UI.Helpers;
using WD.Quartz.UI.Models.BO;
using WD.Quartz.UI.Models.Output;

namespace WD.Quartz.UI.Services.Imp
{
    public class DbConfigService : IDbConfigService
    {
        private readonly QuartzFileHelper _quartzFileHelper;
        public DbConfigService(QuartzFileHelper quartzFileHelper)
        {
            _quartzFileHelper = quartzFileHelper;
        }

        /// <summary>
        /// 获取数据库配置
        /// </summary>
        /// <returns></returns>
        public Task<RspCommon<DbOption>> Get()
        {
            return Task.Run(() =>
            {
                var db = _quartzFileHelper.GetDbInfo();
                return RspCommon.Success(db);
            });
        }

        /// <summary>
        /// 添加数据库配置
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<RspCommon> Add(DbOption req)
        {
            return Task.Run(() =>
            {
                try
                {
                    _quartzFileHelper.SaveDbInfo(req);
                    return RspCommon.Success("添加数据库配置成功");
                }
                catch (Exception ex)
                {
                    return RspCommon.Fail($"添加数据库配置异常：{ex.Message}");
                }
            });
        }

    }
}
