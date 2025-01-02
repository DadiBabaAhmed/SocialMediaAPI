using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Post;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface IPostRepository
    {
        Task<List<Post>> GetAllPostsAsync(QueryObject queryObject);
        Task<Post?> GetPostByIdAsync(int id);
        Task<Post> CreatePostAsync(Post post);
        Task<Post?> UpdatePostAsync(int id, UpdatePostRequestDto updatedPost);
        Task<Post?> DeletePostAsync(int id);
        Task<bool> PostExists(int id);
    }
}