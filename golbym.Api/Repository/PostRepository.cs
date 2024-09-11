
using golbym.Api.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using golbym.Api.Domains;

namespace golbym.Api.Repository
{
	public class PostRepository(AppDbContext context) : IRepository<Post>
	{
		public async Task AddAsync(Post entity)
		{
			await context.Posts.AddAsync(entity);
		}

		public void Delete(Post entity)
		{
			context.Posts.Remove(entity);
		}

		public IQueryable<Post> GetAll()
		{
			return context.Posts.AsQueryable();
		}

		public async Task<Post?> GetByIdAsync(string id)
		{
			return await context.Posts
				.Include(p => p.Tags)
				.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task SaveChangesAsync()
		{
			await context.SaveChangesAsync();
		}

		public void Update(Post entity)
		{
			context.Posts.Update(entity);
		}

		public async Task<Tag?> GetTagByNameAsync(string tagName)
		{
			var tag = await context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);

			return tag;
		}

		public async Task<List<Post>> GetRelatedPostsByTagNameAsync(string postId, List<string> tagName)
		{
			return await context.Posts
				.Where(p => p.Id != postId && p.Tags.Any(t => tagName.Contains(t.Name)))
				.Take(5)
				.ToListAsync();
		}

		public IQueryable<Post> GetPostByKeyword(string keyword)
		{
			var posts = GetAll();

			if (string.IsNullOrEmpty(keyword) || string.IsNullOrWhiteSpace(keyword))
				return posts.OrderByDescending(p => p.ReleaseDate);
			
			keyword = keyword.ToLower().Trim().Replace(" ", "");

			posts = posts.Where(p =>
					p.Title.ToLower().Trim().Replace(" ", "").Contains(keyword) ||
					p.Tags.Any(t => t.Name.ToLower().Trim().Replace(" ", "").Contains(keyword)));

			return posts.OrderByDescending(p => p.ReleaseDate); ;
		}
	}
}
