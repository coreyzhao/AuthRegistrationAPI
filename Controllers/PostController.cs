using AuthRegistrationAPI.Data;
using AuthRegistrationAPI.Dtos;
using AuthRegistrationAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthRegistrationAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("Posts")]
        public IEnumerable<Post> GetPosts()
        {
            string sql = @"SELECT [PostId], [UserId], [PostTitle], [PostContent], [PostCreated], [PostUpdated]
                           FROM AppSchema.Posts";
            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("PostSingle/{postId}")]
        public ActionResult<Post> GetPostSingle(int postId)
        {
            string sql = $@"SELECT [PostId], [UserId], [PostTitle], [PostContent], [PostCreated], [PostUpdated]
                           FROM AppSchema.Posts
                           WHERE PostId = {postId}";

            var post = _dapper.LoadDataSingle<Post>(sql);
            if (post == null) return NotFound("Post not found");
            return Ok(post);
        }

        [HttpGet("PostsByUser/{userId}")]
        public IEnumerable<Post> GetPostsByUser(int userId)
        {
            string sql = $@"SELECT [PostId], [UserId], [PostTitle], [PostContent], [PostCreated], [PostUpdated]
                           FROM AppSchema.Posts
                           WHERE UserId = {userId}";

            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            var userId = User.FindFirst("userId")?.Value;
            string sql = $@"SELECT [PostId], [UserId], [PostTitle], [PostContent], [PostCreated], [PostUpdated]
                           FROM AppSchema.Posts
                           WHERE UserId = {userId}";

            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("PostsBySearch/{searchParam}")]
        public IEnumerable<Post> PostsBySearch(string searchParam)
        {
            string sql = $@"SELECT [PostId], [UserId], [PostTitle], [PostContent], [PostCreated], [PostUpdated]
                           FROM AppSchema.Posts
                           WHERE PostTitle LIKE '%{searchParam}%' OR PostContent LIKE '%{searchParam}%'";

            return _dapper.LoadData<Post>(sql);
        }

        [HttpPost("Post")]
        public IActionResult AddPost(PostToAddDto postToAdd)
        {
            var userId = User.FindFirst("userId")?.Value;
            string sql = $@"INSERT INTO AppSchema.Posts([UserId], [PostTitle], [PostContent], [PostCreated], [PostUpdated])
                            VALUES ({userId}, '{postToAdd.PostTitle}', '{postToAdd.PostContent}', GETDATE(), GETDATE())";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok("Post created successfully");
            }

            return StatusCode(500, "Error: Unable to create new post");
        }

        [HttpPut("Post")]
        public IActionResult EditPost(PostToEditDto postToEdit)
        {
            var userId = User.FindFirst("userId")?.Value;
            string sql = $@"UPDATE AppSchema.Posts
                           SET PostContent = '{postToEdit.PostContent}', PostTitle = '{postToEdit.PostTitle}', PostUpdated = GETDATE()
                           WHERE PostId = {postToEdit.PostId} AND UserId = {userId}";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok("Post updated successfully");
            }

            return StatusCode(500, "Error: Failed to edit post");
        }

        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            var userId = User.FindFirst("userId")?.Value;
            string sql = $@"DELETE FROM AppSchema.Posts 
                           WHERE PostId = {postId} AND UserId = {userId}";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok("Post deleted successfully");
            }

            return StatusCode(500, "Error: Failed to delete post");
        }
    }
}
