using Quartz.Impl.Matchers;
using Quartz;
using Quartz.Impl.Triggers;
using Quartz.Spi;
using WD.Quartz.UI.Services;
using WD.Quartz.UI.Models.PO;
using WD.Quartz.UI.BaseJobs;
using WD.Quartz.UI.Models.Input;
using WD.Quartz.UI.Models.Output;
using WD.Quartz.UI.Models.Enums;

namespace WD.Quartz.UI.Handle.Imp
{
    public class QuartzHandle : IQuartzHandle
    {
        readonly IQuartzService _quartzService;
        readonly ISchedulerFactory _schedulerFactory;
        readonly IQuartzLogService _quartzLogService;
        readonly IJobFactory _jobFactory;
        readonly ILogger<QuartzHandle> _logger;
        public QuartzHandle(ISchedulerFactory schedulerFactory,
            IQuartzService quartzService,
            IQuartzLogService quartzLogService,
            IJobFactory jobFactory,
            ILogger<QuartzHandle> logger)
        {
            _schedulerFactory = schedulerFactory;
            _quartzService = quartzService;
            _quartzLogService = quartzLogService;
            _jobFactory = jobFactory;
            _logger = logger;
        }

        /// <summary>
        /// 获取所有的作业
        /// </summary>
        public async Task<RspPageData<TQuarzTask>> GetJobs(ReqTaskPageQuery req)
        {
            var list = new List<TQuarzTask>();
            var recordCount = 0;
            try
            {
                IScheduler _scheduler = await _schedulerFactory.GetScheduler();
                var groups = await _scheduler.GetJobGroupNames();
                var tasks = await _quartzService.GetJobs(x => 1 == 1);
                recordCount = tasks.Count;
                list = tasks.Skip(req.PageSize * (req.PageNumber - 1)).Take(req.PageSize).ToList();
                foreach (var groupName in groups)
                {
                    foreach (var jobKey in await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName)))
                    {
                        TQuarzTask jobTask = list.Where(x => x.GroupName == jobKey.Group && x.TaskName == jobKey.Name).FirstOrDefault();
                        if (jobTask == null)
                            continue;

                        var triggers = await _scheduler.GetTriggersOfJob(jobKey);
                        foreach (ITrigger trigger in triggers)
                        {
                            DateTimeOffset? dateTimeOffset = trigger.GetPreviousFireTimeUtc();
                            if (dateTimeOffset != null)
                            {
                                jobTask.LastRunTime = Convert.ToDateTime(dateTimeOffset.ToString());
                            }
                            else
                            {
                                var runlog = await _quartzLogService.GetLastlog(new ReqLogPageQuery
                                {
                                    TaskName = jobTask.TaskName,
                                    GroupName = jobTask.GroupName,
                                });
                                if (runlog != null)
                                {
                                    jobTask.LastRunTime = runlog.Data.BeginDate;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogWarning("获取作业异常：" + ex.Message + ex.StackTrace);
                }
            }
            return new RspPageData<TQuarzTask> { Data = list, Total = recordCount };
        }

        /// <summary>
        /// 添加
        /// </summary>
        public async Task<RspCommon> Add(TQuarzTask req)
        {
            try
            {
                var validExpression = IsValidExpression(req.Interval);
                if (!validExpression.IsOk())
                    return validExpression;

                var jobTask = _quartzService.GetJobs(x => x.TaskName == req.TaskName && x.GroupName == req.GroupName).Result.FirstOrDefault();
                if (jobTask != null)
                    return RspCommon.Fail("任务已存在,添加失败");

                await _quartzService.AddJob(req);
                IJobDetail job = null;
                if (req.TaskType == (int)TaskTypeEnum.DLL)
                {
                    job = JobBuilder.Create<DLLJob>()
                    .WithIdentity(req.TaskName, req.GroupName)
                    .Build();
                }
                else
                {
                    job = JobBuilder.Create<HttpJob>()
                    .WithIdentity(req.TaskName, req.GroupName)
                    .Build();
                }
                ITrigger trigger = TriggerBuilder.Create()
                   .WithIdentity(req.TaskName, req.GroupName)
                   .WithDescription(req.Describe)
                   .WithCronSchedule(req.Interval, x => x.WithMisfireHandlingInstructionDoNothing())
                   .Build();
                IScheduler scheduler = await _schedulerFactory.GetScheduler();
                if (_jobFactory != null)
                    scheduler.JobFactory = _jobFactory;

                await scheduler.ScheduleJob(job, trigger);
                if (req.Status == (int)JobStateEnum.开启)
                {              
                    await scheduler.Start();
                }
                else
                {
                    await scheduler.PauseJob(job.Key);
                    //await Pause(req);
                }
                var runlog = new TQuarzTaskLog()
                {
                    TaskName = req.TaskName,
                    GroupName = req.GroupName,
                    BeginDate = DateTime.Now,
                    Msg = $"任务新建未启动，状态为：【{Enum.GetName(typeof(JobStateEnum), req.Status)}】"
                };
                await _quartzLogService.AddLog(runlog);
                return RspCommon.Success("任务添加成功");
            }
            catch (Exception ex)
            {
                return RspCommon.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public async Task InitJobs()
        {
            var jobs = await _quartzService.GetJobs(x => 1 == 1);
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            foreach (var item in jobs)
            {
                try
                {
                    IJobDetail job = null;
                    if (item.TaskType == (int)TaskTypeEnum.DLL)
                    {
                        job = JobBuilder.Create<DLLJob>()
                        .WithIdentity(item.TaskName, item.GroupName)
                        .Build();
                    }
                    else
                    {
                        job = JobBuilder.Create<HttpJob>()
                        .WithIdentity(item.TaskName, item.GroupName)
                        .Build();
                    }
                    ITrigger trigger = TriggerBuilder.Create()
                       .WithIdentity(item.TaskName, item.GroupName)
                       .WithDescription(item.Describe)
                       .WithCronSchedule(item.Interval, x => x.WithMisfireHandlingInstructionDoNothing())
                       .Build();
                    if (_jobFactory != null)
                        scheduler.JobFactory = _jobFactory;

                    if (item.Status == (int)JobStateEnum.开启)
                    {
                        await scheduler.ScheduleJob(job, trigger);
                        var runlog = new TQuarzTaskLog()
                        {
                            TaskName = item.TaskName,
                            GroupName = item.GroupName,
                            BeginDate = DateTime.Now,
                            Msg = $"任务初始化启动成功，状态为：【{Enum.GetName(typeof(JobStateEnum), item.Status)}】"
                        };
                        await _quartzLogService.AddLog(runlog);
                    }
                    else
                    {
                        await scheduler.ScheduleJob(job, trigger);
                        await Pause(item);
                        _logger.LogInformation($"任务初始化未启动，状态为：【{Enum.GetName(typeof(JobStateEnum), item.Status)}】");
                    }
                }
                catch (Exception ex)
                {
                    var runlog = new TQuarzTaskLog()
                    {
                        TaskName = item.TaskName,
                        GroupName = item.GroupName,
                        BeginDate = DateTime.Now,
                        Msg = $"任务初始化出现异常，异常信息：{ex.Message}"
                    };
                    await _quartzLogService.AddLog(runlog);
                    continue;
                }
                await scheduler.Start();
            }
        }


        /// <summary>
        /// 暂停
        /// </summary>
        public async Task<RspCommon> Pause(TQuarzTask req)
        {
            try
            {
                var job = await IsQuartzJob(req.TaskName, req.GroupName);
                var jobTask = (await _quartzService.GetJobs(x => x.TaskName == req.TaskName && x.GroupName == req.GroupName)).FirstOrDefault();
                if (job.IsOk())
                {
                    IScheduler scheduler = await _schedulerFactory.GetScheduler();
                    List<JobKey> jobKeys = scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(req.GroupName)).Result.ToList();
                    JobKey jobKey = jobKeys.Where(x => scheduler.GetTriggersOfJob(x).Result.Any(x => (x as CronTriggerImpl).Name == req.TaskName)).FirstOrDefault();
                    var triggers = await scheduler.GetTriggersOfJob(jobKey);
                    ITrigger trigger = triggers?.Where(x => (x as CronTriggerImpl).Name == req.TaskName).FirstOrDefault();
                    await scheduler.PauseTrigger(trigger.Key);
                }
                if (jobTask != null)
                {
                    jobTask.Status = (int)JobStateEnum.暂停;
                    await _quartzService.Update(jobTask);
                    var runlog = new TQuarzTaskLog()
                    {
                        TaskName = req.TaskName,
                        GroupName = req.GroupName,
                        BeginDate = DateTime.Now,
                        Msg = $"任务状态为：【{Enum.GetName(typeof(JobStateEnum), jobTask.Status)}】"
                    };
                    await _quartzLogService.AddLog(runlog);
                }
                return RspCommon.Success("任务暂停成功");
            }
            catch (Exception ex)
            {
                return RspCommon.Fail(ex.Message);
            }

        }

        /// <summary>
        /// 更新
        /// </summary>
        public async Task<RspCommon> Update(TQuarzTask req)
        {
            var quartzJob = await IsQuartzJob(req.TaskName, req.GroupName);
            var jobTask = (await _quartzService.GetJobs(x => x.TaskName == req.TaskName && x.GroupName == req.GroupName)).FirstOrDefault();
            try
            {
                if (quartzJob.IsOk()) //如果Quartz存在就更新
                {
                    IScheduler scheduler = await _schedulerFactory.GetScheduler();
                    List<JobKey> jobKeys = scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(req.GroupName)).Result.ToList();
                    JobKey jobKey = jobKeys.Where(x => scheduler.GetTriggersOfJob(x).Result.Any(x => (x as CronTriggerImpl).Name == req.TaskName)).FirstOrDefault();
                    var triggers = await scheduler.GetTriggersOfJob(jobKey);
                    ITrigger triggerold = triggers?.Where(x => (x as CronTriggerImpl).Name == req.TaskName).FirstOrDefault();
                    await scheduler.PauseTrigger(triggerold.Key);
                    await scheduler.UnscheduleJob(triggerold.Key);// 移除触发器
                    await scheduler.DeleteJob(triggerold.JobKey);
                    IJobDetail job = null;
                    if (req.TaskType == (int)TaskTypeEnum.DLL)
                    {
                        job = JobBuilder.Create<DLLJob>()
                        .WithIdentity(req.TaskName, req.GroupName)
                        .Build();
                    }
                    else
                    {
                        job = JobBuilder.Create<HttpJob>()
                        .WithIdentity(req.TaskName, req.GroupName)
                        .Build();
                    }
                    ITrigger triggernew = TriggerBuilder.Create()
                       .WithIdentity(req.TaskName, req.GroupName)
                       .StartNow()
                       .WithDescription(req.Describe)
                       .WithCronSchedule(req.Interval, x => x.WithMisfireHandlingInstructionDoNothing())
                       .Build();

                    if (_jobFactory != null)
                        scheduler.JobFactory = _jobFactory;

                    await scheduler.ScheduleJob(job, triggernew);
                    if (req.Status == (int)JobStateEnum.开启)
                    {
                        await scheduler.Start();
                    }
                    else
                    {
                        await scheduler.PauseTrigger(triggernew.Key);
                        var runlog = new TQuarzTaskLog()
                        {
                            TaskName = req.TaskName,
                            GroupName = req.GroupName,
                            BeginDate = DateTime.Now,
                            Msg = $"更新任务成功，状态为：【{Enum.GetName(typeof(JobStateEnum), req.Status)}】"
                        };
                        await _quartzLogService.AddLog(runlog);
                    }
                }

                if (jobTask != null)
                    await _quartzService.Update(req);

                return RspCommon.Success("更新任务成功");
            }
            catch (Exception ex)
            {
                return RspCommon.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        public async Task<RspCommon> Start(TQuarzTask req)
        {
            try
            {
                var quartzJob = await IsQuartzJob(req.TaskName, req.GroupName);
                var jobTask = (await _quartzService.GetJobs(x => x.TaskName == req.TaskName && x.GroupName == req.GroupName)).FirstOrDefault();
                jobTask.Status = (int)JobStateEnum.开启;
                IScheduler scheduler = await _schedulerFactory.GetScheduler();
                if (!quartzJob.IsOk()) //如果不存在则加入
                {
                    IJobDetail job = null;
                    if (req.TaskType == (int)TaskTypeEnum.DLL)
                    {
                        job = JobBuilder.Create<DLLJob>()
                        .WithIdentity(req.TaskName, req.GroupName)
                        .Build();
                    }
                    else
                    {
                        job = JobBuilder.Create<HttpJob>()
                        .WithIdentity(req.TaskName, req.GroupName)
                        .Build();
                    }
                    ITrigger trigger = TriggerBuilder.Create()
                       .WithIdentity(req.TaskName, req.GroupName)
                       .WithDescription(req.Describe)
                       .WithCronSchedule(req.Interval, x => x.WithMisfireHandlingInstructionDoNothing())
                       .Build();
                    if (_jobFactory != null)
                        scheduler.JobFactory = _jobFactory;

                    await scheduler.ScheduleJob(job, trigger);
                    await scheduler.Start();
                }
                else //存在则直接启动
                {
                    var jobKeys = scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(req.GroupName)).Result.ToList();
                    JobKey jobKey = jobKeys.Where(x => scheduler.GetTriggersOfJob(x).Result.Any(x => (x as CronTriggerImpl).Name == req.TaskName)).FirstOrDefault();
                    var triggers = await scheduler.GetTriggersOfJob(jobKey);
                    ITrigger trigger = triggers?.Where(x => (x as CronTriggerImpl).Name == req.TaskName).FirstOrDefault();
                    await scheduler.ResumeTrigger(trigger.Key);
                }
                await _quartzService.Update(jobTask);
                var runlog = new TQuarzTaskLog()
                {
                    TaskName = req.TaskName,
                    GroupName = req.GroupName,
                    BeginDate = DateTime.Now,
                    Msg = $"任务启动成功，状态为：【{Enum.GetName(typeof(JobStateEnum), jobTask.Status)}】"
                };
                await _quartzLogService.AddLog(runlog);
                return RspCommon.Success("任务启动成功"); ;
            }
            catch (Exception ex)
            {
                return RspCommon.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 立即执行
        /// </summary>
        public async Task<RspCommon> Run(TQuarzTask req)
        {
            try
            {
                var job = await IsQuartzJob(req.TaskName, req.GroupName);
                if (!job.IsOk())
                    return job;
                IScheduler scheduler = await _schedulerFactory.GetScheduler();
                List<JobKey> jobKeys = scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(req.GroupName)).Result.ToList();
                JobKey jobKey = jobKeys.Where(x => scheduler.GetTriggersOfJob(x).Result.Any(x => (x as CronTriggerImpl).Name == req.TaskName)).FirstOrDefault();
                var triggers = await scheduler.GetTriggersOfJob(jobKey);
                ITrigger trigger = triggers?.Where(x => (x as CronTriggerImpl).Name == req.TaskName).FirstOrDefault();
                await scheduler.TriggerJob(jobKey);
                var runlog = new TQuarzTaskLog()
                {
                    TaskName = req.TaskName,
                    GroupName = req.GroupName,
                    BeginDate = DateTime.Now,
                    Msg = $"任务立即执行"
                };
                await _quartzLogService.AddLog(runlog);
                return RspCommon.Success($"任务立即执行成功");
            }
            catch (Exception ex)
            {
                return RspCommon.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        public async Task<RspCommon> Remove(TQuarzTask req)
        {
            var job = await IsQuartzJob(req.TaskName, req.GroupName);
            var jobTask = (await _quartzService.GetJobs(x => x.TaskName == req.TaskName && x.GroupName == req.GroupName)).FirstOrDefault();
            try
            {
                if (job.IsOk())
                {
                    IScheduler scheduler = await _schedulerFactory.GetScheduler();
                    List<JobKey> jobKeys = scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(req.GroupName)).Result.ToList();
                    JobKey jobKey = jobKeys.Where(x => scheduler.GetTriggersOfJob(x).Result.Any(x => (x as CronTriggerImpl).Name == req.TaskName)).FirstOrDefault();
                    var triggers = await scheduler.GetTriggersOfJob(jobKey);
                    ITrigger trigger = triggers?.Where(x => (x as CronTriggerImpl).Name == req.TaskName).FirstOrDefault();
                    await scheduler.PauseTrigger(trigger.Key);
                    await scheduler.UnscheduleJob(trigger.Key);// 移除触发器
                    await scheduler.DeleteJob(trigger.JobKey);
                }
                if (jobTask != null)
                {
                    await _quartzService.Remove(jobTask);
                    var runlog = new TQuarzTaskLog()
                    {
                        TaskName = req.TaskName,
                        GroupName = req.GroupName,
                        BeginDate = DateTime.Now,
                        Msg = $"任务移除成功"
                    };
                    await _quartzLogService.AddLog(runlog);
                }
                return RspCommon.Success("任务移除成功");
            }
            catch (Exception ex)
            {
                return RspCommon.Fail(ex.Message);
            }

        }

        /// <summary>
        /// 判断是否存在此任务
        /// </summary>
        public async Task<RspCommon> IsQuartzJob(string taskName, string groupName)
        {
            try
            {
                string errorMsg = "";
                IScheduler scheduler = await _schedulerFactory.GetScheduler();
                var jobKeys = scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName)).Result.ToList();
                if (jobKeys == null || jobKeys.Count() == 0)
                    return RspCommon.Fail($"未找到分组【{groupName}】");

                JobKey jobKey = jobKeys.Where(s => scheduler.GetTriggersOfJob(s).Result.Any(x => (x as CronTriggerImpl).Name == taskName)).FirstOrDefault();
                if (jobKey == null)
                    return RspCommon.Fail($"未找到任务【{taskName}】");

                var triggers = await scheduler.GetTriggersOfJob(jobKey);
                ITrigger trigger = triggers?.Where(x => (x as CronTriggerImpl).Name == taskName).FirstOrDefault();

                if (trigger == null)
                    return RspCommon.Fail($"未找到触发器【{taskName}】");

                return RspCommon.Success();
            }
            catch (Exception ex)
            {
                return RspCommon.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 验证cron
        /// </summary>
        public RspCommon IsValidExpression(string cronExpression)
        {
            try
            {
                CronTriggerImpl trigger = new CronTriggerImpl();
                trigger.CronExpressionString = cronExpression;
                DateTimeOffset? date = trigger.ComputeFirstFireTimeUtc(null);
                var isCron = date != null;
                if (!isCron)
                    return RspCommon.Fail($"请确认表达式【{cronExpression}】是否正确");

                return RspCommon.Success();
            }
            catch (Exception ex)
            {
                return RspCommon.Fail($"请确认表达式【{cronExpression}】是否正确");
            }
        }
    }
}
