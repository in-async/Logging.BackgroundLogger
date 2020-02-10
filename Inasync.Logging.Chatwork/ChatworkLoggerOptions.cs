using System;

namespace Inasync.Logging.Chatwork {

    /// <summary>
    /// Chatwork ロガーの設定。
    /// </summary>
    public class ChatworkLoggerOptions {

        /// <summary>
        /// Chatwork API のアクセス トークン。
        /// <para>Not null.</para>
        /// </summary>
        public string ApiToken { get; set; }

        /// <summary>
        /// ログの送信先ルーム ID。
        /// <para>Not null.</para>
        /// </summary>
        public string RoomId { get; set; }

        /// <summary>
        /// ログのヘッダー テキスト。
        /// </summary>
        public string HeaderText { get; set; }

        /// <summary>
        /// ログ テキストを生成するデリゲート。
        /// </summary>
        public Func<LogMessage, string> LogMessageFormatter { get; set; }

        /// <summary>
        /// バックグラウンド キューの最大サイズ。
        /// </summary>
        public int BackgroundQueueSize {
            get => _backgroundQueueSize;
            set {
                if (value <= 0) { throw new ArgumentOutOfRangeException(nameof(value), value, null); }
                _backgroundQueueSize = value;
            }
        }

        private int _backgroundQueueSize = 1024;
    }
}
