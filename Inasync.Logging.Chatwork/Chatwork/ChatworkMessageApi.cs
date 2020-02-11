using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inasync.Logging.Chatwork {

    /// <summary>
    /// Chatwork API とメッセージに関するやり取りを行います。
    /// </summary>
    public sealed class ChatworkMessageApi {
        private readonly string _apiToken;
        private readonly string _roomId;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// <see cref="ChatworkMessageApi"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="apiToken">API トークン。</param>
        /// <param name="roomId">メッセージの対象となるルーム ID。</param>
        /// <param name="httpClient">Chatwork API との通信に使用する HTTP クライアント。</param>
        /// <exception cref="ArgumentNullException"><paramref name="apiToken"/> or <paramref name="roomId"/> or <paramref name="httpClient"/> is <c>null</c>.</exception>
        public ChatworkMessageApi(string apiToken, string roomId, HttpClient httpClient) {
            _apiToken = apiToken ?? throw new ArgumentNullException(nameof(apiToken));
            _roomId = roomId ?? throw new ArgumentNullException(nameof(roomId));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// ルームにメッセージを投稿します。
        /// </summary>
        /// <param name="message">投稿されるメッセージ。</param>
        /// <param name="cancellationToken">キャンセル トークン。</param>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> is <c>null</c>.</exception>
        /// <exception cref="ChatworkApiException">Chatwork API との接続中にエラーが生じました。</exception>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> へのキャンセル要求に従い、操作が中止されました。</exception>
        public async Task InsertAsync(string message, CancellationToken cancellationToken) {
            if (message == null) { throw new ArgumentNullException(nameof(message)); }

            var endpoint = "https://api.chatwork.com/v2/rooms/" + _roomId + "/messages";
            var escapedMessage = Uri.EscapeDataString(message);

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint) {
                Headers = { { "X-ChatWorkToken", _apiToken } },
                Content = new ByteArrayContent(Encoding.ASCII.GetBytes("body=" + escapedMessage)) {
                    Headers = { ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded") },
                }
            };
            try {
                using var res = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                res.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex) { throw new ChatworkApiException(message: null, ex); }
        }
    }
}
