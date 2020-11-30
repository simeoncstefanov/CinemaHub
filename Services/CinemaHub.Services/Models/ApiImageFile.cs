namespace CinemaHub.Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

    public class ApiImageFile : IFormFile
    {
        private const int DefaultBufferSize = 80 * 1024;

        private readonly byte[] bytes;

        public ApiImageFile(byte[] bytes, long length, string name, string fileName)
        {
            this.bytes = bytes;
            this.Length = length;
            this.Name = name;
            this.FileName = fileName;
        }

        public string ContentType => null;

        public string ContentDisposition => null;

        public IHeaderDictionary Headers { get; set; }

        public long Length { get; }

        public string Name { get; }

        public string FileName { get; }

        public void CopyTo(Stream target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            using (var readStream = this.OpenReadStream())
            {
                readStream.CopyTo(target, DefaultBufferSize);
            }
        }

        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            using (var readStream = this.OpenReadStream())
            {
                await readStream.CopyToAsync(target, DefaultBufferSize);
            }
        }

        public Stream OpenReadStream()
        {
            return new MemoryStream(this.bytes);
        }
    }
}
