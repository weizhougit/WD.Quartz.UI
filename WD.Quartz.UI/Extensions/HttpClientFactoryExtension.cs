using System.Text;
using WD.Quartz.UI.Models.Output;

namespace WD.Quartz.UI.Extensions
{
    public static class HttpClientFactoryExtension
    {
        public static async Task<RspCommon> HttpSendAsync(this IHttpClientFactory httpClientFactory, HttpMethod method, string url, string parmet, Dictionary<string, string> headers = null)
        {
            var client = httpClientFactory.CreateClient();
            var postContent = new StringContent(parmet, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(method, url)
            {
                Content = postContent
            };
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            try
            {
                var message = await client.SendAsync(request);
                if (!message.IsSuccessStatusCode)
                    return RspCommon.Fail($"连接失败，状态码：{message.StatusCode}");

                var res = await message.Content.ReadAsStringAsync();
                return RspCommon.Success(res);
            }
            catch (Exception ex)
            {
                return RspCommon.Fail($"出现异常：{ex.Message}");
            }
        }
    }
}
