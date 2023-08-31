using Quartz;
using Quartz.Impl.Triggers;
using Quartz.Impl;
using WD.Quartz.UI.Services;
using WD.Quartz.UI.Models.PO;
using WD.Quartz.UI.Models.Enums;

namespace WD.Quartz.UI.BaseJobs
{
    public class DLLJob : IJob
    {
        readonly IQuartzService _quartzService;
        readonly ILogger<DLLJob> _logger;
        readonly IQuartzLogService _quartzLogService;
        readonly IServiceProvider _serviceProvider;
        readonly IMailService _mailService;
        public DLLJob(IQuartzService quartzService,
            ILogger<DLLJob> logger,
            IServiceProvider serviceProvider,
            IQuartzLogService quartzLogService,
            IMailService mailService)
        {
            _quartzService = quartzService;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _quartzLogService = quartzLogService;
            _mailService = mailService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            DateTime dateTime = DateTime.Now;
            string httpMessage = "";
            AbstractTrigger trigger = (context as JobExecutionContextImpl).Trigger as AbstractTrigger;
            var jobTask = (await _quartzService.GetJobs(x => x.TaskName == trigger.Name && x.GroupName == trigger.Group)).FirstOrDefault();
            if (jobTask == null)
            {
                jobTask = (await _quartzService.GetJobs(x => x.TaskName == trigger.JobName && x.GroupName == trigger.JobGroup)).FirstOrDefault();
            }
            if (jobTask == null)
            {
                _logger.LogError($"组别：【{trigger.Group}】,名称：【{trigger.Name}】，的作业未找到，可能已被移除");
                return;
            }

            _logger.LogInformation($"组别：【{trigger.Group}】,名称：【{trigger.Name}】，的作业开始执行，时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}");
            var tasklog = new TQuarzTaskLog() { TaskName = jobTask.TaskName, GroupName = jobTask.GroupName, BeginDate = DateTime.Now };
            if (string.IsNullOrEmpty(jobTask.DllClassName))
            {
                _logger.LogError($"组别：【{trigger.Group}】,名称：【{trigger.Name}】，类名不能为空，时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}");
                return;
            }

            try
            {
                var services = _serviceProvider.GetServices<IJobService>();
                var service = services.Where(x => x.GetType().Name == jobTask.DllClassName).FirstOrDefault();
                if (service != null)
                {
                    httpMessage = service.Execute(jobTask.ApiParameter);
                    if (jobTask.MailLevel != MailMessageEnum.全部.GetHashCode())
                    {
                        await _mailService.InfoAsync(jobTask.TaskName, httpMessage, (MailMessageEnum)jobTask.MailLevel);
                    }
                }
                else
                {
                    httpMessage = "未找到对应类型，请检查是否注入";
                }
            }
            catch (Exception ex)
            {
                httpMessage = ex.Message;
                if (jobTask.MailLevel != MailMessageEnum.全部.GetHashCode())
                {
                    await _mailService.ErrorAsync(jobTask.TaskName, httpMessage, (MailMessageEnum)jobTask.MailLevel);
                }
            }
            finally
            {
                if (jobTask.MailLevel == MailMessageEnum.全部.GetHashCode())
                {
                    await _mailService.ErrorAsync(jobTask.TaskName, httpMessage, (MailMessageEnum)jobTask.MailLevel);
                }
            }
            try
            {
                tasklog.EndDate = DateTime.Now;
                tasklog.Msg = httpMessage;
                await _quartzLogService.AddLog(tasklog);
            }
            catch (Exception)
            {
            }
            return;
        }
    }
}
