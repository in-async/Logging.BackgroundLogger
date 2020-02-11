using System;
using System.Net.Http;
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
        private static readonly HttpClient _defaultHttpClient = new HttpClient();
        private readonly ChatworkLogMessageFormatter _formatter;
        private readonly ChatworkMessageApi _chatworkApi;
        private readonly MessageProcessor<LogMessage> _processor;

        /// <summary>
        /// <see cref="ChatworkLoggerProvider"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="optionsMonitor">ロガーの設定。</param>
        /// <param name="httpClient">Chatwork API との通信に使用する HTTP クライアント。<c>null</c> の場合は既定の <see cref="HttpClient"/> で代用されます。</param>
        /// <exception cref="ArgumentNullException"><see cref="ChatworkLoggerOptions.ApiToken"/> or <see cref="ChatworkLoggerOptions.RoomId"/> is <c>null</c>.</exception>
        public ChatworkLoggerProvider(IOptionsMonitor<ChatworkLoggerOptions> optionsMonitor, HttpClient? httpClient = null) {
            var options = optionsMonitor?.CurrentValue ?? throw new ArgumentNullException(nameof(optionsMonitor));
            if (options.ApiToken == null) { throw new ArgumentNullException(nameof(options.ApiToken)); }
            if (options.RoomId == null) { throw new ArgumentNullException(nameof(options.RoomId)); }

            _formatter = new ChatworkLogMessageFormatter(options.LogMessageFormatter, headerText: options.HeaderText);
            _chatworkApi = new ChatworkMessageApi(apiToken: options.ApiToken, roomId: options.RoomId, httpClient ?? _defaultHttpClient);
            _processor = new MessageProcessor<LogMessage>(consumer: WriteMessages, queueSize: options.BackgroundQueueSize);
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
        }

        private void WriteMessages(IBlockingConsumerCollection<LogMessage> messages) {
            foreach (var message in messages.GetConsumingEnumerable()) {
                string text = _formatter.Invoke(message);

                try {
                    _chatworkApi.InsertAsync(text, CancellationToken.None).GetAwaiter().GetResult();
                }
                catch (ChatworkApiException) { }
            }
        }
    }
}
