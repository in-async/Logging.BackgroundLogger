using System;

namespace Inasync.Logging {

    internal sealed class NullScope : IDisposable {
        public static NullScope Instance { get; } = new NullScope();

        private NullScope() {
        }

        /// <inheritdoc />
        public void Dispose() {
        }
    }
}
