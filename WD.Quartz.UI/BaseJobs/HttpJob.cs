using Quartz;
using Quartz.Impl.Triggers;
using Quartz.Impl;
using WD.Quartz.UI.Models.PO;
using WD.Quartz.UI.Services;
using WD.Quartz.UI.Extensions;

namespace WD.Quartz.UI.BaseJobs
{
    public class HttpJob : IJob
    {

        readonly IHttpClientFactory _httpClientFactory;
        readonly IQuartzService _quartzService;
        readonly IQuartzLogService _quartzLogService;
        readonly ILogger<IJob> _logger;

        public HttpJob(IHttpClientFactory httpClientFactory,
            IQuartzService quartzService,
            IQuartzLogService quartzLogService,
            ILogger<IJob> logger)
        {
            _httpClientFactory = httpClientFactory;
            _quartzLogService = quartzLogService;
            _quartzService = quartzService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
           DateTime dateTime = DateTime.Now;
            string httpMessage = "";
            AbstractTrigger trigger = (context as JobExecutionContextImpl).Trigger as AbstractTrigger;
            var jobTask = (await _quartzService.GetJobs(x => x.TaskName == trigger.Name && x.GroupName == trigger.Group)).FirstOrDefault();
            if (jobTask == null)
            {
                jobTask = _quartzService.GetJobs(a => a.TaskName == trigger.JobName && a.GroupName == trigger.JobGroup).Result.FirstOrDefault();
            }
            if (jobTask == null)
            {
                _logger.LogError($"组别：【{trigger.Group}】,名称：【{trigger.Name}】，的作业未找到，可能已被移除");
                return;
            }
            _logger.LogInformation($"组别：【{trigger.Group}】,名称：【{trigger.Name}】，的作业开始执行，时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}");

            var tasklog = new TQuarzTaskLog() { TaskName = jobTask.TaskName, GroupName = jobTask.GroupName, BeginDate = DateTime.Now };
            if (string.IsNullOrEmpty(jobTask.ApiUrl) || jobTask.ApiUrl == "/")
            {
                _logger.LogError($"组别：【{trigger.Group}】,名称：【{trigger.Name}】，参数非法或者异常，时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}");
                return;
            }
            try
            {
                Dictionary<string, string> header = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(jobTask.ApiAuthKey) && !string.IsNullOrEmpty(jobTask.ApiAuthValue))
                {
                    header.Add(jobTask.ApiAuthKey.Trim(), jobTask.ApiAuthValue.Trim());
                }
                var response = await _httpClientFactory.HttpSendAsync(
                        jobTask.ApiRequestType?.ToLower() == "get" ? HttpMethod.Get : HttpMethod.Post,
                        jobTask.ApiUrl,
                        jobTask.ApiParameter,
                        header);

                if (!response.IsOk())
                    httpMessage = $"执行成功，响应消息：{response.Message}";

                httpMessage = $"执行失败，响应消息：{response.Message}";
            }
            catch (Exception ex)
            {
                httpMessage = ex.Message;
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
