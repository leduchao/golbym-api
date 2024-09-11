using golbym.Api.Domains;
using golbym.Api.Models.Dtos;

namespace golbym.Api.Models;

public class PostModel
{
    public string Id { get; set; } = string.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    public string Content { get; set; } = string.Empty;
    
    public string Thumbnail { get; set; } = string.Empty;
    
    public string Author { get; set; } = string.Empty;
    
    public string Tags { get; set; } = string.Empty;
    
    public DateOnly ReleaseDate { get; set; }

    public PostModel()
    {
        
    }

    // convert a dto to model
    public PostModel(PostDto dto)
    {
        Id = dto.Id;
        Title = dto.Title;
        Content = dto.Content;
        Thumbnail = dto.Thumbnail;
        ReleaseDate = dto.ReleaseDate;
    }
    
    // convert an entity to model
    public PostModel(Post entity)
    {
        Id = entity.Id;
        Title = entity.Title;
        Content = entity.Content;
        Thumbnail = entity.Thumnail;
        ReleaseDate = entity.ReleaseDate;
    }

    public Post ToEntity(string author)
    {
        var entity = new Post
        {
            Id = Id,
            Title = Title,
            Content = Content,
            Thumnail = Thumbnail,
            ReleaseDate = ReleaseDate,
            Author = author,
        };

        return entity;
    }

    public PostDto ToDto()
    {
        var dto = new PostDto
        {
            Id = Id,
            Title = Title,
            Content = Content,
            Author = Author,
            ReleaseDate = ReleaseDate,
            Thumbnail = Thumbnail
        };

        // var tags = Tags.Select(t => t.Name).ToList();
        //
        // dto.Tags = string.Join(",", tags);

        return dto;
    }
}