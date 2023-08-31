using WD.Quartz.UI.Models.BO;
using WD.Quartz.UI.Models.Enums;
using WD.Quartz.UI.Models.Output;

namespace WD.Quartz.UI.Services
{
    public interface IMailService
    {

        /// <summary>
        /// 添加邮箱
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<RspCommon> Add(MailOption req);


        /// <summary>
        /// 获取邮箱
        /// </summary>
        /// <returns></returns>
        Task<RspCommon<MailOption>> Get();


        /// <summary>
        /// 正常消息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="mailMessage"></param>
        /// <returns></returns>
        Task InfoAsync(string title, string msg, MailMessageEnum mailMessage);


        /// <summary>
        /// 异常消息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="mailMessage"></param>
        /// <returns></returns>
        Task ErrorAsync(string title, string msg, MailMessageEnum mailMessage);
    }
}
