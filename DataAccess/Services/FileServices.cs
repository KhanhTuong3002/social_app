using BussinessObject.Entities;
using DataAccess.Helpers.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DataAccess.Services
{
    public class FileServices : IFileServices
    {
        public async Task<string> UploadFileAsync(IFormFile file, ImageFileType imageFileType)
        {
            string fileUploadPath = imageFileType switch
            {
                ImageFileType.postImage => Path.Combine("images","posts"),
                ImageFileType.storyImage => Path.Combine("images", "stories"),
                ImageFileType.profileImage => Path.Combine("images", "profiles"),
                ImageFileType.coverImage => Path.Combine("images", "covers"),
                ImageFileType.PostVideo => Path.Combine("videos", "posts"),
                ImageFileType.StoryVideo => Path.Combine("videos", "stories"),
                _ => throw new ArgumentException("Invalid file type", nameof(imageFileType))
            };

            if (file != null && file.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (file.ContentType.Contains("image")|| file.ContentType.Contains("video"))
                {
                    string rootFolderPathImage = Path.Combine(rootFolderPath, fileUploadPath);
                    Directory.CreateDirectory(rootFolderPathImage); // Create the directory if it doesn't exist

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(rootFolderPathImage, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    //set the image url to the new post
                   // story.ImageUrl = "/images/stories/" + fileName; // Set the image URL to the new post

                    return $"{fileUploadPath}\\{fileName}";
                }
            }
            return "";
        }
    }
}  



