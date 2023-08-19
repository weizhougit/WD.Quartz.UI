using System.Linq.Expressions;
using WD.Quartz.UI.Models.Input;
using WD.Quartz.UI.Models.Output;
using WD.Quartz.UI.Models.PO;

namespace WD.Quartz.UI.Services.Imp
{
    public class DbQuartzLogService : IQuartzLogService
    {
        readonly IFreeSql _freeSql;
        public DbQuartzLogService(IFreeSql freeSql)
        {

            _freeSql = freeSql;
        }

        /// <summary>
        /// 获取分页日志
        /// </summary>
        public async Task<RspPageData<TQuarzTaskLog>> GetLogPageList(ReqLogPageQuery req)
        {
            var logs = await _freeSql.Select<TQuarzTaskLog>()
                                     .Where(_ => _.TaskName == req.TaskName && _.GroupName == req.GroupName)
                                     .OrderByDescending(_ => _.BeginDate)
                                     .Page(req.PageNumber, req.PageSize)
                                     .Count(out long recordCount)
                                     .ToListAsync();
            return new RspPageData<TQuarzTaskLog> { Total = recordCount, Data = logs };
        }

        /// <summary>
        /// 获取最后日志
        /// </summary>
        public async Task<RspCommon<TQuarzTaskLog>> GetLastlog(ReqLogPageQuery req)
        {
            var log = await _freeSql.Select<TQuarzTaskLog>()
                                  .Where(_ => _.TaskName == req.TaskName && _.GroupName == req.GroupName)
                                  .OrderByDescending(_ => _.BeginDate)
                                  .FirstAsync();

            return RspCommon.Success(log);

        }

        /// <summary>
        /// 添加日志
        /// </summary>
        public async Task<RspCommon> AddLog(TQuarzTaskLog req)
        {
            await _freeSql.Insert<TQuarzTaskLog>(req).ExecuteAffrowsAsync();
            return RspCommon.Success("日志数据保存成功");
        }

        /// <summary>
        /// 删除日志
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<RspCommon> DeleteLogByWhere(int day)
        {
            var date = DateTime.Now.AddDays(-day).Date;
            await _freeSql.Delete<TQuarzTaskLog>().Where(x => x.BeginDate <= date).ExecuteAffrowsAsync();
            return RspCommon.Success("删除日志数据成功");
        }
    }
}
