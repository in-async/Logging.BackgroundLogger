using System;
using Microsoft.Extensions.Logging;

namespace Inasync.Logging {

    /// <summary>
    /// 汎用的なログ メッセージの構造体。
    /// </summary>
    public readonly struct LogMessage {

        /// <summary>
        /// カテゴリ名。
        /// </summary>
        public readonly string CategoryName;

        /// <summary>
        /// ログ レベル。
        /// </summary>
        public readonly LogLevel LogLevel;

        /// <summary>
        /// イベント ID。
        /// </summary>
        public readonly EventId EventId;

        /// <summary>
        /// ログの主要メッセージ。
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// 例外ログ。
        /// </summary>
        public readonly Exception? Exception;

        /// <summary>
        /// ログのタイムスタンプ。
        /// </summary>
        public readonly DateTimeOffset Timestamp;

        /// <summary>
        /// <see cref="LogMessage"/> 構造体の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="categoryName"><see cref="CategoryName"/> に渡される値。</param>
        /// <param name="logLevel"><see cref="LogLevel"/> に渡される値。</param>
        /// <param name="eventId"><see cref="EventId"/> に渡される値。</param>
        /// <param name="message"><see cref="Message"/> に渡される値。</param>
        /// <param name="exception"><see cref="Exception"/> に渡される値。</param>
        /// <param name="timestamp"><see cref="Timestamp"/> に渡される値。</param>
        public LogMessage(string categoryName, LogLevel logLevel, EventId eventId, string message, Exception? exception, DateTimeOffset timestamp) {
            CategoryName = categoryName;
            LogLevel = logLevel;
            EventId = eventId;
            Message = message;
            Exception = exception;
            Timestamp = timestamp;
        }
    }
}
