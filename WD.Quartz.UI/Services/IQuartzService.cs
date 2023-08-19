using System.Linq.Expressions;
using WD.Quartz.UI.Models.Output;
using WD.Quartz.UI.Models.PO;

namespace WD.Quartz.UI.Services
{
    public interface IQuartzService
    {

        /// <summary>
        /// 获取所有job
        /// </summary>
        Task<List<TQuarzTask>> GetJobs(Expression<Func<TQuarzTask, bool>> where);

        /// <summary>
        /// 添加任务
        /// </summary>
        Task<RspCommon> AddJob(TQuarzTask req);

        /// <summary>
        /// 更新任务
        /// </summary>
        Task<RspCommon> Update(TQuarzTask req);

        /// <summary>
        /// 删除任务
        /// </summary>
        Task<RspCommon> Remove(TQuarzTask req);

        /// <summary>
        /// 启动
        /// </summary>
        Task<RspCommon> Start(TQuarzTask req);

        /// <summary>
        /// 暂停
        /// </summary>
        Task<RspCommon> Pause(TQuarzTask req);

    }
}
