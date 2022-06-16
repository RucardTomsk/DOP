using APIDOP.Models.DTO;
using APIDOP.Models.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using APIDOP.Enums;
using APIDOP.Services;

namespace APIDOP.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthenticateController: ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthenticateService _authenticateService;

        public AuthenticateController(
            UserManager<User> userManager,
            IAuthenticateService authenticateService)
        {
            _userManager = userManager;
            _authenticateService = authenticateService;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                var token = _authenticateService.LoginUser(model);
                if (token != null) 
                {
                    return Ok(new
                    {
                        token = token.Result
                    });
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,ex.ToString());
            }
        
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User already exists!" });

            
            if(_authenticateService.AddUser(model).Result)
                return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
            
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User creation failed! Please check user details and try again." });
        }

    }
}
