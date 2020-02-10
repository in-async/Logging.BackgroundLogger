using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Inasync.Logging.Chatwork {

    /// <summary>
    /// <see cref="ILoggerProvider"/> の Chatwork 実装。
    /// </summary>
    [ProviderAlias("Chatwork")]
    public sealed class ChatworkLoggerProvider : ILoggerProvider, IAsyncDisposable {
        private readonly ChatworkLogMessageFormatter _formatter;
        private readonly ChatworkMessageApi _chatworkApi;
        private readonly MessageProcessor<LogMessage> _processor;

        /// <summary>
        /// <see cref="ChatworkLoggerProvider"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="optionsMonitor">ロガーの設定。</param>
        public ChatworkLoggerProvider(IOptionsMonitor<ChatworkLoggerOptions> optionsMonitor) {
            var options = optionsMonitor?.CurrentValue ?? throw new ArgumentNullException(nameof(optionsMonitor));

            _formatter = new ChatworkLogMessageFormatter(options.LogMessageFormatter, headerText: options.HeaderText);
            _chatworkApi = new ChatworkMessageApi(apiToken: options.ApiToken, roomId: options.RoomId);
            _processor = new MessageProcessor<LogMessage>(consumer: WriteMessage, queueSize: options.BackgroundQueueSize);
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName) {
            return new LogMessageLogger(categoryName, _processor);
        }

        /// <inheritdoc />
        public void Dispose() => DisposeAsync().GetAwaiter().GetResult();

        /// <inheritdoc />
        public async ValueTask DisposeAsync() {
            await _processor.DisposeAsync().ConfigureAwait(false);
            _chatworkApi.Dispose();
        }

        private void WriteMessage(LogMessage message) {
            string text = _formatter.Invoke(message);
            Debug.Assert(text != null);

            try {
                _chatworkApi.InsertAsync(text, CancellationToken.None).GetAwaiter().GetResult();
            }
            catch (ChatworkApiException) { }
        }
    }
}
