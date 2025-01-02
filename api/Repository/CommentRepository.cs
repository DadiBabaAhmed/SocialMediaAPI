using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.DTOs.Comment;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDBContext _context;
        public CommentRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment?> DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == id);
            if (comment == null)
            {
                return null;
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<List<Comment>> GetAllCommentsAsync()
        {
            return await _context.Comments.Include(a => a.User).ToListAsync();
        }

        public async Task<Comment?> GetCommentByIdAsync(int id)
        {
            return await _context.Comments.Include(a => a.User).FirstOrDefaultAsync(c => c.CommentId == id);
        }

        public async Task<Comment?> UpdateCommentAsync(int id, Comment updatedComment)
        {
            var commentModel = await _context.Comments.Include(a => a.User).FirstOrDefaultAsync(c => c.CommentId == id);
            if (commentModel == null)
            {
                return null;
            }

            commentModel.Content = updatedComment.Content;
            commentModel.UpdatedAt = updatedComment.UpdatedAt;

            await _context.SaveChangesAsync();
            return commentModel;
        }
    }
}