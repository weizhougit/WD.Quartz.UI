using Newtonsoft.Json;
using System.Linq.Expressions;
using WD.Quartz.UI.Extensions;
using WD.Quartz.UI.Models.BO;
using WD.Quartz.UI.Models.PO;

namespace WD.Quartz.UI.Helpers
{
    public class QuartzFileHelper
    {
        string _rootPath { get; set; }
        string _logPath { get; set; }
        private string QuartzSettingsFolder { get; set; } = "QuartzSettings";
        private string TaskJobFileName { get; set; } = "quartz_task_job.json";
        private string Logs { get; set; } = "logs";

        private string MailFile { get; set; } = "Mail.txt";

        private string UserFile { get; set; } = "User.txt";

        private string DataBaseFile { get; set; } = "DataBaseFile.txt";

        private IWebHostEnvironment _env;

        public string RootPath { get { return _rootPath; } }
        public string LogPath { get { return _logPath; } }

        public QuartzFileHelper(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            CreateQuartzRootPath();
        }

        /// <summary>
        /// 创建作业所在根目录及日志文件夹 
        /// </summary>
        public string CreateQuartzRootPath()
        {
            var basePath = Directory.GetParent(_env.ContentRootPath).FullName;
            _rootPath = Path.Combine(basePath, QuartzSettingsFolder);
            _logPath = Path.Combine(_rootPath, Logs);
            //生成日志文件夹
            if (!Directory.Exists(_logPath))
                Directory.CreateDirectory(_logPath);
            return _rootPath;
        }


        #region jobs
        /// <summary>
        /// 获取jobs
        /// </summary>
        public List<TQuarzTask> GetJobs(Expression<Func<TQuarzTask, bool>> where)
        {
            string path = $"{_rootPath}\\{TaskJobFileName}";
            List<TQuarzTask> list = new List<TQuarzTask>();
            path = path.ReplacePath();
            if (!File.Exists(path))
                return list;
            var tasks = FileHelper.ReadFile(path);
            if (string.IsNullOrEmpty(tasks))
                return null;
            var taskList = JsonConvert.DeserializeObject<List<TQuarzTask>>(tasks);
            return taskList.Where(where.Compile()).ToList();
        }

        /// <summary>
        /// 读取任务日志
        /// </summary>
        public List<TQuarzTaskLog> GetJobRunLog(string taskName, string groupName, int page, int pageSize = 100)
        {
            string path = $"{_logPath}{groupName}\\{taskName}";
            List<TQuarzTaskLog> list = new List<TQuarzTaskLog>();
            path = path.ReplacePath();
            if (!File.Exists(path))
                return list;
            var logs = FileHelper.ReadPageLine(path, page, pageSize, true);
            foreach (string item in logs)
            {
                string[] arr = item?.Split('_');
                if (item == "" || arr == null || arr.Length == 0)
                    continue;
                if (arr.Length != 3)
                {
                    list.Add(new TQuarzTaskLog() { Msg = item });
                    continue;
                }
                list.Add(new TQuarzTaskLog() { BeginDate = Convert.ToDateTime(arr[0]), EndDate = Convert.ToDateTime(arr[1]), Msg = arr[2] });
            }
            return list.OrderByDescending(x => x.BeginDate).ToList();
        }


        /// <summary>
        /// 写入任务
        /// </summary>
        public void WriteJobConfig(List<TQuarzTask> taskList)
        {
            string jobs = JsonConvert.SerializeObject(taskList);
            //写入配置文件
            FileHelper.WriteFile(_rootPath, TaskJobFileName, jobs);
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        public void WriteJobLogs(TQuarzTaskLog tasklog)
        {
            var fileName = $"log_{DateTime.Now.ToString("yyyyMMdd")}.txt";
            var content = JsonConvert.SerializeObject(tasklog) + "\r\n";
            FileHelper.WriteFile(LogPath, fileName, content, true);
        }

        /// <summary>
        /// 获取job日志
        /// </summary><returns></returns>
        public List<TQuarzTaskLog> GetJobsLog(int pageSize = 1)
        {
            var fileName = $"log_{DateTime.Now.ToString("yyyyMMdd")}.txt";
            string path = Path.Combine(LogPath, fileName);
            if (!File.Exists(path))
                return default;

            var logs = FileHelper.ReadPageLine(path, pageSize, 5000, true).ToList();
            var taskLogs = new List<TQuarzTaskLog>();
            foreach (var item in logs)
            {
                taskLogs.Add(JsonConvert.DeserializeObject<TQuarzTaskLog>(item));
            }
            return taskLogs;
        }

        /// <summary>
        /// 删除job日志
        /// </summary>
        /// <param name="day"></param>
        public void DeleteJobsLog(int day)
        {
            FileHelper.DeleteFolder(LogPath, day);
        }
        #endregion


        #region 用户配置
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <returns></returns>
        public UserOption GetUserInfo()
        {
            var filePath = Path.Combine(_rootPath, UserFile);
            if (!File.Exists(filePath))
                return new UserOption();
            var userStr = FileHelper.ReadFile(filePath);
            if (string.IsNullOrEmpty(userStr))
                return new UserOption();
            var user = JsonConvert.DeserializeObject<UserOption>(userStr);
            return user;
        }


        /// <summary>
        /// 保存用户
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public void SaveUserInfo(UserOption req)
        {
            string userStr = JsonConvert.SerializeObject(req);
            //写入配置文件
            FileHelper.WriteFile(_rootPath, UserFile, userStr);
        }
        #endregion

        #region 邮箱配置
        /// <summary>
        /// 获取邮件
        /// </summary>
        /// <returns></returns>
        public MailOption GetMailInfo()
        {
            var filePath = Path.Combine(_rootPath, MailFile);
            if (!File.Exists(filePath))
                return new MailOption();
            var mailStr = FileHelper.ReadFile(filePath);
            if (string.IsNullOrEmpty(mailStr))
                return new MailOption();
            var mail = JsonConvert.DeserializeObject<MailOption>(mailStr);
            return mail;
        }


        /// <summary>
        /// 保存邮件
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public void SaveMailInfo(MailOption req)
        {
            string mailStr = JsonConvert.SerializeObject(req);
            //写入配置文件
            FileHelper.WriteFile(_rootPath, MailFile, mailStr);
        }
        #endregion

        #region 数据库配置

        /// <summary>
        /// 获取数据库配置
        /// </summary>
        /// <returns></returns>
        public DbOption GetDbInfo()
        {
            var filePath = Path.Combine(_rootPath, DataBaseFile);
            if (!File.Exists(filePath))
                return new DbOption();
            var dbStr = FileHelper.ReadFile(filePath);
            if (string.IsNullOrEmpty(dbStr))
                return new DbOption();
            var db = JsonConvert.DeserializeObject<DbOption>(dbStr);
            return db;
        }

        /// <summary>
        /// 保存数据库配置
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public void SaveDbInfo(DbOption req)
        {
            string dbStr = JsonConvert.SerializeObject(req);
            //写入配置文件
            FileHelper.WriteFile(_rootPath, DataBaseFile, dbStr);
        }
        #endregion
    }
}
