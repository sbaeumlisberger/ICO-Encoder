using IcoEncoderLib;
using System;
using System.IO;
using WIC;

namespace IcoEncoderCLI
{
    public class Program
    {
        private static readonly WICImagingFactory wic = new WICImagingFactory();

        public static void Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                Console.WriteLine("Parameters not matching!");
                Console.WriteLine("Usage: IcoEncoderCLI.exe <SourceImagePath> [<OutputPath>]");
                return;
            }

            string sourceFilePath = Path.GetFullPath(args[0]);
            string icoFilePath;
            if (args.Length >= 2)
            {
                icoFilePath = Path.GetFullPath(args[1]);
            }
            else
            {
                string directoryPath = Path.GetDirectoryName(sourceFilePath);
                icoFilePath = Path.Combine(directoryPath, Path.GetFileNameWithoutExtension(sourceFilePath) + ".ico");
            }

            using var icoStream = File.OpenWrite(icoFilePath);
            var icoEncoder = new IcoEncoder(icoStream);

            EncodeImageForResolution(icoEncoder, new WICSize(256, 256), sourceFilePath);

            icoEncoder.Commit();
        }

        private static void EncodeImageForResolution(IcoEncoder icoEncoder, WICSize resolution, string sourceImagePath)
        {
            using var sourceImageStream = File.OpenRead(sourceImagePath);

            var sourceImageDecoder = wic.CreateDecoderFromStream(sourceImageStream, WICDecodeOptions.WICDecodeMetadataCacheOnLoad);

            var pngImageStream = new MemoryStream();

            var pngEncoder = wic.CreateEncoder(ContainerFormat.Png);
            pngEncoder.Initialize(pngImageStream, WICBitmapEncoderCacheOption.WICBitmapEncoderNoCache);

            var pngImage = pngEncoder.CreateNewFrame();
            pngImage.Initialize();
            pngImage.SetPixelFormat(WICPixelFormat.WICPixelFormat32bppBGRA);
            pngImage.SetSize(resolution.Width, resolution.Height);

            var sourceImage = sourceImageDecoder.GetFrame(0);

            if (sourceImage.GetSize().Equals(resolution))
            {
                pngImage.WriteSource(sourceImage);
            }
            else
            {
                var scaler = wic.CreateBitmapScaler();
                scaler.Initialize(sourceImage, resolution, WICBitmapInterpolationMode.WICBitmapInterpolationModeNearestNeighbor);
                pngImage.WriteSource(scaler);
            }

            pngImage.Commit();
            pngEncoder.Commit();
            pngImageStream.Flush();

            byte width = (byte)(resolution.Width == 256 ? 0 : resolution.Width);
            byte height = (byte)(resolution.Height == 256 ? 0 : resolution.Height);

            icoEncoder.AddImage(new ImageInfo(pngImageStream, width, height, 0, 32));
        }

    }
}
