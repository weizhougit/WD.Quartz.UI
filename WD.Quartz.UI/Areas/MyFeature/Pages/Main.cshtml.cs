using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WD.Quartz.UI.Converter;
using WD.Quartz.UI.Factory;
using WD.Quartz.UI.Handle;
using WD.Quartz.UI.Models.Enums;
using WD.Quartz.UI.Models.Input;
using WD.Quartz.UI.Models.PO;
using WD.Quartz.UI.Services;

namespace WD.Quartz.UI.Areas.MyFeature.Pages
{
    public class MainModel : PageModel
    {
        readonly IQuartzHandle _quartzHandle;
        readonly IQuartzLogService _logService;
        public MainModel(IQuartzHandle quartzHandle, IQuartzLogService logService)
        {
            _quartzHandle = quartzHandle;
            _logService = logService;
        }

        [BindProperty]
        public TQuarzTask Input { get; set; }


        /// <summary>
        /// ��ȡ�����б�
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostJobList(int pageNumber, int pageSize)
        {
            var req = new ReqTaskPageQuery()
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var res = await _quartzHandle.GetJobs(req);
            return new JsonDataResult(res);
        }

        /// <summary>
        /// �½�����
        /// </summary>
        public async Task<IActionResult> OnPostAddJob()
        {
            var res = await _quartzHandle.Add(Input);
            Input.Status = (int)JobStateEnum.��ͣ;
            return new JsonDataResult(res);
        }


        /// <summary>
        /// �޸�����
        /// </summary>
        public async Task<IActionResult> OnPostUpdateJob()
        {

            var res = await _quartzHandle.Update(Input);
            return new JsonDataResult(res);
        }


        /// <summary>
        /// ��ͣ����
        /// </summary>
        public async Task<IActionResult> OnPostPauseJob()
        {
            var res = await _quartzHandle.Pause(Input);
            return new JsonDataResult(res);
        }

        /// <summary>
        /// ��������
        /// </summary>
        public async Task<IActionResult> OnPostStartJob()
        {
            var res = await _quartzHandle.Start(Input);
            return new JsonDataResult(res);
        }

        /// <summary>
        /// ����ִ������
        /// </summary>
        public async Task<IActionResult> OnPostRunJob()
        {
            var res = await _quartzHandle.Run(Input);
            return new JsonDataResult(res);
        }

        /// <summary>
        /// ɾ������
        /// </summary>
        public async Task<IActionResult> OnPostDeleteJob()
        {
            var res = await _quartzHandle.Remove(Input);
            return new JsonDataResult(res);
        }

        /// <summary>
        /// ��ȡ��ע���������
        /// </summary>
        public IActionResult OnGetSelectClassJob()
        {
            var res = ClassJobFactory.GetAll();
            return new JsonDataResult(res);
        }

        /// <summary>
        /// ��ȡ����ִ�м�¼
        /// </summary>
        public async Task<IActionResult> OnPostJobRecord(string taskName, string groupName, int pageNumber, int pageSize)
        {
            var req = new ReqLogPageQuery
            {
                TaskName = taskName,
                GroupName = groupName,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var res = await _logService.GetLogPageList(req);
            return new JsonDataResult(res);
        }


        /// <summary>
        /// �ǳ�
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetLogOut()
        {
            return Redirect("/Main");
        }

        public void OnGet()
        {
        }
    }
}
