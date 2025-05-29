using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PersonnelTransferRequest.Common.Extensions
{

    /// <summary>
    /// Provides an extension method to upload and store profile images on the server with validation for allowed file types.
    /// </summary>


    public static class UploadImageExtension
    {
        private static readonly List<string> AllowedExtensions = new List<string> { ".jpg", ".png", ".svg", ".jpeg" };
        private const string UploadFolder = "wwwroot/uploads/profilPhoto/";
        private const string ReturnPath = "/uploads/profilPhoto/";

        public static string UploadProfileImage(IFormFile photoFile, string filePrefix = "profil-photo")
        {
            if (photoFile == null || photoFile.Length == 0)
                return null;

            var fileExtension = Path.GetExtension(photoFile.FileName).ToLower();

            if (!AllowedExtensions.Contains(fileExtension))
                return null;

            try
            {
                // Ensure directory exists
                Directory.CreateDirectory(UploadFolder);

                // Generate filename with prefix and cleaned GUID
                var fileName = $"{filePrefix}-{Guid.NewGuid():N}{fileExtension}";
                var filePath = Path.Combine(UploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    photoFile.CopyTo(stream);
                }

                return $"{ReturnPath}{fileName}";
            }
            catch (Exception ex)
            {
                // Log error here if needed
                return null;
            }
        }
    }
}