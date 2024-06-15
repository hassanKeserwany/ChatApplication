using API.Helper;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySetttings> config)
        {
            var account = new Account
                (
                    config.Value.CloudName,
                    config.Value.ApiKey,
                    config.Value.ApiSecret
                );
            _cloudinary =new Cloudinary(account);
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResults = new ImageUploadResult();

            // Check if the file has content
            if (file.Length > 0)
            {
                // Open a stream to read the file content
                using var stream = file.OpenReadStream();

                // Define the upload parameters
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),  // Add the stream here to upload the file
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };

                // Asynchronously upload the file to Cloudinary
                uploadResults = await _cloudinary.UploadAsync(uploadParams);
            }
            return  uploadResults;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result =await _cloudinary.DestroyAsync(deleteParams);
            return result;
        }
    }
}
