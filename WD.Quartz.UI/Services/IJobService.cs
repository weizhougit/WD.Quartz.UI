using WD.Quartz.UI.Models.Output;

namespace WD.Quartz.UI.Services
{
    public interface IJobService
    {
        string Execute(string parameter);
    }
}
