using System.Drawing;
using ElectronNET.API.Entities;
using ElectronNET.IntegrationTests.Common;
using System.Runtime.Versioning;
using RectangleEntity = ElectronNET.API.Entities.Rectangle;

namespace ElectronNET.IntegrationTests.Tests
{
    [Collection("ElectronCollection")]
    [SupportedOSPlatform(Windows)]
    public class NativeImageTests : IntegrationTestBase
    {
        public NativeImageTests(ElectronFixture fx) : base(fx)
        {
        }

        [IntegrationFact]
        public async Task Create_from_bitmap_and_to_png()
        {
            using var bmp = new Bitmap(10, 10);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Red);
            }

            var native = NativeImage.CreateFromBitmap(bmp);
            var size = native.GetSize();
            size.Width.Should().Be(10);
            size.Height.Should().Be(10);
            var png = native.ToPNG(new ToPNGOptions { ScaleFactor = 1.0f });
            png.Should().NotBeNull();
            png!.Length.Should().BeGreaterThan(0);
        }

        [IntegrationFact]
        public async Task Create_from_buffer_and_to_data_url()
        {
            // Prepare PNG bytes
            using var bmp = new Bitmap(8, 8);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Blue);
            }

            using var ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            var bytes = ms.ToArray();
            var native = NativeImage.CreateFromBuffer(bytes);
            var dataUrl = native.ToDataURL(new ToDataUrlOptions { ScaleFactor = 1.0f });
            dataUrl.Should().NotBeNullOrWhiteSpace();
            dataUrl!.StartsWith("data:image/", StringComparison.Ordinal).Should().BeTrue();
        }

        [IntegrationFact]
        public async Task Resize_and_crop_produce_expected_sizes()
        {
            using var bmp = new Bitmap(12, 10);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Green);
            }

            var native = NativeImage.CreateFromBitmap(bmp);
            var resized = native.Resize(new ResizeOptions { Width = 6, Height = 5 });
            var rsize = resized.GetSize();
            rsize.Width.Should().Be(6);
            rsize.Height.Should().Be(5);
            var cropped = native.Crop(new RectangleEntity { X = 2, Y = 2, Width = 4, Height = 3 });
            var csize = cropped.GetSize();
            csize.Width.Should().Be(4);
            csize.Height.Should().Be(3);
        }

        [IntegrationFact]
        public async Task Add_representation_for_scale_factor()
        {
            using var bmp = new Bitmap(5, 5);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Black);
            }

            using var ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            var buffer = ms.ToArray();
            var native = NativeImage.CreateFromBitmap(bmp);
            native.AddRepresentation(new AddRepresentationOptions { Buffer = buffer, ScaleFactor = 2.0f });
            var size2X = native.GetSize(2.0f);
            size2X.Should().NotBeNull();
            size2X.Width.Should().Be(5);
            size2X.Height.Should().Be(5);
        }
    }
}