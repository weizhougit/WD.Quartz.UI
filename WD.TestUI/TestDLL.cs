using WD.Quartz.UI.Services;

namespace WD.TestUI
{
    public class TestDLL : IJobService
    {
        public string Execute(string parameter)
        {
            return "定时任务已执行成功!";
        }
    }
}
