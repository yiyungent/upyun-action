using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpyunAction.ResponseModels;

namespace UpyunAction
{
    public class UpyunApi
    {
        public string BaseApiUrl { get; set; } = "https://api.upyun.com";

        public string Token { get; set; }

        public bool CacheBatchRefresh(string urls)
        {
            string url = $"{BaseApiUrl}/buckets/purge/batch";
            string reqJsonStr = Utils.JsonUtil.Obj2JsonStr(new
            {
                noif = 1, // 强制刷新
                source_url = urls,
            });
            // Authorization: Bearer <Token>
            string[] headers = new string[] {
                $"Authorization: Bearer {this.Token}",
                "Content-Type: application/json"
            };
            string resJsonStr = string.Empty;
            try
            {
                resJsonStr = Utils.HttpUtil.HttpPost(url: url, postDataStr: reqJsonStr, headers: headers);
            }
            catch (Exception ex)
            {
                Utils.LogUtil.Exception(ex);
            }

            if (!resJsonStr.Contains("\"code\": 1"))
            {
                // 刷新失败
                Utils.LogUtil.Error("刷新失败:");
                Utils.LogUtil.Error(resJsonStr);

                return false;
            }
            CacheBatchRefreshItemResponseModel[] responseModel = Utils.JsonUtil.JsonStr2Obj<CacheBatchRefreshItemResponseModel[]>(resJsonStr);
            Utils.LogUtil.Info("刷新成功");
            foreach (var item in responseModel)
            {
                Utils.LogUtil.Info($"{item.status}: {item.url}");
            }

            return true;
        }

        /// <summary>
        /// 缓存批量刷新
        /// </summary>
        public bool CacheBatchRefresh(string[] urls)
        {
            string urlsStr = string.Join(@"\n", urls); // 多条使用换行符(\n)分割开

            return CacheBatchRefresh(urls);
        }






    }
}
