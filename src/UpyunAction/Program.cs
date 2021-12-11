// See https://aka.ms/new-console-template for more information

using UpyunAction.ResponseModels;

namespace UpyunAction // Note: actual namespace depends on the project name.
{
    public class Program
    {
        /// <summary>
        /// 在本次 GitHub Action 执行中 所创建使用的 token 的 name
        /// </summary>
        public const string UpyunTokenName = "upyun-action";

        public const string EnvKeyDebug = "upyun_debug";
        public const string EnvKeyUpyunUserName = "upyun_username";
        public const string EnvKeyUpyunPassword = "upyun_password";
        public const string EnvKeyRefreshCacheUrls = "refresh_cache_urls";

        public static UpyunApi UpyunApi { get; set; }

        public static void Main(string[] args)
        {
            Utils.LogUtil.Info($"Hello upyun-action!");

            #region Debug

            string debugStr = Utils.GitHubActionsUtil.GetEnv(EnvKeyDebug);
            bool debug = false;
            if (!string.IsNullOrEmpty(debugStr) && Boolean.TryParse(debugStr, out debug) && debug)
            {
                Utils.GitHubActionsUtil.SetOutput("upyun_response", "strawberry");
            }

            if (debug)
            {
                Utils.LogUtil.Info($"args: {string.Join(", ", args)}");

                var envKeys = Environment.GetEnvironmentVariables().Keys;
                foreach (var key in envKeys)
                {
                    Utils.LogUtil.Info($"Environment: {key}");
                }
            }

            #endregion

            #region 任务

            #region 初始化
            UpyunApi = new UpyunApi();
            // 1. 创建 token
            string userName = Utils.GitHubActionsUtil.GetEnv(EnvKeyUpyunUserName);
            string password = Utils.GitHubActionsUtil.GetEnv(EnvKeyUpyunPassword);
            CreateTokenResponseModel createTokenResponseModel = null;
            string upyunTokenName = string.Empty;
            for (int i = 1; i <= 10; i++)
            {
                // 尝试 10 次, 因为 又拍云只允许创建10个token
                upyunTokenName = $"{UpyunTokenName}-{i}";
                createTokenResponseModel = UpyunApi.CreateTokenAsync(userName, password, upyunTokenName, "global").Result;
                if (createTokenResponseModel != null)
                {
                    // 成功则 break
                    Utils.LogUtil.Info($"成功创建相关token: {upyunTokenName}");
                    break;
                }
            }
            if (createTokenResponseModel == null)
            {
                return;
            }
            // 赋值 Token
            UpyunApi.Token = createTokenResponseModel.access_token;
            #endregion

            #region 任务: 刷新缓存
            string refreshCacheUrls = Utils.GitHubActionsUtil.GetEnv(EnvKeyRefreshCacheUrls);
            if (!string.IsNullOrEmpty(refreshCacheUrls))
            {
                // 3. 刷新缓存
                CacheBatchRefreshItemResponseModel[] responseModel = UpyunApi.CacheBatchRefreshAsync(refreshCacheUrls).Result;
                foreach (var item in responseModel)
                {
                    Utils.LogUtil.Info($"{item.status}: {item.url}");
                }
            }
            #endregion

            // 任务完成: 删除 token
            DeleteToken();

            #endregion

            Utils.LogUtil.Info($"GoodBye upyun-action!");
        }

        #region DeleteToken
        public static void DeleteToken()
        {
            Utils.LogUtil.Info("任务完成: 开始回收 (删除) 相关token");
            // 获取 token 信息
            var tokenListModel = UpyunApi.TokenListAsync().Result;
            if (tokenListModel == null)
            {
                return;
            }
            foreach (var item in tokenListModel)
            {
                if (item.name.StartsWith(UpyunTokenName))
                {
                    // 删除所有 与 upyun-action 相关 token
                    var deleteModel = UpyunApi.DeleteTokenAsync(item.name).Result;
                    if (deleteModel.result)
                    {
                        Utils.LogUtil.Info($"成功: 删除 token: {item.name}");
                    }
                    else
                    {
                        Utils.LogUtil.Info($"失败: 删除 token: {item.name}");
                    }
                }
            }
        }
        #endregion


    }
}