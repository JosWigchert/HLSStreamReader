using CircularBuffer;
using HLSStreamReader.CameraStream.Helpers;
using HLSStreamReader.CameraStream.Parsers;
using HLSStreamReader.CameraStream.StreamReaders;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FourCC = OpenCvSharp.FourCC;
using Mat = OpenCvSharp.Mat;

namespace HLSStreamReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ICameraStreamReader _reader;
        IStreamParser _parser;

        CircularBuffer<Mat> _circularBuffer;

        private int fps = 25;
        private int backlogTime = 30;

        public MainWindow()
        {
            InitializeComponent();

            _reader = new AxisCameraStreamReader("http://172.16.2.15");
            _parser = new MJPEGStreamParser();
            _circularBuffer = new CircularBuffer<Mat>(fps * backlogTime);
        }

        private async void StartButon_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Started...";

            Thread t = new Thread(async () => { await _reader.StartReadStream(1920, 1080); }) { IsBackground = true, Name = "Stream Reader" };
            t.Start();
            Thread t2 = new Thread(async () => { await AddToBuffer(); }) { IsBackground = true, Name = "Stream Parser" };
            t2.Start();
        }

        private async Task AddToBuffer()
        {
            await foreach (var item in _parser.Parse(_reader.StreamData, _reader.CancellationToken))
            {
                _circularBuffer.PushBack(item);
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _reader.StopReadStream();
            StatusText.Text = "Stopped and saved";

        }

        private void ConvertStream_Click(object sender, RoutedEventArgs e)
        {
            //MP4Converter.ConvertMjpegToMp4("output.mjpeg", "output_h264.mp4", FourCC.H264);
            //MP4Converter.ConvertMjpegToMp4("output.mjpeg", "output_h265.mp4", FourCC.H265);
            Thread t = new Thread(async () => { MP4Converter.CreateVideoFromFrames(_circularBuffer.ToArray(), fps, "output_" + DateTime.Now.TimeOfDay.TotalSeconds + ".mp4", FourCC.MP4V); }) { IsBackground = true, Name = "Stream Writer to file" };
            t.Start();
            //Thread t2 = new Thread(async () => { MP4Converter.ConvertMjpegToMp4("output.mjpeg", "output_h265.mp4", FourCC.H265); }) { IsBackground = true, Name = "Stream Writer to file" };
            //t2.Start();

        }
    }
}
