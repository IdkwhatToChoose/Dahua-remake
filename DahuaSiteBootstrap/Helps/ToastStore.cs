using Microsoft.AspNetCore.Mvc;
using DahuaSiteBootstrap.Helps;
using DahuaSiteBootstrap.Model;
using DahuaSiteBootstrap.ViewModels;
using DahuaSiteBootstrap.wwwroot.ViewModel;
using Microsoft.AspNetCore.Authentication;
using BCrypt.Net;
using System.Net.Mail;
using System.Net;
using Org.BouncyCastle.Asn1.Cms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO.Compression;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using ImageMagick;

namespace DahuaSiteBootstrap.Helps
{


    public class Support
    {
        readonly string APP_PASSWORD = Environment.GetEnvironmentVariable("eyepas");

        public string SendEmail(MailModel mailbody)
        {

            try
            {
                // Credentials
                var credentials = new NetworkCredential("eyeteh91@gmail.com", APP_PASSWORD);

                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(mailbody.Email),
                    Subject = mailbody.Subject,
                    Body = mailbody.Message
                    
                };

                mail.IsBodyHtml = true;
                mail.To.Add(new MailAddress("eyeteh91@gmail.com"));

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Credentials = credentials,
                    Timeout = 15000,
                };
                client.Send(mail);

                return "Съобщението бе изпратено.Благодарим за обратната връзка!";
            }
            catch (Exception)
            {
                return "Изпращането на съобщение се провали. Моля опитайте по-късно.ERR";
            }

        }

        public bool ConfirmEmail(string email)
        {
            try
            {
                MailAddress ma = new MailAddress(email);
                return true;
            }
            catch { return false; }
        }


    }
    public static class FileSupport
    {
        

        public async static Task<byte[]> ToBytes(IFormFile file)
        {

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }


        }

        public static IFormFile ToFormFile(byte[] data, string fname, string ctype)
        {
            MemoryStream ms = new MemoryStream();

            var formfile = new FormFile(ms, 0, data.Length, "file", fname)
            {
                Headers = new HeaderDictionary(),
                ContentType = ctype
            };

            return formfile;
        }

        public static string FormatFileSize(long bytes)
        {
            var unit = 1024;
            if (bytes < unit) { return $"{bytes} B"; }

            var exp = (int)(Math.Log(bytes) / Math.Log(unit));
            return $"{bytes / Math.Pow(unit, exp):F2} {("KMGTPE")[exp - 1]}B";
        }

        private static async Task<byte[]> Compress(byte[] data)
        {
            using (var stream = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(stream, CompressionMode.Compress))
                {
                    await gzip.WriteAsync(data, 0, data.Length);
                }

                return stream.ToArray();
            }
        }

        public static byte[] Decompress(byte[] compressedData)
        {
            // Create a MemoryStream to hold the compressed data
            using (var compressedStream = new MemoryStream(compressedData))
            {
                // Use GZipStream to decompress the data
                using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                {
                    // Create a MemoryStream to hold the decompressed data
                    using (var decompressedStream = new MemoryStream())
                    {
                        // Copy the decompressed data to the MemoryStream
                        gzipStream.CopyTo(decompressedStream);

                        // Return the decompressed data as a byte array
                        return decompressedStream.ToArray();
                    }
                }
            }
        }

        public static byte[] IncreaseQuality(byte[] data)
        {
            using var image = new MagickImage(data);
            image.Format = MagickFormat.Png;
            
            
            using Stream convertedStream = new MemoryStream();
            image.Write(convertedStream);

            using (var originalImage = Image.FromStream(convertedStream))
            {


                    using (var graphics = Graphics.FromImage(originalImage))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighQuality;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = SmoothingMode.HighQuality;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    
                        

                        graphics.DrawImage(originalImage, 0, 0);

                    }

                    using (var outputStream = new MemoryStream())
                    {

                        originalImage.Save(outputStream, ImageFormat.Jpeg);
                        return outputStream.ToArray();

                    }

                
            }
        }

        public static string GetMimeString(Dsfile image)
        {
            image.Data = Decompress(image.Data);
            string mimeString = $"data:{image.Content.Split(",")[0]};base64,{Convert.ToBase64String(IncreaseQuality(image.Data))}";
            return mimeString;
        }
        public static IEnumerable<string> ReadFileLines(byte[] data)
        {
            using Stream ms = new MemoryStream(data);
            using StreamReader reader = new StreamReader(ms);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        public static string ReadFileLink(byte[] data)
        {
            string[] lines = ReadFileLines(data).ToArray();
            return lines[lines.Length - 1].Split("-")[1];
        }

        public static async Task<byte[]> CompressJpeg(byte[] imageData, long quality = 75L)
        {
            try
            {
                using var inputStream = new MemoryStream(imageData);
                using var outputStream = new MemoryStream();

                using (var image = Image.FromStream(inputStream))
                {
                    var encoderParams = new EncoderParameters(1)
                    {
                        Param = { [0] = new EncoderParameter(Encoder.Quality, quality) }
                    };

                    var jpegEncoder = ImageCodecInfo.GetImageEncoders()
                        .First(x => x.FormatID == ImageFormat.Jpeg.Guid);

                    image.Save(outputStream, jpegEncoder, encoderParams);
                }

                return outputStream.ToArray();
            }
            catch
            {
                // If compression fails, return original
                return imageData;
            }
        }
    }
    public enum EntityType
    {
        File,
        Task,
    }
}
