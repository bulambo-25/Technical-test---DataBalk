using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Workflow.API.Data;
using Workflow.API.Models;
using Workflow.API.Models.Dto;

namespace Workflow.API.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _db;
        public UserController(ILogger<UserController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        // Get All The Users
        [HttpGet]
        [Route("UserList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult <IEnumerable<UserDto>> GetUsers()
        {
            _logger.LogInformation("Getting all users");
            //return Ok(new List<UserDto>()
            //{
            //    new UserDto { Id = 1, Username = "Michel", Email = "michel@gmail.com", Password = "password"}
            //});

            return Ok(_db.tUser.ToList());
        }

        // Get Single User
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult <IEnumerable<UserDto>> GetUser(int id)
        {

            if (id == 0)
                return BadRequest();

            var  user = _db.tUser.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // Create New User
        [HttpPost]
        [Route("CreateUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<UserDto> CreateUser([FromBody]UserDto userDto)
        {

            if (userDto == null)
                return BadRequest(userDto);
            if (userDto.Email == null)
                return StatusCode(StatusCodes.Status409Conflict);
            if (userDto.Id > 0)
                return StatusCode(StatusCodes.Status409Conflict);

            // check if user exist already

            if(_db.tUser.FirstOrDefault(u => u.Email.ToLower() == userDto.Email.ToLower()) != null)
            {
                _logger.LogError("Error Occur: User Already Exist");
                return BadRequest(ModelState);

            }

            User userObj = new() 
            { 
                Username = userDto.Username,
                Email = userDto.Email,
                Password = userDto.Password
            };

            _db.tUser.Add(userObj);
            _db.SaveChanges();
            
            return Ok("User Successfully Created");
        }

        

        [HttpPut("Edit/{id:int}", Name = "Edit")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult EditUser(int id, [FromBody] UserDto userDto)
        {
            try
            {
                if (userDto == null || id != userDto.Id)
                    return BadRequest();

                var existingUser = _db.tUser.FirstOrDefault(u => u.Id == id);

                if (existingUser == null)
                    return NotFound(); 

                // Check if the email is being changed to an existing email
                var existingEmail = _db.tUser.FirstOrDefault(u => u.Id == id);

                if (existingEmail != null && existingEmail.Email != userDto.Email)
                    return BadRequest(); 

                // Update existing user properties
                existingUser.Username = userDto.Username;
                existingUser.Email = userDto.Email;
                existingUser.Password = userDto.Password;

                _db.tUser.Update(existingUser);
                _db.SaveChanges();

                return Ok("User Updated Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error", ex.Message); 
                return BadRequest();
            }
        }



        // Delete Existing User
        [HttpDelete("Delete/{id:int}", Name = "Delete")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteUser(int id)
        {

            if(id == 0)
                return BadRequest();

            var user = _db.tUser.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();
            
            _db.tUser.Remove(user);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
