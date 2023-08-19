using System.Linq.Expressions;
using WD.Quartz.UI.Helpers;
using WD.Quartz.UI.Models.Enums;
using WD.Quartz.UI.Models.Output;
using WD.Quartz.UI.Models.PO;

namespace WD.Quartz.UI.Services.Imp
{
    public class FileQuartzService : IQuartzService
    {

        private QuartzFileHelper _quartzFileHelper;
        public FileQuartzService(QuartzFileHelper quartzFileHelper)
        {
            _quartzFileHelper = quartzFileHelper;
        }

        /// <summary>
        /// 添加
        /// </summary>
        public Task<RspCommon> AddJob(TQuarzTask req)
        {
            return Task.Run(() =>
            {
                var list = _quartzFileHelper.GetJobs(x => 1 == 1);
                if (list == null)
                    list = new List<TQuarzTask>();

                if (list.Count == 0)
                {
                    req.Id = 1;
                }
                else
                {
                    req.Id = list.Max(x => x.Id) + 1;
                }
                list.Add(req);
                req.CreateTime = DateTime.Now;
                _quartzFileHelper.WriteJobConfig(list);
                return RspCommon.Success("保存成功");
            });
        }

        /// <summary>
        /// 获取
        /// </summary>
        public Task<List<TQuarzTask>> GetJobs(Expression<Func<TQuarzTask, bool>> where)
        {
            return Task.Run(() =>
            {
                var list = _quartzFileHelper.GetJobs(where);
                return list;
            });
        }

        /// <summary>
        /// 移除
        /// </summary>
        public Task<RspCommon> Remove(TQuarzTask req)
        {
            return Task.Run(() =>
            {
                var list = _quartzFileHelper.GetJobs(x => 1 == 1);
                var task = list.Find(x => x.TaskName == req.TaskName && x.GroupName == req.GroupName);
                list.Remove(task);
                _quartzFileHelper.WriteJobConfig(list);
                return RspCommon.Success("移除成功");
            });
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public Task<RspCommon> Pause(TQuarzTask req)
        {
            return Task.Run(() =>
            {
                var list = _quartzFileHelper.GetJobs(x => 1 == 1);
                list.ForEach(x =>
                {
                    if (x.TaskName == req.TaskName && x.GroupName == req.GroupName)
                    {
                        x.Status = (int)JobStateEnum.暂停;
                    }
                });
                _quartzFileHelper.WriteJobConfig(list);
                return RspCommon.Success("暂停成功");

            });
        }

        /// <summary>
        /// 修改
        /// </summary>
        public Task<RspCommon> Update(TQuarzTask req)
        {
            return Task.Run(() =>
            {
                var list = _quartzFileHelper.GetJobs(x => 1 == 1);
                var task = list.Find(x => x.Id == req.Id);
                list.Remove(task);
                list.Add(req);
                req.UpdateTime = DateTime.Now;
                _quartzFileHelper.WriteJobConfig(list);
                return RspCommon.Success("修改成功");
            });
        }

        /// <summary>
        /// 启动
        /// </summary>
        public Task<RspCommon> Start(TQuarzTask req)
        {
            return Task.Run(() =>
            {
                var list = _quartzFileHelper.GetJobs(x => 1 == 1);
                list.ForEach(x =>
                {
                    if (x.TaskName == req.TaskName && x.GroupName == req.GroupName)
                    {
                        x.Status = (int)JobStateEnum.开启;
                    }
                });
                _quartzFileHelper.WriteJobConfig(list);
                return RspCommon.Success("启动成功");
            });
        }
    }
}
