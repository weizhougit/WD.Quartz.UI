using WD.Quartz.UI.Models.BO;
using WD.Quartz.UI.Models.Output;

namespace WD.Quartz.UI.Services
{
    public interface IUserService
    {
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<RspCommon> Add(UserOption req);


        /// <summary>
        /// 获取用户
        /// </summary>
        /// <returns></returns>
        Task<RspCommon<UserOption>> Get();
    }
}
