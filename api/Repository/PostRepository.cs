using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.DTOs.Post;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDBContext _context;
        public PostRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<Post> CreatePostAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post?> DeletePostAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return null;
            }
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<List<Post>> GetAllPostsAsync(QueryObject queryObject)
        {
            var posts = _context.Posts.Include(p => p.User).Include(c => c.Comments).ThenInclude(c => c.User).AsQueryable();
            if (!string.IsNullOrWhiteSpace(queryObject.Title))
            {
                posts = posts.Where(p => p.Title.Contains(queryObject.Title));
            }
            if (!string.IsNullOrWhiteSpace(queryObject.Content))
            {
                posts = posts.Where(p => p.Content.Contains(queryObject.Content));
            }
            if (!string.IsNullOrWhiteSpace(queryObject.SortBy))
            {
                if(queryObject.SortBy.Equals("title", StringComparison.OrdinalIgnoreCase))
                {
                    posts =queryObject.IsSortAscending ? posts.OrderBy(p => p.Title) : posts.OrderByDescending(p => p.Title);
                }
                if(queryObject.SortBy.Equals("content", StringComparison.OrdinalIgnoreCase))
                {
                    posts =queryObject.IsSortAscending ? posts.OrderBy(p => p.Content) : posts.OrderByDescending(p => p.Content);
                }
            }
            var skipNumber = (queryObject.PageNumber - 1) * queryObject.PageSize;

            return await posts.Skip(skipNumber).Take(queryObject.PageSize).ToListAsync();
        }

        public async Task<Post?> GetPostByIdAsync(int id)
        {
            return await _context.Posts.Include(p => p.User).Include(c => c.Comments).ThenInclude(u => u.User).FirstOrDefaultAsync(p => p.PostId == id);
        }

        public Task<bool> PostExists(int id)
        {
            return _context.Posts.AnyAsync(e => e.PostId == id);
        }

        public async Task<Post?> UpdatePostAsync(int id, UpdatePostRequestDto updatedPostDto)
        {
            var postModel = await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(x => x.PostId == id);
            if (postModel == null)
            {
                return null;
            }
            postModel.Title = updatedPostDto.Title;
            postModel.Content = updatedPostDto.Content;
            postModel.UpdatedAt = updatedPostDto.UpdatedAt;
            await _context.SaveChangesAsync();

            return postModel;
        }
    }
}