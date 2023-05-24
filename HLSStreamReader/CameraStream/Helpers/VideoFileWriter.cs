using OpenCvSharp;


namespace HLSStreamReader.CameraStream.Helpers
{
    public class VideoFileWriter
    {
        private VideoWriter writer;
        private string filename;
        private int fps;
        private Size size;
        private FourCC codec;

        public VideoFileWriter(string filename, int fps, Size frameSize, FourCC codec)
        {
            this.filename = filename;
            this.fps = fps;
            size = frameSize;
            this.codec = codec;

            writer = new VideoWriter();
        }

        public void Open()
        {
            writer.Open(filename, codec, fps, size, false);
        }

        public void WriteFrame(Mat frame)
        {
            if (writer.IsOpened())
                writer.Write(frame);
        }

        public void Release()
        {
            writer.Release();
        }
    }
}
