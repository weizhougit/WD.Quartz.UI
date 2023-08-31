namespace WD.Quartz.UI.Models.BO
{
    public class MailOption
    {
        /// <summary>
        /// 邮箱 smtp 地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 发件人地址
        /// </summary>
        public string SenderAddress { get; set; }

        /// <summary>
        /// 发件人姓名
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// 邮箱密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 收件邮箱
        /// </summary>
        public string ReciverEmail { get; set; }

        /// <summary>
        /// 邮箱服务器 smtp 端口
        /// </summary>
        public int Port { get; set; } = 465;

        /// <summary>
        /// 是否采用ssl链接
        /// </summary>
        public bool UseSsl { get; set; } = true;
    }
}
