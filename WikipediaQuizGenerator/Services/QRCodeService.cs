using System.Drawing.Imaging;
using ZXing.Rendering;
using ZXing.Common;
using ZXing;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using ZXing.QrCode.Internal;

namespace WikipediaQuizGenerator.Services
{
    public class QRCodeService
    {
        private readonly IWebHostEnvironment _env;

        public QRCodeService(IWebHostEnvironment env) 
        {
            _env = env;
        }
        /// <summary>
        /// Generates a QR Code given a url address. This qr image is generated as a png and saved to the client's wwwroot/images folder
        /// as qrcode.png. Each time this method is called, the image is overwritten and replaced. This method currently is only functional on
        /// windows computers as it uses a bitmap data type that is incompatible with other OS.
        /// </summary>
        /// <param name="url">string represting a website, usually beginning with http:// or https:// </param>
        /// <returns>string representation of the qrcode's save directory</returns>
        public string GenerateQrCode(string url)
        {
            {

                string filename = DateTime.UtcNow.Ticks.ToString();
                string qrCodeurl = $"images\\qrcode.png?{filename}";

                // Initialize the BarcodeWriter with PixelDataRenderer
                var writer = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new EncodingOptions
                    {
                        Width = 200,
                        Height = 200
                    },
                    Renderer = new PixelDataRenderer() // Use PixelDataRenderer
                };

                // Generate pixel data
                var pixelData = writer.Write(url);

                // Convert pixel data to Bitmap
                using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
                {
                    // Lock bitmap's bits
                    var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

                    // Copy pixel data to bitmap
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);

                    // Unlock bitmap's bits
                    bitmap.UnlockBits(bitmapData);

                    // Define the path where the QR code will be saved
                    string folderPath = Directory.GetCurrentDirectory();
                    folderPath = Path.Combine(folderPath.Substring(0, folderPath.Length-23), "WikipediaQuizGenerator.Client", "wwwroot", "images");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath = Path.Combine(folderPath, "qrcode.png");

                    // Save the Bitmap to a file
                    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        bitmap.Save(stream, ImageFormat.Png);
                    }

                    // Return the path to the saved file
                    return qrCodeurl;
                }
            }


            {



                var writer = new ZXing.BarcodeWriter<System.Drawing.Bitmap>
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new EncodingOptions { Height = 500, Width = 500 },
                };



                var bitmap = writer.Write(url);
                {
                    // Save the QR code to a file
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "WikipediaQuizGenerator.Client", "wwwroot", "images");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string filePath = Path.Combine(folderPath, "qrcode.png");

                    // Ensure the file stream is disposed properly
                    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        bitmap.Save(stream, ImageFormat.Png);
                    }

                    return filePath;
                }
            }

        }


                public string getCurrentFilepath() 
        {
            string currentDirectory = _env.ContentRootPath;

            return currentDirectory;

        }
    }
}
