using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Inasync.Logging {

    /// <summary>
    /// ログをメッセージに変換し、<see cref="MessageProcessor{TMessage}"/> のキューに追加するロガー。
    /// </summary>
    public sealed class LogMessageLogger : ILogger {
        private readonly string _categoryName;
        private readonly MessageProcessor<LogMessage> _messageProcessor;

        /// <summary>
        /// <see cref="LogMessageLogger"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="categoryName">カテゴリ名。</param>
        /// <param name="messageProcessor">ログ メッセージをバックグラウンドで処理するメッセージ プロセッサー。</param>
        public LogMessageLogger(string categoryName, MessageProcessor<LogMessage> messageProcessor) {
            _categoryName = categoryName ?? throw new ArgumentNullException(nameof(categoryName));
            _messageProcessor = messageProcessor ?? throw new ArgumentNullException(nameof(messageProcessor));
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state) {
            return NullScope.Instance;
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel) {
            if (logLevel == LogLevel.None) { return false; }

            return true;
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
            if (formatter == null) { throw new ArgumentNullException(nameof(formatter)); }
            if (!IsEnabled(logLevel)) { return; }

            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message) && exception == null) { return; }

            _messageProcessor.TryPost(new LogMessage(_categoryName, logLevel, eventId, message, exception, DateTimeOffset.Now), timeout: Timeout.Infinite);
        }
    }
}
