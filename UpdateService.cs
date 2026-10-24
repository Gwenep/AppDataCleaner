using System;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AppDataCleaner
{
    /// <summary>
    /// 新版本信息。
    /// </summary>
    internal sealed record UpdateInfo(Version Version, string TagName, string ReleaseUrl, string Notes);

    /// <summary>
    /// 通过 GitHub Releases 检查是否有新版本。
    /// 网络异常、限流、仓库未发布 Release 等情况一律视为「没有更新」，不打扰用户。
    /// </summary>
    internal static class UpdateService
    {
        private const string Owner = "Gwenep";
        private const string Repository = "AppDataCleaner";
        private const string LatestReleaseApi =
            "https://api.github.com/repos/" + Owner + "/" + Repository + "/releases/latest";

        public const string ReleasesUrl =
            "https://github.com/" + Owner + "/" + Repository + "/releases/latest";

        private static readonly Lazy<Version> CurrentVersionLazy = new(() =>
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);
            return Normalize(version);
        });

        /// <summary>当前程序版本（三段式，例如 0.3.0）。</summary>
        public static Version CurrentVersion => CurrentVersionLazy.Value;

        /// <summary>
        /// 查询最新 Release。发现更高版本时返回 <see cref="UpdateInfo"/>，否则返回 null。
        /// </summary>
        public static async Task<UpdateInfo?> CheckAsync(CancellationToken cancellationToken = default)
        {
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            http.DefaultRequestHeaders.UserAgent.ParseAdd($"AppDataCleaner/{CurrentVersion}");
            http.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");

            using var response = await http
                .GetAsync(LatestReleaseApi, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                AppLogger.Info($"检查更新：HTTP {(int)response.StatusCode}，跳过。");
                return null;
            }

            await using var stream = await response.Content
                .ReadAsStreamAsync(cancellationToken)
                .ConfigureAwait(false);

            using var document = await JsonDocument
                .ParseAsync(stream, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object ||
                !root.TryGetProperty("tag_name", out var tagElement))
            {
                AppLogger.Info("检查更新：返回内容缺少 tag_name，跳过。");
                return null;
            }

            string tagName = tagElement.GetString()?.Trim() ?? "";
            if (!TryParseTag(tagName, out var latest))
            {
                AppLogger.Info($"检查更新：无法解析版本标签 \"{tagName}\"，跳过。");
                return null;
            }

            if (latest <= CurrentVersion)
                return null;

            string releaseUrl = root.TryGetProperty("html_url", out var urlElement)
                ? urlElement.GetString() ?? ReleasesUrl
                : ReleasesUrl;

            string notes = root.TryGetProperty("body", out var bodyElement)
                ? bodyElement.GetString() ?? ""
                : "";

            return new UpdateInfo(latest, tagName, releaseUrl, notes);
        }

        /// <summary>把 "V0.3.1"、"v0.3.1-beta" 之类的标签解析为版本号。</summary>
        private static bool TryParseTag(string tagName, out Version version)
        {
            version = new Version(0, 0, 0);

            string text = tagName.Trim();
            if (text.StartsWith("v", StringComparison.OrdinalIgnoreCase))
                text = text[1..];

            // 去掉可能的预发布后缀（0.3.1-beta.1 -> 0.3.1）。
            int separator = text.IndexOfAny(new[] { '-', '+' });
            if (separator >= 0)
                text = text[..separator];

            if (!Version.TryParse(text, out var parsed))
                return false;

            version = Normalize(parsed);
            return true;
        }

        private static Version Normalize(Version version) =>
            new(version.Major, version.Minor, Math.Max(version.Build, 0));
    }
}
