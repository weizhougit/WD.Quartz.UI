using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Unicode;

namespace WD.Quartz.UI.Converter
{
    public class JsonDataResult : JsonResult
    {
        public JsonDataResult(object values, object option) : base(values, option)
        {
        }

        public JsonDataResult(object values) : base(values)
        {

        }

        public override void ExecuteResult(ActionContext context)
        {
            var services = context.HttpContext.RequestServices;
            var executor = services.GetRequiredService<IActionResultExecutor<JsonResult>>();
            var typename = executor.GetType().FullName;
            if (typename.Equals("Microsoft.AspNetCore.Mvc.Infrastructure.SystemTextJsonResultExecutor"))
            {
                this.SerializerSettings = new System.Text.Json.JsonSerializerOptions()
                {
                    PropertyNamingPolicy = null,
                    WriteIndented = true,
                    Converters = { new DatetimeJsonConverter() },
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All)
                };

            }
            if (typename.Equals("Microsoft.AspNetCore.Mvc.NewtonsoftJson.NewtonsoftJsonResultExecutor"))
            {
                this.SerializerSettings = new Newtonsoft.Json.JsonSerializerSettings()
                {
                    DateFormatString = "yyyy-MM-dd HH:mm:ss",
                    ContractResolver = null,
                    DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local,
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize
                };
            }
            base.ExecuteResult(context);
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            var services = context.HttpContext.RequestServices;
            var executor = services.GetRequiredService<IActionResultExecutor<JsonResult>>();
            var typename = executor.GetType().FullName;
            if (typename.Equals("Microsoft.AspNetCore.Mvc.Infrastructure.SystemTextJsonResultExecutor"))
            {
                this.SerializerSettings = new System.Text.Json.JsonSerializerOptions()
                {
                    PropertyNamingPolicy = null,
                    WriteIndented = true,
                    Converters = { new DatetimeJsonConverter() },
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All)
                };
            }
            if (typename.Equals("Microsoft.AspNetCore.Mvc.NewtonsoftJson.NewtonsoftJsonResultExecutor"))
            {
                this.SerializerSettings = new Newtonsoft.Json.JsonSerializerSettings()
                {
                    DateFormatString = "yyyy-MM-dd HH:mm:ss",
                    ContractResolver = null,
                    DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local,
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize
                };
            }
            return base.ExecuteResultAsync(context);
        }

    }
}
