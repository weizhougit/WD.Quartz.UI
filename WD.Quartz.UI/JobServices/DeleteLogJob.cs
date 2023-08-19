using WD.Quartz.UI.Services;

namespace WD.Quartz.UI.JobServices
{
    public class DeleteLogJob : IJobService
    {
        private readonly IQuartzLogService _quartzLogService;
        public DeleteLogJob(IQuartzLogService quartzLogService)
        {
            _quartzLogService = quartzLogService;
        }

        public string Execute(string parameter)
        {
            var day = 7;
            if (!string.IsNullOrWhiteSpace(parameter) && !int.TryParse(parameter, out day))
                return $"参数格式错误：【{parameter}】";
            var res = _quartzLogService.DeleteLogByWhere(day).Result;
            return $"成功删除【{day}】天之前的日志";
        }
    }
}
