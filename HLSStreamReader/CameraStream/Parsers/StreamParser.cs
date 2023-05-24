using OpenCvSharp;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace HLSStreamReader.CameraStream.Parsers
{
    public abstract class StreamParser : IStreamParser
    {
        public abstract IAsyncEnumerable<Mat> Parse(ConcurrentQueue<byte> bytes, CancellationToken cancellationToken);
    }
}
