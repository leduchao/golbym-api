namespace golbym.Api.Models.Dtos
{
	public class PostDto
	{
		public string Id { get; set; } = string.Empty;
		
		public string Title { get; init; } = string.Empty;

		public string Content { get; init; } = string.Empty;
		
		public string Author { get; set; } = string.Empty;

		public DateOnly ReleaseDate { get; set; }
		
		public string Thumbnail { get; set; } = string.Empty;

		public string Tags { get; set; } = string.Empty;

	}
}
