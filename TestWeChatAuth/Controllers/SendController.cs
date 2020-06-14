using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TestWeChatAuth.Controllers
{
    [Route("aa")]
    [ApiController]
    public class SendController : ControllerBase
    {
        private static readonly string token = "9ce75fb_8a64711e_a9f9700";

        [HttpGet("send")]
        [AllowAnonymous]
        public async Task<string> SendAsync(string code, string phone, string number)
        {
            var model = new
            {
                Phone = phone,
                WeChatCode = code,
                Code = number,
                AppId = "wx1ba34dff5334199d",
                ClientId = 111222
            };
            //var url = "http://www.baidu.com";
            var url = "http://127.0.0.1:2595/api/user/wechatLogin";
            var jsonData = JsonConvert.SerializeObject (model);
            var httpClient = new HttpClient();
            var httpContent = new StringContent(jsonData);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var resp = await httpClient.PostAsync(url, httpContent);
            if (!resp.IsSuccessStatusCode)
                return "请求了";
            var result = await resp.Content.ReadAsStringAsync();
            return result;
        }

        [HttpGet("check")]
        [AllowAnonymous]
        public string CheckAsync([FromQuery] WXRequest input)
        {
            var xxx = JsonConvert.SerializeObject(input);
            var aaa = new string[] { token, input.TimeStamp, input.Nonce };
            Array.Sort(aaa);

            string bbb = aaa[0] + aaa[1] + aaa[2];
            byte[] temp1 = Encoding.UTF8.GetBytes(bbb);
            SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider();
            byte[] temp2 = sha.ComputeHash(temp1);
            sha.Clear();
            // 注意， 不能用这个
            // string output = Convert.ToBase64String(temp2);// 不能直接转换成base64string
            var output = BitConverter.ToString(temp2);
            output = output.Replace("-", "");
            output = output.ToLower();
            if (input.Signature == output)
                return input.Echostr;

            return "false";
        }
    }

    public class WXRequest
    {
        public string Signature { get; set; }
        public string TimeStamp { get; set; }

        public string Nonce { get; set; }

        public string Echostr { get; set; }
    }
}
