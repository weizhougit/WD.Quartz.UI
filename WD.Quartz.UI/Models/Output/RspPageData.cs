namespace WD.Quartz.UI.Models.Output
{
    /// <summary>
    /// 分页数据
    /// </summary>
    public class RspPageData<T>
    {

        public RspPageData(IEnumerable<T> items, long total)
        {
            Data = items;
            Total = total;
        }

        public RspPageData()
        {

        }

        /// <summary>
        /// 数据列表
        /// </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// 空数据行
        /// </summary>
        /// <returns></returns>
        public static RspPageData<T> Empty()
        {
            return new RspPageData<T>(Array.Empty<T>(), 0);
        }
    }
}
