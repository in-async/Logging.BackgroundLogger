using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Inasync.Logging.Chatwork {

    /// <summary>
    /// Chatwork に投稿する <see cref="LogMessage"/> の文字列フォーマッター。
    /// </summary>
    public sealed class ChatworkLogMessageFormatter {
        private readonly Func<LogMessage, string>? _formatter;
        private readonly string? _headerText;

        /// <summary>
        /// <see cref="ChatworkLogMessageFormatter"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="formatter">優先される <see cref="LogMessage"/> の文字列フォーマッター。<c>null</c> の場合は既定のフォーマットが行われます。</param>
        /// <param name="headerText">既定のフォーマットで使用されるヘッダー テキスト。</param>
        public ChatworkLogMessageFormatter(Func<LogMessage, string>? formatter, string? headerText) {
            _formatter = formatter;
            _headerText = headerText;
        }

        /// <summary>
        /// 指定した <see cref="LogMessage"/> を文字列にフォーマットします。
        /// </summary>
        /// <param name="message">フォーマット対象のメッセージ。</param>
        /// <returns><paramref name="message"/> をフォーマットしてできた文字列。</returns>
        public string Invoke(LogMessage message) {
            if (_formatter != null) {
                return _formatter(message);
            }

            return CreateLogText(message);
        }

        private string CreateLogText(LogMessage message) {
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(_headerText)) {
                builder.Append(_headerText);
            }
            builder.Append('\n');
            builder.Append("[info][title]");
            builder.Append(GetShortName(message.LogLevel));
            builder.Append(": ");
            builder.Append(message.CategoryName);
            builder.Append(" [");
            builder.Append(message.EventId);
            builder.Append("][/title]");
            builder.Append(message.Timestamp.ToString("yyyy-MM-dd HH:mm:ss zzz"));
            builder.Append('\n');
            builder.Append(message.Message);
            if (message.Exception is Exception ex) {
                builder.Append('\n');
                builder.Append(ex.ToString());
            }
            builder.Append("[/info]");

            return builder.ToString();
        }

        private static string GetShortName(LogLevel logLevel) {
            return logLevel switch
            {
                LogLevel.Trace => "trce",
                LogLevel.Debug => "dbug",
                LogLevel.Information => "info",
                LogLevel.Warning => "warn",
                LogLevel.Error => "fail",
                LogLevel.Critical => "crit",
                _ => Enum.GetName(typeof(LogLevel), logLevel),
            };
        }
    }
}
