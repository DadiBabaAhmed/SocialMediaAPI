using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs.Comment
{
    public class CreateCommentRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "Content must be at least 10 characters long")]
        [MaxLength(280, ErrorMessage = "Content must be at most 500 characters long")]
        public string Content { get; set; } = string.Empty;
    }
}