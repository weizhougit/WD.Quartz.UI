using System.Linq.Expressions;
using WD.Quartz.UI.Models.Input;
using WD.Quartz.UI.Models.Output;
using WD.Quartz.UI.Models.PO;

namespace WD.Quartz.UI.Services
{
    public interface IQuartzLogService
    {
        /// <summary>
        /// 获取分页日志
        /// </summary>
        Task<RspPageData<TQuarzTaskLog>> GetLogPageList(ReqLogPageQuery req);

        /// <summary>
        /// 获取最后日志
        /// </summary>
        Task<RspCommon<TQuarzTaskLog>> GetLastlog(ReqLogPageQuery req);

        /// <summary>
        /// 添加日志
        /// </summary>
        Task<RspCommon> AddLog(TQuarzTaskLog req);

        /// <summary>
        /// 删除日志
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<RspCommon> DeleteLogByWhere(int day);
    }
}
