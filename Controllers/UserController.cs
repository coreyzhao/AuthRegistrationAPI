using AuthRegistrationAPI.Models;
using AuthRegistrationAPI.Data;
using AuthRegistrationAPI.Dtos;
using Microsoft.AspNetCore.Mvc;


namespace AuthRegistrationAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    DataContextDapper _dapper;
    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }
   

    
    [HttpGet("GetSingleUser/{userId}")]
    // public IEnumerable<User> GetUsers()
    public User GetSingleUser(int userId)
    {
        string sql = @"
            SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active] 
            FROM AppSchema.Users
                WHERE UserId = " + userId.ToString(); //"7"
        User user = _dapper.LoadDataSingle<User>(sql);
        return user;
    }

    [HttpGet("GetUsers")]
    // public IEnumerable<User> GetUsers()
    public IEnumerable<User> GetUsers()
    {
        string sql = @"
            SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active] 
            FROM AppSchema.Users";
        IEnumerable<User> users = _dapper.LoadData<User>(sql);
        return users;
    }

    


    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"
        UPDATE AppSchema.Users
            SET [FirstName] = '" + user.FirstName + 
                "', [LastName] = '" + user.LastName +
                "', [Email] = '" + user.Email + 
                "', [Gender] = '" + user.Gender + 
                "', [Active] = '" + user.Active + 
            "' WHERE UserId = " + user.UserId;
        

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        } 

        throw new Exception("Error: Unable to Update User");
    }


    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        string sql = @"
            INSERT INTO AppSchema.Users(
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
            ) VALUES (" +
                "'" + user.FirstName + 
                "', '" + user.LastName +
                "', '" + user.Email + 
                "', '" + user.Gender + 
                "', '" + user.Active + 
            "')";


        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        } 

        throw new Exception("Error: Unable to Add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"
            DELETE FROM AppSchema.Users 
                WHERE UserId = " + userId.ToString();


        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        } 

        throw new Exception("Error: Unable to Delete User");
    }




}
