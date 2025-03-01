using golbym.Api.Domains;
using Microsoft.EntityFrameworkCore;

namespace golbym.Api.Models.ApiResponse
{
    public class PostResponse(Post master)
    {
        public string Id { get; set; } = master.Id;

        public string Title { get; set; } = master.Title;

        public string ReleaseDate { get; set; } = master.ReleaseDate.ToString("MMM-dd-yyyy");

        public string Content { get; set; } = master.Content;

        public string Thumbnail { get; set; } = master.Thumbnail;

        public string Author { get; set; } = master.Author;

        public List<TagResponse> Tags { get; set; } = [.. master.Tags.Select(p => new TagResponse
        {
            Id = p.Id,
            Name = p.Name
        })];
    }

    public static class ExtensionMethods
    {
        public static async Task<List<PostResponse>> ToResponseModel(this IQueryable<Post> postList)
        {
            return await postList.Select(p => new PostResponse(p)).ToListAsync();
        }
    }
}
