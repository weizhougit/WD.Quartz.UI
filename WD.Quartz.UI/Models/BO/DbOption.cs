using FreeSql;

namespace WD.Quartz.UI.Models.BO
{
    public class DbOption
    {
        public DataType DataType { get; set; }

        public string ConnectionString { get; set; }

        public bool Enable { get; set; } = false;
    }
}
