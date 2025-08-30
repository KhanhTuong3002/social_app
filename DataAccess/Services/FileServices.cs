using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BussinessObject.Entities;
using DataAccess.Helpers.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DataAccess.Services
{
    public class FileServices : IFileServices
    {
        private readonly BlobServiceClient _blobServiceClient;
        public FileServices(string cntString)
        {
            _blobServiceClient = new BlobServiceClient(cntString);
        }

        public async Task<string> UploadFileAsync(IFormFile file, ImageFileType imageFileType)
        {

            string containerName = imageFileType switch
            {
                ImageFileType.postImage => "posts",
                ImageFileType.storyImage => "stories",
                ImageFileType.profileImage => "profiles",
                ImageFileType.coverImage => "covers",
                mageFileType.PostVideo => "posts",
                ImageFileType.StoryVideo =>"stories",
                _ => throw new ArgumentException("Invalid file type", nameof(imageFileType))
            };

            if(file == null || file.Length == 0)
              return "";
            //ensure the file is not null and has content
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            //generate a unique file name
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = containerClient.GetBlobClient(fileName);

            //upload the file to the blob storage
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                });
            }

            return blobClient.Uri.ToString();

           /* if (file != null && file.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (file.ContentType.Contains("image")|| file.ContentType.Contains("video"))
                {
                    string rootFolderPathImage = Path.Combine(rootFolderPath, containerName);
                    Directory.CreateDirectory(rootFolderPathImage); // Create the directory if it doesn't exist

                    *//*string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);*//*
                    string filePath = Path.Combine(rootFolderPathImage, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    //set the image url to the new post
                   // story.ImageUrl = "/images/stories/" + fileName; // Set the image URL to the new post

                    return $"{containerName}\\{fileName}";
                }
            }
            return "";*/
        }
    }
}  



