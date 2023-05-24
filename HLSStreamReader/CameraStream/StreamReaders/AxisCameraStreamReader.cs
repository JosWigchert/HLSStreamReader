namespace HLSStreamReader.CameraStream.StreamReaders
{
    using OpenCvSharp;

    public class AxisCameraStreamReader : CameraStreamReader
    {
        public AxisCameraStreamReader(string cameraUrl)
            : base(cameraUrl)
        {
        }

        protected override string GetStreamUrl(Size size)
        {
            return $"{_cameraUrl}/axis-cgi/mjpg/video.cgi?resolution={size.Width}x{size.Height}";
        }
    }
}
