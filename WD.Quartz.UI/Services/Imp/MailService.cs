using MimeKit;
using Quartz;
using System.Text.RegularExpressions;
using WD.Quartz.UI.Helpers;
using WD.Quartz.UI.Models.BO;
using WD.Quartz.UI.Models.Enums;
using WD.Quartz.UI.Models.Output;

namespace WD.Quartz.UI.Services.Imp
{
    public class MailService : IMailService
    {

        private readonly QuartzFileHelper _quartzFileHelper;
        private readonly ILogger<MailService> _logger;
        public MailService(QuartzFileHelper quartzFileHelper, ILogger<MailService> logger)
        {
            _quartzFileHelper = quartzFileHelper;
            _logger = logger;
        }

        /// <summary>
        /// 添加邮箱
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Task<RspCommon> Add(MailOption req)
        {

            return Task.Run(() =>
            {
                try
                {
                    _quartzFileHelper.SaveMailInfo(req);
                    return RspCommon.Success("添加邮箱成功");
                }
                catch (Exception ex)
                {
                    return RspCommon.Fail($"添加邮箱异常：{ex.Message}");
                }
            });
        }

        /// <summary>
        /// 获取邮箱
        /// </summary>
        /// <returns></returns>
        public Task<RspCommon<MailOption>> Get()
        {
            return Task.Run(() =>
            {
                var mail = _quartzFileHelper.GetMailInfo();
                return RspCommon.Success(mail);
            });
        }



        /// <summary>
        /// 正常消息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="mailMessage"></param>
        /// <returns></returns>
        public async Task InfoAsync(string title, string msg, MailMessageEnum mailMessage)
        {
            if (mailMessage == MailMessageEnum.正常)
            {
                var res = await SendMail($"任务调度-{title}消息", $"任务调度-{title}：{msg}");
                if (!res.IsOk())
                    _logger.LogError($"任务调度-{title}【正常】消息，发送邮件异常：{msg}");
            }
        }

        /// <summary>
        /// 异常消息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="mailMessage"></param>
        /// <returns></returns>
        public async Task ErrorAsync(string title, string msg, MailMessageEnum mailMessage)
        {
            if (mailMessage == MailMessageEnum.异常 || mailMessage == MailMessageEnum.全部)
            {
                var level = Enum.GetName(typeof(MailMessageEnum), mailMessage);
                var res = await SendMail($"任务调度-{title}消息", $"任务调度-{title}：{msg}");
                if (!res.IsOk())
                    _logger.LogError($"任务调度-{title}【{level}】消息，发送邮件异常：{msg}");
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private async Task<RspCommon> SendMail(string title, string content)
        {
            var mailOption = _quartzFileHelper.GetMailInfo();
            if (mailOption == null)
                return RspCommon.Fail("请先配置邮箱");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(mailOption.SenderName, mailOption.SenderAddress));
            foreach (var item in mailOption.ReciverEmail.Replace("；", ";").Replace("，", ";").Replace(",", ";").Split(';'))
            {
                message.To.Add(new MailboxAddress(item, item));
            }
            message.Subject = string.Format(title);
            var rootPath = new FileInfo(typeof(MailService).Assembly.Location).Directory.FullName;
            var filePath = Path.Combine(rootPath, "wwwroot", "templates", "task_notify.html");
            var fileContent = await File.ReadAllTextAsync(filePath);

            var target = Regex.Replace(fileContent, "{{content}}", content);
            target = Regex.Replace(target, "{{Date}}", DateTime.Now.ToString("yyyy 年 MM 月 dd 日"));
            message.Body = new TextPart("html")
            {
                Text = target
            };
            using var client = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                await client.ConnectAsync(mailOption.Host, mailOption.Port, mailOption.UseSsl);
                await client.AuthenticateAsync(mailOption.SenderAddress, mailOption.Password);
                await client.SendAsync(message);
                return RspCommon.Success();
            }
            catch (Exception ex)
            {
                return RspCommon.Fail($"发送邮件异常：{ex.Message}");
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }

        /// <summary>
        /// 验证邮件
        /// </summary>
        public static bool IsEmail(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            Regex regex = new Regex(@"^[A-Za-z0-9\u4e00-\u9fa5]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$");
            return regex.IsMatch(input);
        }

    }
}
