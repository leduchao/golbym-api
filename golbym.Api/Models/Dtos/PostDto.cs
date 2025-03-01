using golbym.Api.Domains;

namespace golbym.Api.Models.Dtos
{
	public class PostDto
	{		
		public string Title { get; init; } = string.Empty;

		public string Content { get; init; } = string.Empty;
		
		public string Author { get; set; } = string.Empty;

		public DateTime ReleaseDate { get; set; }
		
		public string Thumbnail { get; set; } = string.Empty;

		public string Tags { get; set; } = string.Empty;

		public Post ToEntity()
		{
			return new Post
			{
				Id = Guid.NewGuid().ToString(),
				Title = Title,
				Content = Content,
				Author = Author,
				ReleaseDate = ReleaseDate,
				Thumbnail = Thumbnail
			};
		}
	}
}
