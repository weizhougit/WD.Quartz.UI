namespace WD.Quartz.UI.Models.Output
{
    /// <summary>
    /// 公共返回
    /// </summary>
    public class RspCommon
    {
        public RspCommon(int result = 1, string message = "")
        {
            Set(result, message);
        }


        public void Set(int result = 1, string message = "")
        {
            Result = result;
            Message = message;
        }

        /// <summary>
        /// 返回值 
        ///  小于等于 0 时（返回警告消息），请求出错 
        ///  小于等于 -99（返回异常消息） 异常  
        ///  大于 0 成功 （返回成功消息）
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// 返回信息（界面提示）
        /// </summary>
        public string Message { get; set; }

        public bool IsOk()
        {
            return Result > 0;
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        public static RspCommon Success(string message = "")
        {
            return new RspCommon { Result = 1, Message = message };
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        public static RspCommon<T> Success<T>(T data, string message = "") => new(data, 1, message);

        /// <summary>
        /// 返回失败
        /// </summary>
        public static RspCommon Fail(string message = "", int result = -1)
        {
            return new RspCommon { Result = result, Message = message };
        }

        /// <summary>
        /// 返回失败
        /// </summary>
        public static RspCommon<T> Fail<T>(string message) => new(-1, message);

        /// <summary>
        /// 返回失败
        /// </summary>
        public static RspCommon<T> Fail<T>(string message, T data) => new(data, -1, message);

        /// <summary>
        /// 获取错误返回值
        /// </summary>
        /// <returns></returns>
        public static RspCommon GetErrorResult(Exception exception, string message = "内部错误")
        {
            return new RspCommon() { Result = -99, Message = message };
        }

        /// <summary>
        /// 回值页面数据
        /// </summary>
        public static RspCommonPaging<T> Pageing<T>(IEnumerable<T> data, long total) => new(data, total);

        public override string ToString()
        {
            return string.Format("code:{0},msg:{1}", Result, Message);
        }
    }

    public class RspCommon<T> : RspCommon
    {
        public RspCommon(int result = 1, string message = "")
        {
            Set(result, message);
        }

        public RspCommon(T t, int result = 1, string message = "")
        {
            Set(result, message);
            Data = t;
        }

        /// <summary>
        /// 详细数据
        /// </summary>
        public T Data { get; set; }
    }

    /// <summary>
    /// 分页返回对象
    /// </summary>
    public class RspCommonPaging<T> : RspCommon
    {
        public RspCommonPaging(IEnumerable<T> t, long total, int result = 1, string message = "")
        {
            Set(result, message);
            Total = total;
            Data = t;
        }

        public RspCommonPaging(RspPageData<T> t, int result = 1, string message = "")
        {
            Set(result, message);
            Total = t.Total;
            Data = t.Data;
        }

        public RspCommonPaging(string message, int result = -1)
        {
            Set(result, message);
            Total = 0;
            Data = Array.Empty<T>();
        }

        /// <summary>
        /// 总数量
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// 详细数据
        /// </summary>
        public IEnumerable<T> Data { get; set; }

    }
}
