using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace Inasync {

    /// <summary>
    /// ロガーを構成する為のヘルパー クラス。
    /// </summary>
    public static class LoggingBuilderExtensions {

        /// <summary>
        /// バックグラウンド ロガーを構成します。
        /// </summary>
        /// <typeparam name="TProvider">構成するロガー プロバイダの型。</typeparam>
        /// <param name="builder">ログ構成ビルダー。</param>
        /// <param name="defaultMinLevel">ロガーの既定の最小ログ レベル</param>
        /// <returns><paramref name="builder"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
        public static ILoggingBuilder AddLogger<TProvider>(this ILoggingBuilder builder, LogLevel? defaultMinLevel = null)
            where TProvider : class, ILoggerProvider {
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }

            builder.AddConfiguration();

            if (defaultMinLevel != null) {
                builder.Services.Configure<LoggerFilterOptions>(options => {
                    var defaultRule = new LoggerFilterRule(
                         providerName: typeof(TProvider).FullName
                       , categoryName: null
                       , logLevel: defaultMinLevel
                       , filter: null
                    );
                    options.Rules.Insert(0, defaultRule);
                });
            }
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, TProvider>());

            return builder;
        }

        /// <summary>
        /// バックグラウンド ロガーを構成します。
        /// </summary>
        /// <typeparam name="TProvider">構成するロガー プロバイダの型。</typeparam>
        /// <typeparam name="TOptions">ロガー オプションの型。</typeparam>
        /// <param name="builder">ログ構成ビルダー。</param>
        /// <param name="defaultMinLevel">ロガーの既定の最小ログ レベル</param>
        /// <returns><paramref name="builder"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
        public static ILoggingBuilder AddLogger<TProvider, TOptions>(this ILoggingBuilder builder, LogLevel? defaultMinLevel = null)
            where TProvider : class, ILoggerProvider
            where TOptions : class {
            builder.AddLogger<TProvider>(defaultMinLevel);
            LoggerProviderOptions.RegisterProviderOptions<TOptions, TProvider>(builder.Services);

            return builder;
        }
    }
}
