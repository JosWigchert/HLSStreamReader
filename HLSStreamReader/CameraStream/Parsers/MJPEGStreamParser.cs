using OpenCvSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace HLSStreamReader.CameraStream.Parsers
{
    public class MJPEGStreamParser : StreamParser
    {
        public async override IAsyncEnumerable<Mat> Parse(ConcurrentQueue<byte> bytes, CancellationToken cancellationToken)
        {
            if (bytes == null)
            {
                yield break;
            }

            bool found = false;

            int start = 0;
            int end = 0;

            List<byte> img = new List<byte>();

            byte b = 0x00;
            Stopwatch sw = Stopwatch.StartNew();
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!found)
                {
                    // Waiting for the start bytes of MJPEG
                    if (bytes.TryDequeue(out byte b2))
                    {
                        if (b == 0xFF && b2 == 0xD8)
                        {
                            found = true;
                            img.Add(b);
                        }
                        b = b2;
                    }
                    else
                    {
                        // Data probably not available yet, sleep for 1 ms
                    }
                }
                else
                {
                    // Waiting for the end bytes of MJPEG. Not found? just add the bytes to the list
                    if (bytes.TryDequeue(out byte b2))
                    {
                        img.Add(b);
                        if (b == 0xFF && b2 == 0xD9)
                        {
                            found = false;
                            img.Add(b2);

                            //string filename = "output/" + DateTime.Now.TimeOfDay.TotalMilliseconds + ".jpg";
                            //File.WriteAllBytes(filename, img.ToArray());

                            Mat mat = Cv2.ImDecode(img.ToArray(), ImreadModes.Color);
                            yield return mat;
                            float fps = 1000f / sw.ElapsedMilliseconds;
                            sw.Restart();
                            Debug.WriteLine($"FPS: {fps}");
                            img.Clear();
                        }
                        b = b2;
                    }
                    else
                    {
                        // Data probably not available yet, sleep for 1 ms
                    }
                }
            }
        }
    }
}
