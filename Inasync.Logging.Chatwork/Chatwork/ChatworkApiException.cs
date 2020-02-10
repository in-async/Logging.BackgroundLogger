using System;
using System.Runtime.Serialization;

namespace Inasync.Logging.Chatwork {

    /// <summary>
    /// Chatwork API への接続中に生じた例外を表すクラス。
    /// </summary>
    [Serializable]
    public class ChatworkApiException : Exception {
        private const string _defaultMessage = "Chatwork API への接続中にエラーが生じました。";

        /// <summary>
        /// <see cref="ChatworkApiException"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public ChatworkApiException() : this(message: _defaultMessage) {
        }

        /// <summary>
        /// 指定したエラー メッセージを使用して、<see cref="ChatworkApiException"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="message">エラーを説明するメッセージ。</param>
        public ChatworkApiException(string message) : base(message ?? _defaultMessage) {
        }

        /// <summary>
        /// 指定したエラー メッセージおよびこの例外の原因となった内部例外への参照を使用して、<see cref="ChatworkApiException"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="message">例外の原因を説明するエラー メッセージ。</param>
        /// <param name="innerException">現在の例外の原因である例外。内部例外が指定されていない場合は <c>null</c> 参照。</param>
        public ChatworkApiException(string message, Exception innerException) : base(message ?? _defaultMessage, innerException) {
        }

        /// <summary>
        /// シリアル化したデータを使用して、<see cref="ChatworkApiException"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="info">スローされている例外に関するシリアル化済みオブジェクト データを保持している <see cref="SerializationInfo"/>。</param>
        /// <param name="context">転送元または転送先についてのコンテキスト情報を含む <see cref="StreamingContext"/>。</param>
        protected ChatworkApiException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}
