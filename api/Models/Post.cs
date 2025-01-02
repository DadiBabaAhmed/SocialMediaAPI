using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using api.Mapper;

namespace api.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public string? UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
        public User? User { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}