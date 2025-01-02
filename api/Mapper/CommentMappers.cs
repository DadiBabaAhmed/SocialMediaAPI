using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Comment;
using api.Models;

namespace api.Mapper
{
    public static class CommentMappers
    {
        public static CommentDto ToCommentDto(this Comment comment)
        {
            if (comment == null)
            {
                return null;
            }

            return new CommentDto
            {
                CommentId = comment.CommentId,
                PostId = comment.PostId ?? 0,
                CreatedBy = comment.User?.UserName ?? string.Empty,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
            };
        }

        public static Comment ToCommentFromCreateDTO(this CreateCommentRequestDto commentDto, int postId, string userId)
        {
            return new Comment
            {
                UserId = userId,
                PostId = postId,
                Content = commentDto.Content,
            };
        }

        public static Comment ToCommentFromUpdateDTO(this UpdateCommentRequestDto commentDto)
        {
            return new Comment
            {
                Content = commentDto.Content,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}