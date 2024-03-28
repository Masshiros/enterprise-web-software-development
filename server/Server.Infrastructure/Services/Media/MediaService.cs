﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Server.Application.Common.Dtos;
using Server.Application.Common.Interfaces.Services;
using System.Net.Http.Headers;

namespace Server.Infrastructure.Services.Media
{
    public class MediaService : IMediaService
    {
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly MediaSettings _settings;
        private readonly IDateTimeProvider _dateTimeProvider;

        public MediaService(IWebHostEnvironment hostingEnv, IOptions<MediaSettings> settings, IDateTimeProvider dateTimeProvider)
        {
            _hostingEnv = hostingEnv;
            _settings = settings.Value;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<List<FileDto>> UploadFiles( List<IFormFile> files,string type)
        {
            
         
            var now = _dateTimeProvider.UtcNow;
            var fileInfos = new List<FileDto>();
            if (files is null || files.Contains(null))
            {
                return new List<FileDto>{};
            }
            foreach (var file in files)
            {
                var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition)?.FileName?.Trim('"');
                var imageFolder = $@"\{_settings.MediaFolder}\{type}\{now:MMyyyy}\{now:D}";
                var folder = _hostingEnv.WebRootPath + imageFolder;
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                var filePath = Path.Combine(folder, filename ?? string.Empty);
                using var fs = File.Create(filePath);
                file.CopyTo(fs);
                fs.Flush();
                var path = Path.Combine(imageFolder, filename ?? string.Empty).Replace("\\", "/");
                var fileInfo = new FileDto
                {
                    Path = path,
                    Type = type,
                    Name = filename
                };
                fileInfos.Add(fileInfo);
            }
            return fileInfos;

           
        }
        public Task RemoveFile(List<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                var absolutePath = Path.Combine(_hostingEnv.WebRootPath, filePath.TrimStart('\\', '/'));

                if (File.Exists(absolutePath))
                {
                    try
                    {
                        File.Delete(absolutePath);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Error deleting file at {absolutePath}", ex);
                    }
                }
            }
          

            return Task.CompletedTask;
        }

    }
}