using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WD.Quartz.UI.Helpers;
using WD.Quartz.UI.Models.Input;
using WD.Quartz.UI.Models.Output;
using WD.Quartz.UI.Models.PO;

namespace WD.Quartz.UI.Services.Imp
{
    public class FileQuartzLogService : IQuartzLogService
    {
        readonly QuartzFileHelper _quartzFileHelper;
        public FileQuartzLogService(QuartzFileHelper quartzFileHelper)
        {
            _quartzFileHelper = quartzFileHelper;

        }

        /// <summary>
        /// 获取分页日志
        /// </summary>
        public Task<RspPageData<TQuarzTaskLog>> GetLogPageList(ReqLogPageQuery req)
        {
            return Task.Run(() =>
            {
                var list = _quartzFileHelper.GetJobsLog();
                if (list == null)
                    return RspPageData<TQuarzTaskLog>.Empty();
                int total = list.Where(x => x.TaskName == req.TaskName && x.GroupName == req.GroupName).Count();
                var data = list.Where(x => x.TaskName == req.TaskName && x.GroupName == req.GroupName).OrderByDescending(x => x.BeginDate)
                               .Skip((req.PageNumber - 1) * req.PageSize).Take(req.PageSize).ToList();
                return new RspPageData<TQuarzTaskLog> { Total = total, Data = data };
            });
        }


        /// <summary>
        /// 获取最后日志
        /// </summary>
        public Task<RspCommon<TQuarzTaskLog>> GetLastlog(ReqLogPageQuery req)
        {
            return Task.Run(() =>
            {
                var list = _quartzFileHelper.GetJobsLog();
                if (list == null)
                    return RspCommon.Success(new TQuarzTaskLog());

                var data = list.Where(x => x.TaskName == req.TaskName && x.GroupName == req.GroupName).OrderByDescending(x => x.BeginDate).FirstOrDefault();
                return data == null ? RspCommon.Success(new TQuarzTaskLog()) : RspCommon.Success(data);
            });
        }


        /// <summary>
        /// 添加日志
        /// </summary>
        public Task<RspCommon> AddLog(TQuarzTaskLog req)
        {
            return Task.Run(() =>
            {
                try
                {
                    req.CreateTime = DateTime.Now;
                    req.UpdateTime = DateTime.Now;
                    _quartzFileHelper.WriteJobLogs(req);
                    return RspCommon.Success("日志数据保存成功");
                }
                catch (Exception)
                {
                    return RspCommon.Fail("日志数据保存失败");
                }
            });
        }

        /// <summary>
        /// 删除日志
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<RspCommon> DeleteLogByWhere(int day)
        {
            return Task.Run(() =>
            {
                try
                {
                    _quartzFileHelper.DeleteJobsLog(day);
                    return RspCommon.Success("删除日志数据成功");
                }
                catch (Exception)
                {
                    return RspCommon.Fail("删除日志数据失败");
                }
            });
        }
    }
}
