using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WD.Quartz.UI.Models.Input
{
    public class ReqPage
    {
        [Description("页数")]
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Description("每页数量")]
        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        [Description("排序字段")]
        public string SortName { get; set; } = "CreateDate";

        [Description("排序方式，缺省不填就是升序")]
        public bool? Ascending { get; set; }
    }
}
