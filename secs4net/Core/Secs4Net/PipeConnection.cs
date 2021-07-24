﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Secs4Net
{
    public sealed class PipeConnection : IHsmsConnection
    {
        bool IHsmsConnection.LinkTestEnabled { get; set; }

        private readonly PipeDecoder _decoder;
        public PipeConnection(PipeDecoder pipeDecoder)
        {
            _decoder = pipeDecoder;
            AsyncHelper.LongRunningAsync(() => _decoder.StartAsync(CancellationToken.None));
        }

        async ValueTask<int> IHsmsConnection.SendAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken)
        {
            // assume the 'PipeDecoder.Input' here is another connector's input
            var result = await _decoder.Input.WriteAsync(source, cancellationToken);
            return source.Length;
        }

        IAsyncEnumerable<SecsMessage> IHsmsConnection.GetDataMessages(CancellationToken cancellation)
            => _decoder.GetDataMessages(cancellation);
    }
}
