using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HLSStreamReader.CameraStream.StreamReaders
{
    public interface ICameraStreamReader
    {
        #region Properties

        public CancellationToken CancellationToken { get; }

        public ConcurrentQueue<byte> StreamData { get; }

        public int BufferSize { get; set; }

        #endregion

        Task StartReadStream(int width = 640, int height = 480);
        void StopReadStream();

    }
}