using IcoEncoderLib;
using System;
using System.IO;
using WIC;
using Xunit;

namespace IcoEncoderLibTest
{
    public class IcoEncoderTest
    {
        private readonly WICImagingFactory wic = new WICImagingFactory();

        [Theory]
        [InlineData("PNG_32x32_32bpp.png", 32, 32)]
        [InlineData("PNG_128x128_32bpp.png", 128, 128)]
        [InlineData("PNG_256x256_32bpp.png", 256, 256)]
        public void Test(string fileName, int width, int height)
        {
            string pngFilePath = TestUtil.GetResourceFilePath(fileName);
            using var pngStream = File.OpenRead(pngFilePath);

            string icoFilePath = TestUtil.GetTestFilePath(Path.GetFileName(pngFilePath) + ".ico");
            using (var icoStream = File.OpenWrite(icoFilePath))
            {
                var icoEncoder = new IcoEncoder(icoStream);
                icoEncoder.AddImage(new ImageInfo(pngStream, (byte)width, (byte)height, 0, 32));
                icoEncoder.Commit();
            }

            ValidateIcoFile(icoFilePath, width, height);
        }

        /// <summary>
        /// Validates if the ICO can be decoded and has the correct size.
        /// </summary>
        private void ValidateIcoFile(string filePath, int width, int height)
        {
            using (var icoStream = File.OpenRead(filePath))
            {
                var deocder = wic.CreateDecoder(ContainerFormat.Ico);
                deocder.Initialize(icoStream.AsCOMStream(), WICDecodeOptions.WICDecodeMetadataCacheOnLoad);
                var size = deocder.GetFrame(0).GetSize();
                Assert.Equal(width, size.Width);
                Assert.Equal(height, size.Height);
            }
        }
    }
}
