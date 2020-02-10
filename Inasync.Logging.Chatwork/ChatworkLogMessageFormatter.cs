using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Inasync.Logging {

    public sealed class ChatworkLogMessageFormatter {
        private readonly Func<LogMessage, string> _formatter;
        private readonly string _headerText;

        public ChatworkLogMessageFormatter(Func<LogMessage, string> formatter, string headerText) {
            _formatter = formatter;
            _headerText = headerText;
        }

        public string Invoke(LogMessage message) {
            if (_formatter != null) {
                return _formatter(message);
            }

            return CreateLogText(message);
        }

        private string CreateLogText(LogMessage message) {
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(_headerText)) {
                builder.Append(_headerText);
            }
            builder.Append('\n');
            builder.Append("[info][title]");
            builder.Append(GetShortName(message.LogLevel));
            builder.Append(": ");
            builder.Append(message.CategoryName);
            builder.Append(" [");
            builder.Append(message.EventId);
            builder.Append("][/title]");
            builder.Append(message.Timestamp.ToString("yyyy-MM-dd HH:mm:ss zzz"));
            builder.Append('\n');
            builder.Append(message.Message);
            if (message.Exception is Exception ex) {
                builder.Append('\n');
                builder.Append(ex.ToString());
            }
            builder.Append("[/info]");

            return builder.ToString();
        }

        private static string GetShortName(LogLevel logLevel) {
            switch (logLevel) {
                case LogLevel.Trace:
                    return "trce";

                case LogLevel.Debug:
                    return "dbug";

                case LogLevel.Information:
                    return "info";

                case LogLevel.Warning:
                    return "warn";

                case LogLevel.Error:
                    return "fail";

                case LogLevel.Critical:
                    return "crit";

                default:
                    return Enum.GetName(typeof(LogLevel), logLevel);
            }
        }
    }
}
