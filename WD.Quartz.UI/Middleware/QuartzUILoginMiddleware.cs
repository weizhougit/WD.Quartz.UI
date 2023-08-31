using System.Net;
using System.Text;
using WD.Quartz.UI.Services;

namespace WD.Quartz.UI.Middleware
{
    public class QuartzUILoginMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUserService _userService;

        public QuartzUILoginMiddleware(RequestDelegate next,
            IUserService userService)
        {
            _next = next;
            _userService = userService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //拦截QuartzUI开头的访问
            if (context.Request.Path.StartsWithSegments("/QuartzUI"))
            {
                var args = context.Request.Query["handler"];
                if (args.Count() > 0 && args[0].ToLower() == "logout")
                {
                    context.Response.Headers["WWW-Authenticate"] = "Basic";
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }
                string authHeader = context.Request.Headers["Authorization"];
                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    //帐户密码读取并解码
                    var encoded = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();
                    var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                    var username = decoded.Split(':', 2)[0];
                    var password = decoded.Split(':', 2)[1];
                    if (IsAuthorized(username, password))
                    {
                        await _next.Invoke(context);
                        return;
                    }
                }
                context.Response.Headers["WWW-Authenticate"] = "Basic";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        /// <summary>
        /// 设置密码
        /// </summary>
        public bool IsAuthorized(string userName, string password)
        {
            // 从配置读取帐户密码,否则默认
            var user = _userService.Get().Result;
            var UserName = user.Data?.UserName ?? "admin";
            var Password = user.Data?.Password ?? "123@abc";
            return userName.Equals(UserName) && password.Equals(Password);
        }
    }
}
