using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.BL.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(IFormFile imageFile);
        Task<string> UploadVideoAsync(IFormFile videoFile);
        Task DeleteFileAsync(string fileName);
        bool IsValidImageFile(IFormFile file);
        bool IsValidVideoFile(IFormFile file);
    }
}