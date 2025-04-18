﻿using System.ComponentModel.DataAnnotations;
using TaskList.CustomActionFilters;

namespace TaskList.Models.DTO
{
    public class UploadImageDto
    {
        [Required(ErrorMessage = "Image file is required")]
        [DataType(DataType.Upload)]
        [MaxFileSize(5 * 1024 * 1024)] // 5MB
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" })]
        public IFormFile Image { get; set; }
    }
}
