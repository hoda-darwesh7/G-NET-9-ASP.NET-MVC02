using GymManagement.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class AttachmentService : IAttachmentService
    {
        private readonly ILogger<AttachmentService> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly long _MaxFileSize = 5 * 1024 * 1024;
        private readonly string[] _AllowedExtensions = { ".png", ".jpeg", ".jpg" };

        public AttachmentService(ILogger<AttachmentService> logger , IWebHostEnvironment environment)
        {
            _logger = logger;
            _env = environment;
        }

        public async Task<string?> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default)
        {
            if (fileStream is null || !fileStream.CanRead) return null;
            if (fileStream.Length == 0) return null;


            if(fileStream.Length > _MaxFileSize)
            {
                _logger.LogError($"File Rehected : Too Large {fileStream.Length}");
                return null;
            }

            var Extension = Path.GetExtension(fileName);

            if(string.IsNullOrWhiteSpace(Extension) || !_AllowedExtensions.Contains(Extension))
            {
                _logger.LogError($"File Rehected : This Extension Not Allowed !");
                return null;
            }


            var UploadsFolder = Path.Combine(_env.ContentRootPath , fileName);
            Directory.CreateDirectory( UploadsFolder );

            var storedFileName = $"{Guid.NewGuid}{fileName}";
            var FilePath = Path.Combine(UploadsFolder, storedFileName);

            try
            {
                using var fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write);

                await fileStream.CopyToAsync(fs, ct);
                return storedFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed To Upload Photo!");
                return null;
            }
           


        }
    }
}
