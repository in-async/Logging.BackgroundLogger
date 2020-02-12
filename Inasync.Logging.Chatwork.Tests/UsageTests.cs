using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inasync.Logging.Chatwork.Tests {

    [TestClass]
    public class UsageTests {

        [TestMethod]
        public void Usage() {
            using var loggerFactory = LoggerFactory.Create(builder => {
                // Chatwork Logger を追加。
                builder.AddChatworkLogger(options => {
                    // Chatwork Logger の設定。ここではコードで設定しているが、通常は appsettings.json から設定する方が簡単。
                    options.ApiToken = "API Token";
                    options.RoomId = "RoomID";
                });

                // テストなので、実際に HTTP リクエストが送信されないよう、ダミーの HttpClient をインジェクション。
                builder.Services.AddSingleton(new HttpClient(new SpyHttpMessageHandler()));
            });

            ILogger logger = loggerFactory.CreateLogger<UsageTests>();
            logger.LogInformation("既定の最小ログ レベルは Warning なので、Information レベルはログされません。");
            logger.LogWarning("このメッセージは Warning レベルなのでログされます。");
        }

        [TestMethod]
        public void Usage_LogInformation() {
            // Arrange
            var handler = new SpyHttpMessageHandler();
            using (var loggerFactory = LoggerFactory.Create(builder => {
                builder.Services.AddSingleton(new HttpClient(handler));
                builder.Services.Configure<ChatworkLoggerOptions>(options => {
                    options.ApiToken = "API Token";
                    options.RoomId = "RoomID";
                });
                builder.AddChatworkLogger();
            })) {
                ILogger logger = loggerFactory.CreateLogger<UsageTests>();

                // Act
                logger.LogInformation("information log message");
            }

            // Assert
            Assert.AreEqual(default, handler.ActualRequest);
        }

        [TestMethod]
        public void Usage_LogWarning() {
            // Arrange
            var handler = new SpyHttpMessageHandler();
            using (var loggerFactory = LoggerFactory.Create(builder => {
                builder.AddChatworkLogger();
                builder.Services.Configure<ChatworkLoggerOptions>(options => {
                    options.ApiToken = "API Token";
                    options.RoomId = "RoomID";
                });
                builder.Services.AddSingleton(new HttpClient(handler));
            })) {
                ILogger logger = loggerFactory.CreateLogger<UsageTests>();

                // Act
                logger.LogWarning("warning log message");
            }

            // Assert
            Assert.AreEqual(HttpMethod.Post, handler.ActualRequest.request.Method);
            Assert.AreEqual("https://api.chatwork.com/v2/rooms/RoomID/messages", handler.ActualRequest.request.RequestUri.AbsoluteUri);
            CollectionAssert.AreEqual(new[] { "API Token" }, handler.ActualRequest.request.Headers.GetValues("X-ChatWorkToken").ToArray());
            Assert.AreEqual(new MediaTypeHeaderValue("application/x-www-form-urlencoded"), handler.ActualRequest.request.Content.Headers.ContentType);
            StringAssert.StartsWith(handler.ActualRequest.content, "body=");
        }

        [TestMethod]
        public void Usage_WithHeaderText() {
            // Arrange
            var handler = new SpyHttpMessageHandler();
            using (var loggerFactory = LoggerFactory.Create(builder => {
                builder.AddChatworkLogger();
                builder.Services.Configure<ChatworkLoggerOptions>(options => {
                    options.ApiToken = "API Token";
                    options.RoomId = "RoomID";
                    options.HeaderText = "Header Text";
                });
                builder.Services.AddSingleton(new HttpClient(handler));
            })) {
                ILogger logger = loggerFactory.CreateLogger<UsageTests>();

                // Act
                logger.LogWarning("warning log message");
            }

            // Assert
            Assert.AreEqual(HttpMethod.Post, handler.ActualRequest.request.Method);
            Assert.AreEqual("https://api.chatwork.com/v2/rooms/RoomID/messages", handler.ActualRequest.request.RequestUri.AbsoluteUri);
            CollectionAssert.AreEqual(new[] { "API Token" }, handler.ActualRequest.request.Headers.GetValues("X-ChatWorkToken").ToArray());
            Assert.AreEqual(new MediaTypeHeaderValue("application/x-www-form-urlencoded"), handler.ActualRequest.request.Content.Headers.ContentType);
            StringAssert.StartsWith(handler.ActualRequest.content, "body=Header%20Text");
        }

        [TestMethod]
        public void Usage_WithLogMessageFormatter() {
            // Arrange
            var handler = new SpyHttpMessageHandler();
            using (var loggerFactory = LoggerFactory.Create(builder => {
                builder.AddChatworkLogger();
                builder.Services.Configure<ChatworkLoggerOptions>(options => {
                    options.ApiToken = "API Token";
                    options.RoomId = "RoomID";
                    options.LogMessageFormatter = message => "Custom Message";
                });
                builder.Services.AddSingleton(new HttpClient(handler));
            })) {
                ILogger logger = loggerFactory.CreateLogger<UsageTests>();

                // Act
                logger.LogWarning("warning log message");
            }

            // Assert
            Assert.AreEqual(HttpMethod.Post, handler.ActualRequest.request.Method);
            Assert.AreEqual("https://api.chatwork.com/v2/rooms/RoomID/messages", handler.ActualRequest.request.RequestUri.AbsoluteUri);
            CollectionAssert.AreEqual(new[] { "API Token" }, handler.ActualRequest.request.Headers.GetValues("X-ChatWorkToken").ToArray());
            Assert.AreEqual(new MediaTypeHeaderValue("application/x-www-form-urlencoded"), handler.ActualRequest.request.Content.Headers.ContentType);
            Assert.AreEqual("body=Custom%20Message", handler.ActualRequest.content);
        }
    }
}
