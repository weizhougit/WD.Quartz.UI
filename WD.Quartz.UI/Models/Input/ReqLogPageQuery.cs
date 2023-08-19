using System.ComponentModel;

namespace WD.Quartz.UI.Models.Input
{
    public class ReqLogPageQuery : ReqPage
    {
        /// <summary>
        /// 任务名
        /// </summary>
        [Description("任务名")]
        public string TaskName { get; set; }

        /// <summary>
        /// 分组名
        /// </summary>
        [Description("分组名")]
        public string GroupName { get; set; }
    }
}
