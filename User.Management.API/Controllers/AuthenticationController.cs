using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using User.Management.API.Models;
using User.Management.API.Models.Authentication.SignUp;
using User.Management.Service.Models;
using User.Management.Service.Services;

namespace User.Management.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthenticationController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, string role)
        {
            // Check User Exist
            IdentityUser? userExist = await _userManager.FindByEmailAsync(registerUser.Email);

            if (userExist is not null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new Response { Status = "Error", Message = "User Already exists!" });
            }

            // Add The User to dataBase

            IdentityUser user = new()
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.UserName
            };
            if (await _roleManager.RoleExistsAsync(role))
            {
                IdentityResult result = await _userManager.CreateAsync(user, registerUser.Password);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User Faild to Create!" });
                }
                // Add Role to the User
                await _userManager.AddToRoleAsync(user, role);
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "User Created SuccessFully!" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "This Role Doesnot Exist." });
            }

        }

        [HttpGet]          
        public IActionResult TestEmail()
        {
            var message = new Message(new string[] { "playboypharaohgbp@gmail.com" }, "Test", "Test...");
            _emailService.SendEmail(message);
            
            return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Email Sent SuccessFully!" });
        }

    }
}
