using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HLSStreamReader.CameraStream.Helpers
{
    public class MP4Converter
    {
        public static void CreateVideoFromFrames(Mat[] frames, int framerate, string mp4FilePath, FourCC codec)
        {
            if (frames.Length <= 0)
            {
                return;
            }

            var frameSize = frames[0].Size();

            VideoWriter writer = new VideoWriter(mp4FilePath, codec, framerate, frameSize);

            for (int i = 0; i < frames.Length; i++)
            {
                writer.Write(frames[i]);
            }

            writer.Release();
            Console.WriteLine("Video saved to: " + mp4FilePath);

        }

        public static List<Mat> SplitMjpegFrames(byte[] data)
        {
            var frames = new List<Mat>();

            int start = 0;
            int end = 0;

            for (int i = 0; i < data.Length; i++)
            {
                if (i < data.Length - 1 && data[i] == 0xFF && data[i + 1] == 0xD8)
                {
                    start = i;
                }

                if (i < data.Length - 1 && data[i] == 0xFF && data[i + 1] == 0xD9)
                {
                    end = i + 1;

                    byte[] frameData = data.Skip(start).Take(end - start).ToArray();

                    Mat mat = Cv2.ImDecode(frameData, ImreadModes.Color);
                    frames.Add(mat);
                }
            }

            return frames;
        }
    }
}
