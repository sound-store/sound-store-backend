using Microsoft.AspNetCore.Http;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace SoundStore.Core.Validations
{
    public class AllowedImageFilesAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp"];

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IEnumerable files)
            {
                foreach (var file in files)
                {
                    if (file is IFormFile formFile)
                    {
                        var extension = Path.GetExtension(formFile.FileName)?.ToLowerInvariant();

                        if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
                        {
                            return new ValidationResult($"Invalid file type: {formFile.FileName}");
                        }
                    }
                    else
                    {
                        return new ValidationResult("All items must be of type IFormFile.");
                    }
                }
            }
            return ValidationResult.Success!;
        }
    }
}
