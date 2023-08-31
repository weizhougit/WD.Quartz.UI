using WD.Quartz.UI.Helpers;
using WD.Quartz.UI.Models.BO;
using WD.Quartz.UI.Models.Output;

namespace WD.Quartz.UI.Services.Imp
{
    public class UserService : IUserService
    {

        private readonly QuartzFileHelper _quartzFileHelper;
        public UserService(QuartzFileHelper quartzFileHelper)
        {
            _quartzFileHelper = quartzFileHelper;
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<RspCommon> Add(UserOption req)
        {
            return Task.Run(() =>
            {
                try
                {
                    _quartzFileHelper.SaveUserInfo(req);
                    return RspCommon.Success("添加用户成功");
                }
                catch (Exception ex)
                {
                    return RspCommon.Fail($"添加用户异常：{ex.Message}");
                }
            });
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <returns></returns>
        public Task<RspCommon<UserOption>> Get()
        {
            return Task.Run(() =>
            {
                var user = _quartzFileHelper.GetUserInfo();
                return RspCommon.Success(user);
            });
        }
    }
}
