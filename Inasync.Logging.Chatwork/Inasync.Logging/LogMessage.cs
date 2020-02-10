using System;
using Microsoft.Extensions.Logging;

namespace Inasync.Logging {

    public readonly struct LogMessage {
        public readonly string CategoryName;
        public readonly LogLevel LogLevel;
        public readonly EventId EventId;
        public readonly string Message;
        public readonly Exception Exception;
        public readonly DateTimeOffset Timestamp;

        public LogMessage(string categoryName, LogLevel logLevel, EventId eventId, string message, Exception exception, DateTimeOffset timestamp) {
            CategoryName = categoryName;
            LogLevel = logLevel;
            EventId = eventId;
            Message = message;
            Exception = exception;
            Timestamp = timestamp;
        }
    }
}
