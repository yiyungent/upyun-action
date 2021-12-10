// See https://aka.ms/new-console-template for more information

namespace UpyunAction // Note: actual namespace depends on the project name.
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Utils.LogUtil.Info($"Hello upyun-action!");

            #region Debug
            Utils.LogUtil.Info($"args: {string.Join(", ", args)}");

            var envKeys = Environment.GetEnvironmentVariables().Keys;
            foreach (var key in envKeys)
            {
                Utils.LogUtil.Info($"Environment: {key}");
            }

            // 注意: 当没有这个环境变量时, 不会报错, 而是返回空字符串
            //WriteLine($"Environment: INPUT_UPYUN_TOKEN: {Environment.GetEnvironmentVariable("INPUT_UPYUN_TOKEN")}");
            //Console.WriteLine("::set-output name=upyun_response::strawberry"); 

            #endregion

            UpyunApi upyunApi = new UpyunApi();
            string upyunToken = Utils.GitHubActionsUtil.GetEnv("upyun_token");
            if (!string.IsNullOrEmpty(upyunToken))
            {
                #region Test Action
                if (upyunToken == "test-token")
                {
                    Utils.GitHubActionsUtil.SetOutput("upyun_response", "strawberry");
                }
                #endregion

                upyunApi.Token = upyunToken;
            }

            string refreshCacheUrls = Utils.GitHubActionsUtil.GetEnv("refresh_cache_urls");
            if (!string.IsNullOrEmpty(refreshCacheUrls))
            {
                // 刷新缓存
                upyunApi.CacheBatchRefresh(refreshCacheUrls);
            }

        }

    }
}