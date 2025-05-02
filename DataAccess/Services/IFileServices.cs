using DataAccess.Helpers.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IFileServices
    {
        Task<string> UploadFileAsync(IFormFile file, ImageFileType imageFileType);
    }
}
