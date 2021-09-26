using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CSVLoaderAPI.Data;
using CSVLoaderAPI.Models;
using CSVLoaderAPI.Entities;
using CSVLoaderAPI.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidDataACSVLoaderAPInalyzerAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class BasicAuthController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly ILogger _log;

        public BasicAuthController(UserContext userContext, ILogger<BasicAuthController> logger)
        {
            _context = userContext;
            _log = logger;
        }


        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/users/CreateNewUser
        ///{
        ///  "firstName": "chandu",
        ///  "lastName": "udupa",
        ///  "username": "chandu.udupa@gmail.com",
        ///  "email": "chandu.udupa@gmail.com",
        ///  "password": "test1!"
        ///}  
        /// </remarks>
        /// <param name="user"></param>
        /// <returns>A newly created user</returns>
        /// <response code="201">Returns the newly created user</response>
        /// <response code="400">If the user is null</response>   
        /// <response code="404">If the get method after create returns null</response>
        /// <response code="500">If there is some error</response>
        // POST: api/users/CreateNewUser
        [HttpPost("CreateNewUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<ActionResult<User>> CreateNewUser(User user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest();
                }

                _context.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (ArgumentNullException ex)
            {
                _log.CSVLoaderAPISingleLogError(ex.Message + " Stack trace: " + ex.StackTrace);
                return StatusCode(404, new MessageError(ex.Message, ex.StackTrace));
            }
            catch (Exception ex)
            {
                _log.CSVLoaderAPISingleLogError(ex.Message + " Stack trace: " + ex.StackTrace);
                return StatusCode(500, new MessageError(ex.Message, ex.StackTrace));
            }
        }



        /// <summary>
        /// Authenticates the user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/users/authenticate
        ///{
        ///  "username": "chandu.udupa@gmail.com",
        ///  "password": "test1!"
        ///}  
        /// </remarks>
        /// <returns>Authentication result</returns>
        /// <response code="200">Returns OK</response>
        /// <response code="400">If the user is null or No user is registered</response>   
        /// <response code="404">If the get method returns null</response>
        /// <response code="500">If there is some error</response>
        // POST: api/users/Authenticate
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateModel model)
        {
            try
            {
                if (_context.Users.Count() == 0)
                    return BadRequest(new { message = "No user is registered" });

                var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == model.Username && x.Password == model.Password);
                if (user == null)
                    return BadRequest(new { message = "Username or password is incorrect" });

                return Ok(user);
            }
            catch (ArgumentNullException ex)
            {
                _log.CSVLoaderAPISingleLogError(ex.Message + " Stack trace: " + ex.StackTrace);
                return StatusCode(404, new MessageError(ex.Message, ex.StackTrace));
            }
            catch (Exception ex)
            {
                _log.CSVLoaderAPISingleLogError(ex.Message + " Stack trace: " + ex.StackTrace);
                return StatusCode(500, new MessageError(ex.Message, ex.StackTrace));
            }

        }

        /// <summary>
        /// Retrieves all users
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /api/users/GetAll
        ///
        /// </remarks>
        /// <returns>Returns all the users created until now</returns>
        /// <response code="200">If the users are found and returned succesfully</response>
        /// <response code="400">If the user is null or No user is registered</response>   
        /// <response code="500">If there is any error</response>   
        // GET: api/users/GetAll
        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<User>>> GetAll()
        {
            try
            {

                List<User> result = await _context.Users.AsNoTracking()
                                                    .ToListAsync();
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                _log.CSVLoaderAPISingleLogError(ex.Message + " Stack trace: " + ex.StackTrace);
                return StatusCode(404, new MessageError(ex.Message, ex.StackTrace));
            }
            catch (Exception ex)
            {
                _log.CSVLoaderAPISingleLogError(ex.Message + " Stack trace: " + ex.StackTrace);
                return StatusCode(500, new MessageError(ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// Retrieves user data when passed user id
        /// </summary>
        /// <param name="id"></param>
        /// Sample request:
        ///
        /// GET /api/users/GetUser/1
        ///
        /// </remarks>
        /// <returns>Returns the user data where id is passed id</returns>
        /// <response code="200">If the user is found and returned succesfully</response>
        /// <response code="400">If the user is null or No user is registered</response>   
        /// <response code="500">If there is any error</response>   
        // GET: api/users/GetUser/1
        [HttpGet("GetUser/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<User>> GetUser(int id)
        {

            try
            {
                User user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                return user;
            }
            catch (Exception ex)
            {
                _log.CSVLoaderAPISingleLogError(ex.Message + " Stack trace: " + ex.StackTrace);
                return StatusCode(500, new MessageError(ex.Message, ex.StackTrace));
            }
        }

    }
}
