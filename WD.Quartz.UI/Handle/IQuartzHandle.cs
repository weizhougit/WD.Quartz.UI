using WD.Quartz.UI.Models.Input;
using WD.Quartz.UI.Models.Output;
using WD.Quartz.UI.Models.PO;

namespace WD.Quartz.UI.Handle
{
    public interface IQuartzHandle
    {

        /// <summary>
        /// 初始化
        /// </summary>
        Task InitJobs();


        /// <summary>
        /// 获取
        /// </summary>
        Task<RspPageData<TQuarzTask>> GetJobs(ReqTaskPageQuery req);


        /// <summary>
        /// 添加
        /// </summary>
        Task<RspCommon> Add(TQuarzTask req);

        /// <summary>
        /// 开始
        /// </summary>
        Task<RspCommon> Start(TQuarzTask req);

        /// <summary>
        /// 暂停
        /// </summary>
        Task<RspCommon> Pause(TQuarzTask req);

        /// <summary>
        /// 运行
        /// </summary>
        Task<RspCommon> Run(TQuarzTask req);

        /// <summary>
        /// 移除
        /// </summary>
        Task<RspCommon> Remove(TQuarzTask req);

        /// <summary>
        /// 修改
        /// </summary>
        Task<RspCommon> Update(TQuarzTask req);

        /// <summary>
        /// 是否Job
        /// </summary>
        /// <returns></returns>
        Task<RspCommon> IsQuartzJob(string taskName, string groupName);


        /// <summary>
        /// 验证cron
        /// </summary>
        RspCommon IsValidExpression(string cronExpression);
    }
}
