using OpenCvSharp;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace HLSStreamReader.CameraStream.StreamReaders
{
    public abstract class CameraStreamReader : ICameraStreamReader
    {
        #region Protected Properties

        protected readonly string _cameraUrl;
        protected CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region Private Properties

        private ConcurrentQueue<byte> _streamData;
        private int _bufferSize;

        #endregion

        #region Public Properties

        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        public ConcurrentQueue<byte> StreamData
        {
            get { return _streamData; }
            private set { _streamData = value; }
        }

        public int BufferSize
        {
            get { return _bufferSize; }
            set { _bufferSize = value; }
        }

        #endregion

        public CameraStreamReader(string cameraUrl, int bufferSize = 4096)
        {
            _cameraUrl = cameraUrl;
            _bufferSize = bufferSize;
            _streamData = new ConcurrentQueue<byte>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartReadStream(int width = 640, int height = 480)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            Size size = new Size(width, height);
            await ReadStream(_cancellationTokenSource.Token, size);
        }

        public void StopReadStream()
        {
            _cancellationTokenSource?.Cancel();
        }

        protected async Task ReadStream(CancellationToken cancellationToken, Size imageSize)
        {
            //using (var stream = File.OpenWrite("video.mjpeg"))
            //{
            var streamUrl = await TestStreamUrl(imageSize);
            if (!string.IsNullOrEmpty(streamUrl))
            {
                using (var responseStream = GetResponseStream(streamUrl, cancellationToken))
                {
                    var buffer = new byte[BufferSize];
                    int bytesRead;
                    try
                    {
                        while ((bytesRead = responseStream.Read(buffer, 0, BufferSize)) > 0)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                break;
                            }


                            for (int i = 0; i < bytesRead; i++)
                            {
                                StreamData.Enqueue(buffer[i]);
                                //stream.WriteByte(buffer[i]);
                            }
                            //stream.Flush();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Exception in CameraStreamReader.ReadStream(): {ex.Message} {ex.StackTrace}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Unable to retrieve stream URL.");
            }
            //}
        }

        private async Task<string> TestStreamUrl(Size size)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(GetStreamUrl(size));
                request.Timeout = 5000; // Set a longer timeout

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return response.ResponseUri.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving stream URL: {ex.Message}");
            }

            return null;
        }

        private Stream GetResponseStream(string url, CancellationToken cancellationToken)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 5000; // Set a longer timeout

            var response = (HttpWebResponse)request.GetResponse();
            return new CancellableStream(response.GetResponseStream(), cancellationToken);
        }

        protected abstract string GetStreamUrl(Size size);
    }
}