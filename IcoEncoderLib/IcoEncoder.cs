using System.Collections.Generic;
using System.IO;

namespace IcoEncoderLib
{
    public struct ImageInfo
    {
        /// <summary>
        /// The image data, either in BITMAPINFOHEADER format or as PNG.
        /// </summary>
        public readonly Stream ImageData;

        /// <summary>
        /// Width in pixels, should be 0 if 256 pixels.
        /// </summary>
        public readonly byte Width;

        /// <summary>
        /// Height in pixels, should be 0 if 256 pixels.
        /// </summary>
        public readonly byte Height;

        /// <summary>
        /// Color count, should be 0 if more than 256 colors.
        /// </summary>
        public readonly byte ColorCount;

        /// <summary>
        /// Bits per pixel.
        /// </summary>
        public readonly ushort BitPerPixel;

        public ImageInfo(Stream imageData, byte width, byte height, byte colorCount, ushort bitPerPixel)
        {
            ImageData = imageData;
            Width = width;
            Height = height;
            ColorCount = colorCount;
            BitPerPixel = bitPerPixel;
        }
    }

    public class IcoEncoder
    {
        // https://www.informit.com/articles/article.aspx?p=1186882

        private const uint HeaderLength = 6;
        private const uint DirectoryEntryLength = 16;

        private readonly IList<ImageInfo> images = new List<ImageInfo>();

        private readonly Stream stream;

        private uint nextImageOffset = 0;

        public IcoEncoder(Stream stream)
        {
            this.stream = stream;
        }

        public void AddImage(ImageInfo image)
        {
            images.Add(image);
        }

        public void Commit()
        {
            stream.Position = 0;
            stream.SetLength(0);
            WriteHeader(stream);
            WriteDirectory(stream);
            WriteImageData(stream);
        }
        private void WriteHeader(Stream stream)
        {
            stream.WriteUInt16LE(0); // Reserved. Must always be 0.

            stream.WriteUInt16LE(1); // Specifies image type: 1 for icon(.ICO) image, 2 for cursor(.CUR) image. Other values are invalid.

            stream.WriteUInt16LE((ushort)images.Count); // Specifies number of images in the file.
        }

        private void WriteDirectory(Stream stream)
        {
            uint directoryLength = (uint)images.Count * DirectoryEntryLength;
            nextImageOffset = HeaderLength + directoryLength;

            foreach (var image in images)
            {
                WriteDirectoryEntry(stream, image);
            }
        }

        private void WriteImageData(Stream stream)
        {
            foreach (var image in images)
            {
                WriteImage(stream, image);
            }
        }

        private void WriteDirectoryEntry(Stream stream, ImageInfo image)
        {
            stream.WriteByte(image.Width); // Width, should be 0 if 256 pixels
            stream.WriteByte(image.Height); // Height, should be 0 if 256 pixels

            stream.WriteByte(image.ColorCount); // Color count, should be 0 if more than 256 colors

            stream.WriteByte(0); // Reserved, should be 0

            stream.WriteUInt16LE(1); // Color planes when in .ICO format, should be 0 or 1, or the X hotspot when in .CUR format

            stream.WriteUInt16LE(image.BitPerPixel); // Bits per pixel when in .ICO format, or the Y hotspot when in .CUR format

            stream.WriteUInt32LE((uint)image.ImageData.Length); // Size of the image data in bytes.

            stream.WriteUInt32LE(nextImageOffset); // Offset of the image data in the file.

            nextImageOffset += (uint)image.ImageData.Length;
        }

        private void WriteImage(Stream stream, ImageInfo image)
        {
            image.ImageData.Position = 0;
            image.ImageData.CopyTo(stream);
        }
    }
}
