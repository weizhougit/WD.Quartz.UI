using WD.Quartz.UI.Models.Output;

namespace WD.Quartz.UI.Factory
{
    public class ClassJobFactory
    {
        private static List<string> ClassJobs { get; set; } = new List<string>();


        public static RspCommon<List<string>> GetAll()
        {
            var dll = ClassJobs.ToList();
            return RspCommon.Success(dll);
        }

        public static RspCommon<string> Get(string name)
        {
            var dll = ClassJobs.Where(x => x == name).FirstOrDefault();
            return RspCommon.Success<string>(dll);
        }

        public static RspCommon Add(string name)
        {
            if (ClassJobs.Contains(name))
                return RspCommon.Success();

            ClassJobs.Add(name);
            return RspCommon.Success();
        }
    }
}
