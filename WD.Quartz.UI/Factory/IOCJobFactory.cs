using Quartz;
using Quartz.Spi;

namespace WD.Quartz.UI.Factory
{
    public class IOCJobFactory : IJobFactory
    {
        readonly IServiceScopeFactory _serviceProvider;
        public IOCJobFactory(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceProvider = serviceScopeFactory;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                var sevice = _serviceProvider.CreateScope();
                return sevice.ServiceProvider.GetService(bundle.JobDetail.JobType) as IJob;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
        }
    }
}
