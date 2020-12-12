namespace CinemaHub.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public static class FileDownloader
    {
        public static async Task<string> DownloadImage(IFormFile image, string path, string name)
        {
            Directory.CreateDirectory(path);

            // Check if extension is allowed
            var extension = Path.GetExtension(image.FileName).TrimStart('.');
            var imageFileSignatures = GlobalConstants.ImageFileSignatures;

            if (!imageFileSignatures.Any(x => x.Key == extension))
            {
                throw new Exception($"Invalid image extension {extension}");
            }

            // Check file's header bytes for the given extension's file signature
            using (Stream readStream = image.OpenReadStream())
            {
                using var binaryReader = new BinaryReader(readStream);

                var signatures = imageFileSignatures[extension];
                var headerBytesToCheck = signatures.Max(x => x.Length);

                var headerBytes = binaryReader.ReadBytes(headerBytesToCheck);

                bool isValidImage = signatures.Any(
                    signatureBytes => headerBytes.Take(signatureBytes.Length).SequenceEqual(signatureBytes));

                if (!isValidImage)
                {
                    throw new Exception($"Invalid file format");
                }
            }

            using var fileStream = new FileStream(path + $"{name}.{extension}", FileMode.Create);
            await image.CopyToAsync(fileStream);

            return extension;
        }
    }
}
