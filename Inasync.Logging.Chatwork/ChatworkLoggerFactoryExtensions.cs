using System;
using Inasync.Logging.Chatwork;
using Microsoft.Extensions.Logging;

namespace Inasync {

    /// <summary>
    /// Chatwork ロガーを構成するヘルパー クラス。
    /// </summary>
    public static class ChatworkLoggerFactoryExtensions {

        /// <summary>
        /// Chatwork ロガーを構成します。
        /// </summary>
        /// <param name="builder">ログ構成ビルダー。</param>
        /// <returns><paramref name="builder"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
        public static ILoggingBuilder AddChatworkLogger(this ILoggingBuilder builder) {
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }

            builder.AddLogger<ChatworkLoggerProvider, ChatworkLoggerOptions>(defaultMinLevel: LogLevel.Warning);

            return builder;
        }
    }
}
