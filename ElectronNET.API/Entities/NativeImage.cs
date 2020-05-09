using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// Native Image handler for Electron.NET
    /// </summary>
    [JsonConverter(typeof(NativeImageJsonConverter))]
    public class NativeImage
    {
        private readonly Dictionary<float, Image> _images = new Dictionary<float, Image>();
        private bool _isTemplateImage;

        private static readonly Dictionary<string, float> ScaleFactorPairs = new Dictionary<string, float>
        {
            {"@2x",   2.0f}, {"@3x",     3.0f}, {"@1x",     1.0f}, {"@4x",   4.0f},
            {"@5x",   5.0f}, {"@1.25x", 1.25f}, {"@1.33x", 1.33f}, {"@1.4x", 1.4f},
            {"@1.5x", 1.5f}, {"@1.8x",   1.8f}, {"@2.5x",   2.5f}
        };

        private static float? ExtractDpiFromFilePath(string filePath)
        {
            var withoutExtension = Path.GetFileNameWithoutExtension(filePath);
            return ScaleFactorPairs
                .Where(p => withoutExtension.EndsWith(p.Key))
                .Select(p => p.Value)
                .FirstOrDefault();
        }
        private static Image BytesToImage(byte[] bytes)
        {
            var ms = new MemoryStream(bytes);
            return Image.FromStream(ms);
        }

        /// <summary>
        /// Creates an empty NativeImage
        /// </summary>
        public static NativeImage CreateEmpty()
        {
            return new NativeImage();
        }

        /// <summary>
        /// 
        /// </summary>
        public static NativeImage CreateFromBitmap(Bitmap bitmap, CreateFromBitmapOptions options = null)
        {
            if (options is null)
            {
                options = new CreateFromBitmapOptions();
            }

            return new NativeImage(bitmap, options.ScaleFactor);
        }

        /// <summary>
        /// Creates a NativeImage from a byte array.
        /// </summary>
        public static NativeImage CreateFromBuffer(byte[] buffer, CreateFromBufferOptions options = null)
        {
            if (options is null)
            {
                options = new CreateFromBufferOptions();
            }

            var ms = new MemoryStream(buffer);
            var image = Image.FromStream(ms);

            return new NativeImage(image, options.ScaleFactor);
        }

        /// <summary>
        /// Creates a NativeImage from a base64 encoded data URL.
        /// </summary>
        /// <param name="dataUrl">A data URL with a base64 encoded image.</param>
        public static NativeImage CreateFromDataURL(string dataUrl)
        {
            var images = new Dictionary<float,Image>();
            var parsedDataUrl = Regex.Match(dataUrl, @"data:image/(?<type>.+?),(?<data>.+)");
            var actualData = parsedDataUrl.Groups["data"].Value;
            var binData = Convert.FromBase64String(actualData);

            var image = BytesToImage(binData);

            images.Add(1.0f, image);

            return new NativeImage(images);
        }

        /// <summary>
        /// Creates a NativeImage from an image on the disk.
        /// </summary>
        /// <param name="path">The path of the image</param>
        public static NativeImage CreateFromPath(string path)
        {
            var images = new Dictionary<float,Image>();
            if (Regex.IsMatch(path, "(@.+?x)"))
            {
                var dpi = ExtractDpiFromFilePath(path);
                if (dpi == null)
                {
                    throw new Exception($"Invalid scaling factor for '{path}'.");
                }

                images[dpi.Value] = Image.FromFile(path);
            }
            else
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                var extension = Path.GetExtension(path);
                // Load as 1x dpi
                images[1.0f] = Image.FromFile(path);

                foreach (var scale in ScaleFactorPairs)
                {
                    var fileName = $"{fileNameWithoutExtension}{scale}.{extension}";
                    if (File.Exists(fileName))
                    {
                        var dpi = ExtractDpiFromFilePath(fileName);
                        if (dpi != null)
                        {
                            images[dpi.Value] = Image.FromFile(fileName);
                        }
                    }
                }
            }

            return new NativeImage(images);
        }

        /// <summary>
        /// Creates an empty NativeImage
        /// </summary>
        public NativeImage()
        {
        }

        /// <summary>
        /// Creates a NativeImage from a bitmap and scale factor
        /// </summary>
        public NativeImage(Image bitmap, float scaleFactor = 1.0f)
        {
            _images.Add(scaleFactor, bitmap);
        }

        /// <summary>
        /// Creates a NativeImage from a dictionary of scales and images.
        /// </summary>
        public NativeImage(Dictionary<float, Image> imageDictionary)
        {
            _images = imageDictionary;
        }

        /// <summary>
        /// Crops the image specified by the input rectangle and computes scale factor
        /// </summary>
        public NativeImage Crop(Rectangle rect)
        {
            var images = new Dictionary<float,Image>();
            foreach (var image in _images)
            {
                images.Add(image.Key, Crop(rect.X, rect.Y, rect.Width, rect.Height, image.Key));
            }

            return new NativeImage(images);
        }

        /// <summary>
        /// Resizes the image and computes scale factor
        /// </summary>
        public NativeImage Resize(ResizeOptions options)
        {
            var images = new Dictionary<float, Image>();
            foreach (var image in _images)
            {
                images.Add(image.Key, Resize(options.Width, options.Height, image.Key));
            }

            return new NativeImage(images);
        }

        /// <summary>
        /// Add an image representation for a specific scale factor.
        /// </summary>
        /// <param name="options"></param>
        public void AddRepresentation(AddRepresentationOptions options)
        {
            if (options.Buffer.Length > 0)
            {
                _images[options.ScaleFactor] =
                    CreateFromBuffer(options.Buffer, new CreateFromBufferOptions {ScaleFactor = options.ScaleFactor})
                        .GetScale(options.ScaleFactor);
            }
            else if (!string.IsNullOrEmpty(options.DataUrl))
            {
                _images[options.ScaleFactor] = CreateFromDataURL(options.DataUrl).GetScale(options.ScaleFactor);
            }
        }

        /// <summary>
        /// Gets the aspect ratio for the image based on scale factor
        /// </summary>
        /// <param name="scaleFactor">Optional</param>
        public float GetAspectRatio(float scaleFactor = 1.0f)
        {
            var image = GetScale(scaleFactor);
            if (image != null)
            {
                return image.Width / image.Height;
            }

            return 0f;
        }

        /// <summary>
        /// Returns a byte array that contains the image's raw bitmap pixel data.
        /// </summary>
        public byte[] GetBitmap(BitmapOptions options)
        {
            return ToBitmap(new ToBitmapOptions{ ScaleFactor = options.ScaleFactor });
        }

        /// <summary>
        /// Returns a byte array that contains the image's raw bitmap pixel data.
        /// </summary>
        public byte[] GetNativeHandle()
        {
            return ToBitmap(new ToBitmapOptions());
        }

        /// <summary>
        /// Gets the size of the specified image based on scale factor
        /// </summary>
        public Size GetSize(float scaleFactor = 1.0f)
        {
            if (_images.ContainsKey(scaleFactor))
            {
                var image = _images[scaleFactor];
                return new Size
                {
                    Width = image.Width,
                    Height = image.Height
                };
            }

            return null;
        }

        /// <summary>
        /// Checks to see if the NativeImage instance is empty.
        /// </summary>
        public bool IsEmpty()
        {
            return _images.Count <= 0;
        }

        /// <summary>
        /// Deprecated. Whether the image is a template image.
        /// </summary>
        public bool IsTemplateImage => _isTemplateImage;

        /// <summary>
        /// Deprecated. Marks the image as a template image.
        /// </summary>
        public void SetTemplateImage(bool option)
        {
            _isTemplateImage = option;
        }

        /// <summary>
        /// Outputs a bitmap based on the scale factor
        /// </summary>
        public byte[] ToBitmap(ToBitmapOptions options)
        {
            return ImageToBytes(ImageFormat.Bmp, options.ScaleFactor);
        }

        /// <summary>
        /// Outputs a data URL based on the scale factor
        /// </summary>
        public string ToDataURL(ToDataUrlOptions options)
        {
            if (!_images.ContainsKey(options.ScaleFactor))
            {
                return null;
            }

            var image = _images[options.ScaleFactor];
            var mimeType = ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == image.RawFormat.Guid)?.MimeType;
            if (mimeType is null)
            {
                mimeType = "image/png";
            }

            var bytes = ImageToBytes(image.RawFormat, options.ScaleFactor);
            var base64 = Convert.ToBase64String(bytes);

            return $"data:{mimeType};base64,{base64}";
        }

        /// <summary>
        /// Outputs a JPEG for the default scale factor
        /// </summary>
        public byte[] ToJPEG(int quality)
        {
            return ImageToBytes(ImageFormat.Jpeg, 1.0f, quality);
        }

        /// <summary>
        /// Outputs a PNG for the specified scale factor
        /// </summary>
        public byte[] ToPNG(ToPNGOptions options)
        {
            return ImageToBytes(ImageFormat.Png, options.ScaleFactor);
        }

        private byte[] ImageToBytes(ImageFormat imageFormat = null, float scaleFactor = 1.0f, int quality = 100)
        {
            using var ms = new MemoryStream();

            if (_images.ContainsKey(scaleFactor))
            {
                var image = _images[scaleFactor];
                var encoderCodecInfo = GetEncoder(imageFormat ?? image.RawFormat);
                var encoder = Encoder.Quality;

                var encoderParameters = new EncoderParameters(1)
                {
                    Param = new[]
                    {
                        new EncoderParameter(encoder, quality)
                    }
                };

                image.Save(ms, encoderCodecInfo, encoderParameters);

                return ms.ToArray();
            }

            return null;
        }

        private Image Resize(int? width, int? height, float scaleFactor = 1.0f)
        {
            if (!_images.ContainsKey(scaleFactor) || (width is null && height is null))
            {
                return null;
            }

            var image = _images[scaleFactor];
            using (var g = Graphics.FromImage(image))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;

                var aspect = GetAspectRatio(scaleFactor);

                width ??= Convert.ToInt32(image.Width * aspect);
                height ??= Convert.ToInt32(image.Height * aspect);

                width = Convert.ToInt32(width * scaleFactor);
                height = Convert.ToInt32(height * scaleFactor);

                var bmp = new Bitmap(width.Value, height.Value);
                g.DrawImage(bmp,
                    new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                    new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                    GraphicsUnit.Pixel);

                return bmp;
            }
        }

        private Image Crop(int? x, int? y, int? width, int? height, float scaleFactor = 1.0f)
        {
            if (!_images.ContainsKey(scaleFactor))
            {
                return null;
            }

            var image = _images[scaleFactor];
            using (var g = Graphics.FromImage(image))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;

                x ??= 0;
                y ??= 0;

                x = Convert.ToInt32(x * scaleFactor);
                y = Convert.ToInt32(y * scaleFactor);

                width ??= image.Width;
                height ??= image.Height;

                width = Convert.ToInt32(width * scaleFactor);
                height = Convert.ToInt32(height * scaleFactor);

                var bmp = new Bitmap(width.Value, height.Value);
                g.DrawImage(bmp, new System.Drawing.Rectangle(0, 0, image.Width, image.Height), new System.Drawing.Rectangle(x.Value, y.Value, width.Value, height.Value), GraphicsUnit.Pixel);

                return bmp;
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        internal Dictionary<float,string> GetAllScaledImages()
        {
            var dict = new Dictionary<float,string>();
            try
            {
                foreach (var (scale, image) in _images)
                {
                    dict.Add(scale, Convert.ToBase64String(ImageToBytes(null, scale)));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
            return dict;
        }

        internal Image GetScale(float scaleFactor)
        {
            if (_images.ContainsKey(scaleFactor))
            {
                return _images[scaleFactor];
            }

            return null;
        }
    }
}
