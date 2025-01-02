using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Post;
using api.Models;

namespace api.Mapper
{
    public static class PostMappers
    {
        public static PostDto ToPostDto(this Post post)
        {
            return new PostDto
            {
                PostId = post.PostId,
                PostedBy = post.User.UserName ?? string.Empty,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Comments = post.Comments.Select(c => c.ToCommentDto()).ToList(),
            };
        }

        public static Post ToPostFromCreateDTO(this CreatePostRequestDto postDto, string userId)
        {
            return new Post
            {
                UserId = userId,
                Title = postDto.Title,
                Content = postDto.Content,
            };
        }
    }
}