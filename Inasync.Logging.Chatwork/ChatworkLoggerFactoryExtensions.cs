using System;
using Inasync.Logging.Chatwork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inasync {

    /// <summary>
    /// Chatwork ロガーを構成するヘルパー クラス。
    /// </summary>
    public static class ChatworkLoggerFactoryExtensions {

        /// <summary>
        /// `Chatwork` という名前の Chatwork ロガーを構成します。
        /// </summary>
        /// <param name="builder">使用するログ構成ビルダー。</param>
        /// <returns><paramref name="builder"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
        public static ILoggingBuilder AddChatworkLogger(this ILoggingBuilder builder) {
            builder.AddLogger<ChatworkLoggerProvider, ChatworkLoggerOptions>(defaultMinLevel: LogLevel.Warning);

            return builder;
        }

        /// <summary>
        /// `Chatwork` という名前の Chatwork ロガーを構成します。
        /// </summary>
        /// <param name="builder">使用するログ構成ビルダー。</param>
        /// <param name="configure"><see cref="ChatworkLoggerProvider"/> を構成するデリゲート。</param>
        /// <returns><paramref name="builder"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="configure"/> is <c>null</c>.</exception>
        public static ILoggingBuilder AddChatworkLogger(this ILoggingBuilder builder, Action<ChatworkLoggerOptions> configure) {
            if (configure is null) { throw new ArgumentNullException(nameof(configure)); }

            builder.AddChatworkLogger();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}
