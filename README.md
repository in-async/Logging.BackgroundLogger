# Logging.Chatwork
[![Build status](https://ci.appveyor.com/api/projects/status/cb2wao89os87g89r/branch/master?svg=true)](https://ci.appveyor.com/project/inasync/logging-chatwork/branch/master)
[![NuGet](https://img.shields.io/nuget/v/Inasync.Logging.Chatwork.svg)](https://www.nuget.org/packages/Inasync.Logging.Chatwork/)

***Logging.Chatwork*** は、Chatwork の任意のルームにログ メッセージを投稿する [ILoggerProvider](https://docs.microsoft.com/ja-jp/aspnet/core/fundamentals/logging/) 実装を提供するライブラリです。


## Target Frameworks
- .NET Standard 2.0
- .NET Standard 2.1

## Usage
appsettings.json
```js
{
  "Logging": {
    ...
    "Chatwork": {
      // Default LogLevel is warning.
      //"LogLevel": {
      // "Default": "Warning"
      //},
      "ApiToken"           : "Required: API Token",
      "RoomId"             : "Required: Room ID to which log messages are posted",
      "HeaderText"         : "Optional: Text to be inserted in the log message header",
      "BackgroundQueueSize": "Optional: The size of the background queue for posting messages (default: 1024)"
    }
  },
  ...
}
```

For using `IHostBuilder`:
```cs
hostBuilder.ConfigureLogging(logging => {
    logging.AddChatworkLogger();
});
```

For using `ILoggerFactory`:
```cs
var loggerFactory = LoggerFactory.Create(builder => {
    builder.AddChatworkLogger();
});
```
