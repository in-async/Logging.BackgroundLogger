using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Inasync {

    /// <summary>
    /// キューに追加されたメッセージをバックグラウンドで処理します。
    /// </summary>
    /// <typeparam name="TMessage">対象のメッセージ型。</typeparam>
    public sealed class MessageProcessor<TMessage> : IDisposable, IAsyncDisposable {
        private readonly Action<IBlockingConsumerCollection<TMessage>> _consumer;
        private readonly BlockingCollection<TMessage> _messageQueue;
        private readonly Task _consumerTask;
        private bool _disposed = false;

        /// <summary>
        /// 上限を指定せずに、<see cref="MessageProcessor{TMessage}"/> クラスの新しいインスタンスを初期化します。
        /// <para>メッセージ キューの処理はバックグラウンド スレッドにて直ちに開始されます。</para>
        /// </summary>
        /// <param name="consumer">メッセージを処理するデリゲート。</param>
        /// <exception cref="ArgumentNullException"><paramref name="consumer"/> is <c>null</c>.</exception>
        public MessageProcessor(Action<IBlockingConsumerCollection<TMessage>> consumer) : this(consumer, new BlockingCollection<TMessage>()) { }

        /// <summary>
        /// 上限を指定して、<see cref="MessageProcessor{TMessage}"/> クラスの新しいインスタンスを初期化します。
        /// <para>メッセージ キューの処理はバックグラウンド スレッドにて直ちに開始されます。</para>
        /// </summary>
        /// <param name="consumer">メッセージを処理するデリゲート。</param>
        /// <param name="queueSize">メッセージ キューのサイズ上限。</param>
        /// <exception cref="ArgumentNullException"><paramref name="consumer"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="queueSize"/> は正の値ではありません。</exception>
        public MessageProcessor(Action<IBlockingConsumerCollection<TMessage>> consumer, int queueSize) : this(consumer, new BlockingCollection<TMessage>(queueSize)) { }

        private MessageProcessor(Action<IBlockingConsumerCollection<TMessage>> consumer, BlockingCollection<TMessage> messageQueue) {
            _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
            _messageQueue = messageQueue;

            _consumerTask = Task.Run(ProcessMessageQueue);
        }

        /// <inheritdoc />
        public void Dispose() => DisposeAsync().GetAwaiter().GetResult();

        /// <inheritdoc />
        public ValueTask DisposeAsync() {
            if (_disposed) { return default; }

            _disposed = true;
            return DisposeAsyncCore();

            async ValueTask DisposeAsyncCore() {
                _messageQueue.CompleteAdding();
                await _consumerTask.ConfigureAwait(false);

                _messageQueue.Dispose();
            }
        }

        /// <summary>
        /// メッセージ キューに対して指定したメッセージの追加を試みます。
        /// </summary>
        /// <param name="message">対象のメッセージ。</param>
        /// <param name="timeout">待機するミリ秒数。無制限に待機する場合は <see cref="Timeout.Infinite"/> (-1)。</param>
        /// <param name="cancellationToken">待機に対するキャンセル トークン。</param>
        /// <returns>メッセージがキューに追加された場合は <c>true</c>。それ以外の場合は <c>false</c>。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> は無限のタイムアウトを表す -1 以外の負の数です。</exception>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> へのキャンセル要求に従い、操作が中止されました。</exception>
        /// <exception cref="ObjectDisposedException"><see cref="MessageProcessor{TMessage}"/> が破棄されています。</exception>
        public bool TryPost(TMessage message, int timeout = 0, CancellationToken cancellationToken = default) {
            if (_disposed) { throw new ObjectDisposedException(nameof(MessageProcessor<TMessage>)); }
            if (_messageQueue.IsAddingCompleted) { return false; }

            try {
                _messageQueue.TryAdd(message, millisecondsTimeout: timeout, cancellationToken);
                return true;
            }
            catch (InvalidOperationException) { return false; }
        }

        private void ProcessMessageQueue() {
            _consumer(new BlockingConsumerCollection<TMessage>(_messageQueue));
        }
    }
}
