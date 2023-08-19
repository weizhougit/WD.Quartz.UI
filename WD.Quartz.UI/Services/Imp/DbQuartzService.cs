using System.Linq.Expressions;
using WD.Quartz.UI.Models.Enums;
using WD.Quartz.UI.Models.Output;
using WD.Quartz.UI.Models.PO;

namespace WD.Quartz.UI.Services.Imp
{
    public class DbQuartzService : IQuartzService
    {
        readonly IFreeSql _freeSql;
        public DbQuartzService(IFreeSql freeSql)
        {
            _freeSql = freeSql;
        }

        /// <summary>
        /// 添加
        /// </summary>
        public async Task<RspCommon> AddJob(TQuarzTask req)
        {
            var exist = await _freeSql.Select<TQuarzTask>()
                             .Where(_ => _.TaskName == req.TaskName && _.GroupName == req.GroupName)
                             .AnyAsync();
            if (exist) return RspCommon.Fail("该任务已存在");
            await _freeSql.Insert<TQuarzTask>(req).ExecuteAffrowsAsync();
            return RspCommon.Success("保存成功");
        }

        /// <summary>
        /// 获取
        /// </summary>
        public async Task<List<TQuarzTask>> GetJobs(Expression<Func<TQuarzTask, bool>> where)
        {
            return await _freeSql.Select<TQuarzTask>().Where(where).OrderBy(_ => _.CreateTime).ToListAsync();
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public async Task<RspCommon> Pause(TQuarzTask req)
        {
            var exist = await _freeSql.Select<TQuarzTask>()
                                      .Where(_ => _.TaskName == req.TaskName && _.GroupName == req.GroupName)
                                      .AnyAsync();
            if (!exist) return RspCommon.Fail("任务不存在");
            await _freeSql.Update<TQuarzTask>()
                         .Set(_ => _.Status, (int)JobStateEnum.暂停)
                         .Set(_ => _.UpdateTime, DateTime.Now)
                         .Where(_ => _.TaskName == req.TaskName && _.GroupName == req.GroupName)
                         .ExecuteAffrowsAsync();
            return RspCommon.Success("暂停成功");
        }

        /// <summary>
        /// 移除
        /// </summary>
        public async Task<RspCommon> Remove(TQuarzTask req)
        {
            var exist = await _freeSql.Select<TQuarzTask>()
                                  .Where(_ => _.TaskName == req.TaskName && _.GroupName == req.GroupName)
                                  .AnyAsync();
            if (!exist) return RspCommon.Fail("任务不存在");

            await _freeSql.Delete<TQuarzTask>()
                          .Where(_ => _.TaskName == req.TaskName && _.GroupName == req.GroupName)
                          .ExecuteAffrowsAsync();
            return RspCommon.Success("移除成功");
        }

        /// <summary>
        /// 启动
        /// </summary>
        public async Task<RspCommon> Start(TQuarzTask req)
        {
            var exist = await _freeSql.Select<TQuarzTask>()
                                  .Where(_ => _.TaskName == req.TaskName && _.GroupName == req.GroupName)
                                  .AnyAsync();
            if (!exist) return RspCommon.Fail("任务不存在");
            await _freeSql.Update<TQuarzTask>()
                         .Set(_ => _.Status, (int)JobStateEnum.开启)
                         .Set(_ => _.UpdateTime, DateTime.Now)
                         .Where(_ => _.TaskName == req.TaskName && _.GroupName == req.GroupName)
                         .ExecuteAffrowsAsync();
            return RspCommon.Success("启动成功");
        }


        /// <summary>
        /// 修改
        /// </summary>
        public async Task<RspCommon> Update(TQuarzTask req)
        {
            var exist = await _freeSql.Select<TQuarzTask>()
                        .Where(_ => _.TaskName == req.TaskName && _.GroupName == req.GroupName)
                        .AnyAsync();
            if (!exist) return RspCommon.Fail("任务不存在");

            await _freeSql.Update<TQuarzTask>()
                          .SetSource(req)
                          .IgnoreColumns(x => new
                          {
                              x.Id,
                              x.TaskName,
                              x.GroupName,
                              x.LastRunTime,
                              x.CreateTime,
                          })
                         .Where(_ => _.TaskName == req.TaskName && _.GroupName == req.GroupName)
                         .ExecuteAffrowsAsync();
            return RspCommon.Success("修改成功");
        }
    }
}
