using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace Bookshelf.Server.Helper
{
    public class BookshelfHttpClient : HttpClient
    {
        private readonly IConfiguration _configuration;

        public BookshelfHttpClient(IConfiguration configuration)
        {
            _configuration = configuration;

            // 从配置中获取超时设置，默认设置为 30 秒
            var timeoutInSeconds = _configuration.GetValue<int>("HttpClientSettings:TimeoutInSeconds", 30);
            Timeout = TimeSpan.FromSeconds(timeoutInSeconds);

            // 可选：如果需要配置其他 HttpClient 选项
            DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (platform; rv:geckoversion) Gecko/geckotrail Firefox/firefoxversion\r\n");
        }
    }
}
