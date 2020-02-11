using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Inasync {

    /// <summary>
    /// 元となるコレクション内からアイテムを取り出すコレクションを表します。
    /// </summary>
    /// <typeparam name="T">コレクション内のアイテムの型</typeparam>
    public interface IBlockingConsumerCollection<T> {

        /// <summary>
        /// 指定した期間内に、<see cref="IBlockingConsumerCollection{T}"/> からアイテムを削除して返そうと試みます。
        /// </summary>
        /// <param name="item">
        /// コレクションが空でない場合、コレクションからアイテムを削除して <paramref name="item"/> に格納されます。
        /// コレクションが空の場合は <c>default</c> が格納されます。
        /// </param>
        /// <param name="timeout">待機するミリ秒数。無制限に待機する場合は <see cref="Timeout.Infinite"/> (-1)。</param>
        /// <param name="cancellationToken">待機に対するキャンセル トークン。</param>
        /// <returns>アイテムが正常に削除されて返された場合は <c>true</c>。それ以外の場合は <c>false</c>。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> は無限のタイムアウトを表す -1 以外の負の数です。</exception>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> へのキャンセル要求に従い、操作が中止されました。</exception>
        bool TryTake(out T item, int timeout = 0, CancellationToken cancellationToken = default);

        /// <summary>
        /// コレクション内の項目を取り出す <see cref="IEnumerable{T}"/> を提供します。
        /// <para>
        /// コレクションが空の場合はアイテムが追加されるまで列挙操作はブロックされます。
        /// 元となるコレクションがアイテムの追加を完了するか操作がキャンセルされてもブロックは解除されます。
        /// </para>
        /// </summary>
        /// <param name="cancellationToken">待機に対するキャンセル トークン。</param>
        /// <returns>コレクションから項目を削除して返す <see cref="IEnumerable{T}"/>。</returns>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> へのキャンセル要求に従い、操作が中止されました。</exception>
        IEnumerable<T> GetConsumingEnumerable(CancellationToken cancellationToken = default);
    }

    internal sealed class BlockingConsumerCollection<T> : IBlockingConsumerCollection<T> {
        private readonly BlockingCollection<T> _source;

        public BlockingConsumerCollection(BlockingCollection<T> source) {
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public bool TryTake(out T item, int timeout = 0, CancellationToken cancellationToken = default) {
            return _source.TryTake(out item, timeout, cancellationToken);
        }

        public IEnumerable<T> GetConsumingEnumerable(CancellationToken cancellationToken = default) {
            return _source.GetConsumingEnumerable(cancellationToken);
        }
    }
}
