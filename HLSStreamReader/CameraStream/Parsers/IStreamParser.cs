using OpenCvSharp;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace HLSStreamReader.CameraStream.Parsers
{
    public interface IStreamParser
    {
        IAsyncEnumerable<Mat> Parse(ConcurrentQueue<byte> bytes, CancellationToken cancellationToken);
    }
}