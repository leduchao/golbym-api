using golbym.Api.Domains;
using golbym.Api.Models;
using golbym.Api.Models.ApiResponse;
using golbym.Api.Models.Dtos;
using golbym.Api.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace golbym.Api.Endpoints
{
	public static class PostEndpoints
	{
		public static void MapPostEndpoints(this IEndpointRouteBuilder app)
		{
			var groupEndpoints = app.MapGroup("api/v1/posts").WithTags("Posts");

			groupEndpoints.MapGet("{id}", GetById);
			groupEndpoints.MapGet("related/{postId}", GetRelatedPosts);
			groupEndpoints.MapGet("", GetPosts);
			groupEndpoints.MapPost("", CreatePost).DisableAntiforgery();//.RequireAuthorization("admin_role");
			groupEndpoints.MapPut("update/{postId}", UpdatePost).DisableAntiforgery();
			groupEndpoints.MapDelete("delete/{id}", DeletePost);
		}

		private static async Task<IResult> GetById(PostRepository postRepository, string id)
		{
			var post = await postRepository.GetByIdAsync(id);

			if (post is null)
				return TypedResults.NotFound("This post is not found");

			var model = new PostModel(post);
			var postDto = model.ToDto();

			var tags = post.Tags.Select(t => t.Name).ToString() ?? "";
			
			if (string.IsNullOrEmpty(tags))
				postDto.Tags = string.Join(",", tags);

			return TypedResults.Ok(postDto);
		}

		private static async Task<IResult> GetRelatedPosts(PostRepository postRepository, string postId)
		{
			var post = await postRepository.GetByIdAsync(postId);

			if (post is null)
				return TypedResults.NotFound("Cannot find post");

			var tagName = post.Tags.Select(t => t.Name).ToList();

			var relatedPosts = await postRepository.GetRelatedPostsByTagNameAsync(postId, tagName);

			return TypedResults.Ok(relatedPosts);
		}

		private static Ok<PagedResult<Post>> GetPosts(PostRepository postRepository, string keyword = "", int page = 1, int numberPosts = 9)
		{

			var posts = postRepository.GetPostByKeyword(keyword);

			// if (posts is null)
			// 	return TypedResults.NotFound("No post founded!");

			var totalItems = posts.Count();

			posts = posts.Skip((page - 1) * numberPosts).Take(numberPosts);

			var pagedResult = new PagedResult<Post>
			{
				CurrentPage = page,
				PageSize = numberPosts,
				TotalItems = totalItems,
				TotalPages = (int)Math.Ceiling(totalItems / (double)numberPosts),
				Items = [.. posts],
			};

			return TypedResults.Ok(pagedResult);
		}

		private static async Task<IResult> CreatePost(PostRepository postRepository, [FromForm] PostDto postDto, IFormFile? thumbnail)
		{
			// if (postDto is null)
			// 	return TypedResults.BadRequest("Cannot create post");

			postDto.Id = Guid.NewGuid().ToString();
			postDto.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);
			
			var model = new PostModel(postDto);

			var newPost = model.ToEntity("Admin");

			var tagList = postDto.Tags.Replace(" ", "").Split(",");

			foreach (var tagName in tagList)
			{
				if (string.IsNullOrEmpty(tagName)) continue;
				
				var tag = await postRepository.GetTagByNameAsync(tagName);

				tag ??= new Tag
				{
					Id = Guid.NewGuid().ToString(),
					Name = tagName,
				};

				newPost.Tags.Add(tag);
			}

			if (thumbnail is not null) newPost.Thumnail = await UploadFile(thumbnail, newPost.Id);
			else newPost.Thumnail = "No image";

			await postRepository.AddAsync(newPost);
			await postRepository.SaveChangesAsync();

			return TypedResults.Ok(newPost);
		}

		private static async Task<IResult> UpdatePost(
			PostRepository postRepository,
			string postId,
			[FromForm] PostDto postDto,
			IFormFile? thumbnail)
		{
			// if (postDto is null)
			// 	return TypedResults.BadRequest("Cannot update post");

			var existPost = await postRepository.GetByIdAsync(postId);

			if (existPost is null)
			{
				return TypedResults.BadRequest("This post is not exist!");
			}

			existPost.Title = postDto.Title;
			existPost.Content = postDto.Content;

			//existPost.Author = "Admin";
			//existPost.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);

			var tagList = postDto.Tags
				.Replace(" ", "")
				.Split(",");

			foreach (var tagName in tagList)
			{
				if (!string.IsNullOrEmpty(tagName))
				{
					var tag = await postRepository.GetTagByNameAsync(tagName);

					tag ??= new Tag
					{
						Id = Guid.NewGuid().ToString(),
						Name = tagName,
					};

					existPost.Tags.Add(tag);
				}
			}

			if (thumbnail is not null)
			{
				File.Delete(Directory.GetCurrentDirectory() + "/Uploads/" + existPost.Thumnail);
				existPost.Thumnail = await UploadFile(thumbnail, postId);
			}

			postRepository.Update(existPost);
			await postRepository.SaveChangesAsync();

			return TypedResults.Ok("Update successfully!");
		}

		private static async Task<IResult> DeletePost(PostRepository postRepository, string id)
		{
			var post = await postRepository.GetByIdAsync(id);

			if (post is null)
				return TypedResults.NotFound("There is no post with id=" + id);

			File.Delete(Directory.GetCurrentDirectory() + "/Uploads/" + post.Thumnail);

			postRepository.Delete(post);
			await postRepository.SaveChangesAsync();

			return TypedResults.Ok("Delete successfully");
		}

		private static async Task<string> UploadFile(IFormFile file, string postId)
		{
			string[] validExtensions = [".jpg", ".png", ".jpeg"];
			var extension = Path.GetExtension(file.FileName);

			if (!validExtensions.Contains(extension))
				return "File extension is invalid";

			var fileSize = file.Length;
			const int maxSize = 5 * 1024 * 1024;

			if (fileSize > maxSize)
				return "File size is not greater than 5Mb";

			var fileName = postId + file.FileName;

			var path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
			await using var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create);
			await file.CopyToAsync(fileStream);

			return fileName;
		}
	}
}
