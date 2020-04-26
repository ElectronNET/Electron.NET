using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ElectronNET.API.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class NativeImage
    {
        private Image _image;
        private bool _isTemplateImage = false;

        public static NativeImage CreateEmpty()
        {
            return new NativeImage();
        }

        public static NativeImage CreateFromBuffer(byte[] buffer)
        {
            return new NativeImage(buffer);
        }

        public static NativeImage CreateFromBuffer(byte[] buffer, CreateFromBufferOptions options)
        {
            return new NativeImage(buffer);
        }

        public static NativeImage CreateFromDataURL(string dataUrl)
        {
            var parsedDataUrl = Regex.Match(dataUrl, @"data:image/(?<type>.+?),(?<data>.+)");
            var actualData = parsedDataUrl.Groups["data"].Value;
            var type = parsedDataUrl.Groups["type"].Value;
            var binData = Convert.FromBase64String(actualData);

            var imageFormat = type switch
            {
                "jpeg" => ImageFormat.Jpeg,
                "jpg" => ImageFormat.Jpeg,
                "png" => ImageFormat.Png,
                "gif" => ImageFormat.Gif,
                "x-icon" => ImageFormat.Icon,
                "bmp" => ImageFormat.Bmp,
                _ => ImageFormat.Png
            };

            return new NativeImage(binData, imageFormat);
        }

        public static NativeImage CreateFromPath(string path)
        {
            return new NativeImage(path);
        }

        public NativeImage()
        {
        }

        public NativeImage(Bitmap bitmap)
        {
            _image = bitmap;
        }
        public NativeImage(string path)
        {
            _image = Image.FromFile(path);
        }


        public NativeImage(byte[] buffer)
        {
            var ms = new MemoryStream(buffer);
            _image = Image.FromStream(ms);
        }

        public NativeImage(byte[] buffer, ImageFormat imageFormat)
        {
            _image = BytesToImage(buffer, imageFormat);
        }

        public Image GetBitmap()
        {
            return _image;
        }

        public NativeImage Crop(Rectangle rect)
        {
            using (var g = Graphics.FromImage(_image))
            {
                var bmp = new Bitmap(rect.Width, rect.Height);
                g.DrawImage(bmp, new System.Drawing.Rectangle(0, 0, _image.Width, _image.Height), new System.Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height), GraphicsUnit.Pixel);

                return new NativeImage(bmp);
            }
        }
        public void AddRepresentation(AddRepresentationOptions options)
        {
            if (options.Buffer.Length > 0)
            {
                _image = CreateFromBuffer(options.Buffer).GetBitmap();
            }
            else if (!string.IsNullOrEmpty(options.DataUrl))
            {
                _image = CreateFromDataURL(options.DataUrl).GetBitmap();
            }
        }

        public double GetAspectRatio => _image.Width / _image.Height;


        public byte[] GetBitmap(BitmapOptions options)
        {
            return ImageToBytes(_image.RawFormat);
        }

        public byte[] GetNativeHandle()
        {
            return ImageToBytes(_image.RawFormat);
        }

        public Size GetSize()
        {
            return new Size
            {
                Width = _image.Width,
                Height = _image.Height
            };
        }

        public bool IsEmpty()
        {
            return _image == null;
        }

        /// <summary>
        /// Deprecated. Whether the image is a template image.
        /// </summary>
        /// <param name="option"></param>
        public bool IsTemplateImage => _isTemplateImage;

        /// <summary>
        /// Deprecated. Marks the image as a template image.
        /// </summary>
        public void SetTemplateImage(bool option)
        {
            _isTemplateImage = option;
        }


        public NativeImage Resize(ResizeOptions options)
        {
            return new NativeImage((Bitmap)Resize(options.Width, options.Height));
        }

        public byte[] ToBitmap(ToBitmapOptions options)
        {
            return ImageToBytes(_image.RawFormat);
        }

        public string ToDataURL(ToDataUrlOptions options)
        {
            var mimeType = ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == _image.RawFormat.Guid)?.MimeType;
            if (mimeType is null)
            {
                mimeType = "image/png";
            }

            var bytes = ImageToBytes(_image.RawFormat);
            var base64 = Convert.ToBase64String(bytes);

            return $"data:{mimeType};base64,{base64}";
        }

        public byte[] ToJPEG(int quality)
        {
            return ImageToBytes(ImageFormat.Jpeg, 1.0d, quality);
        }

        public byte[] ToPNG(ToPNGOptions options)
        {
            return ImageToBytes(ImageFormat.Png, options.ScaleFactor);
        }
        
        private byte[] ImageToBytes(ImageFormat imageFormat, double scaleFactor = 1.0d, int quality = 100)
        {
            using (var ms = new MemoryStream())
            {
                Image img = _image;

                if (Math.Abs(scaleFactor - 1.0d) > 0.0)
                {
                    img = Resize(_image.Width, _image.Height, scaleFactor);
                }

                ImageCodecInfo encoderCodecInfo = GetEncoder(imageFormat);
                Encoder encoder = Encoder.Quality;

                EncoderParameters encoderParameters = new EncoderParameters(1)
                {
                    Param = new []
                    {
                        new EncoderParameter(encoder, quality)
                    } 
                };

                img.Save(ms, encoderCodecInfo, encoderParameters);
                
                return ms.ToArray();
            }
        }

        private Image Resize(int? width, int? height, double scaleFactor = 1.0d)
        {
            using (var g = Graphics.FromImage(_image))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;

                var bmp = new Bitmap((int)Math.Round(width ?? _image.Width * scaleFactor), (int)Math.Round(height ?? _image.Height * scaleFactor));
                g.DrawImage(bmp,
                    new System.Drawing.Rectangle(0, 0, _image.Width, _image.Height),
                    new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                    GraphicsUnit.Pixel);

                return bmp;
            }
        }

        private Image BytesToImage(byte[] bytes, ImageFormat imageFormat)
        {
            var ms = new MemoryStream(bytes);
            return Image.FromStream(ms);
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        internal byte[] GetBytes()
        {
            return ImageToBytes(_image.RawFormat);
        }
    }
}
