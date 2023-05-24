using System.IO;
using System.Threading;

namespace HLSStreamReader.CameraStream.StreamReaders
{
    internal class CancellableStream : Stream
    {
        private readonly Stream stream;
        private readonly CancellationToken cancellationToken;

        public CancellableStream(Stream stream, CancellationToken cancellationToken)
        {
            this.stream = stream;
            this.cancellationToken = cancellationToken;
        }

        public override bool CanRead => stream.CanRead;

        public override bool CanSeek => stream.CanSeek;

        public override bool CanWrite => stream.CanWrite;

        public override long Length => stream.Length;

        public override long Position
        {
            get => stream.Position;
            set => stream.Position = value;
        }

        public override void Flush()
        {
            stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }
    }
}
