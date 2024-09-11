using System.ComponentModel.DataAnnotations;

namespace golbym.Api.Domains
{
	public class Tag
	{
		[MaxLength(50)]
		public string Id { get; set; } = null!;
		
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;
		
		public List<Post> Posts { get; set; } = [];
	}
}
