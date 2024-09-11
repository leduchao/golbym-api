namespace golbym.Api.Domains
{
	public class Post
	{
		public string Id { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
		public string Thumnail { get; set; } = string.Empty;
		public string Author { get; set; } = string.Empty;
		public DateOnly ReleaseDate { get; set; }
		public List<Tag> Tags { get; set; } = [];
	}
}
