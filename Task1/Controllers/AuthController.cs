using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using Task1.DTO.Auth;
using Task1.Models;
using Task1.Services;

namespace Task1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;
        private readonly JwtService _jwtService;
        public AuthController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config, EmailService emailService, SignInManager<User> signInManager, JwtService jwtService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _emailService = emailService;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("login")]
        public async Task<ActionResult<userDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.UserName);

            if (user == null)
            {
                return BadRequest("Invalid username or password.");
            }

            if (!user.EmailConfirmed)
            {
                return BadRequest("Please confirm your email before logging in.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.IsLockedOut)
            {
                return BadRequest($"Your account has been locked. Try again after {user.LockoutEnd?.UtcDateTime}.");
            }

            if (!result.Succeeded)
            {
                return Unauthorized("Invalid username or password.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            Console.WriteLine("UserRoles: " + string.Join(", ", userRoles));

            return await CreateApplicationUserDto(user, string.Join(", ", userRoles));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (await CheckEmailExistAsync(model.Email))
            {
                return BadRequest($"An existing account is using {model.Email}, email addres. Please try with another email address ");
            }

            var UserTOAdd = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
                DateCreated = DateTime.Now,
            };

            var result = await _userManager.CreateAsync(UserTOAdd, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }
            await _userManager.AddToRoleAsync(UserTOAdd, "User");

            try
            {
                if (await sendConfirmEmail(UserTOAdd))
                {
                    return Ok("Email sent for varification.");
                }
                return BadRequest("Failed to send email. Please Check your email");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Register] Email sending failed: {ex.Message}");
                return BadRequest("Failed to send email. Please contact admin");
            }
        }
        [HttpPut("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto model)
        {
            User user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("The user with this email doest not exists");
            }
            if (user.EmailConfirmed == true)
            {
                return BadRequest("Your email was confirmed before. Please login to your account");
            }
            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Email confirmed", message = "Your email address is confirmed. You can login now" }));
                }

                return BadRequest("Invalid token. Please try again");

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    title = "Invalid token.",
                    message = ex.Message

                });
            }
        }

        [HttpPost("forgot-username-or-password/{email}")]
        public async Task<IActionResult> ForgotUsernameOrPassword(string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Invalid email");

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return Unauthorized("This email address has not been registerd yet");
            if (user.EmailConfirmed == false) return BadRequest("Please confirm your email address first.");

            try
            {
                if (await SendForgotUsernameOrPasswordEmail(user))
                {
                    return Ok(new JsonResult(new { title = "Forgot username or password email sent", message = "Please check your email" }));
                }

                return BadRequest("Failed to send email. Please contact admin");
            }
            catch (Exception)
            {
                return BadRequest("Failed to send email. Please contact admin");
            }
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized("This email address has not been registerd yet");
            if (user.EmailConfirmed == false) return BadRequest("PLease confirm your email address first");

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Password reset success", message = "Your password has been reset" }));
                }

                return BadRequest("Invalid token. Please try again");
            }
            catch (Exception)
            {
                return BadRequest("Invalid token. Please try again");
            }
        }

        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole(string RoleName)
        {
            var role = new IdentityRole(RoleName);
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return Ok("Role is added ");

            }
            return BadRequest($"User is not Added due to error {result.Errors}");
        }

        [HttpPost("get-roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            if (roles.Count < 1)
            {
                return BadRequest($"No roles added in the database use add role to add data");
            }
            return BadRequest($"Roles are : {roles.ToJson()}");
        }

        [HttpPost("set-user-role")]
        public async Task<IActionResult> SetUserToRole(string Username, string RoleName)
        {
            var roleexists = await _roleManager.RoleExistsAsync(RoleName);
            if (!roleexists)
            {
                return BadRequest($"Specified role '{RoleName}' does not exist.");
            }
            var user = await _userManager.FindByNameAsync(Username);
            if (user == null)
            {
                return BadRequest($"User with Specified username '{Username}' does not exist.");
            }
            var result = await _userManager.AddToRoleAsync(user, RoleName);
            if (result.Succeeded)
            {
                return Ok($"User '{Username}' has been added to role '{RoleName}'.");
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("get-user-roles")]
        public async Task<IActionResult> GetUserRoles(string Username)
        {
            var user = await _userManager.FindByNameAsync(Username);
            if (user == null)
            {
                return BadRequest($"User with Specified username '{Username}' does not exist.");
            }
            var userroles = await _userManager.GetRolesAsync(user);
            if (userroles == null)
            {
                return BadRequest("User has no roles");
            }
            return Ok($"Roles of {Username} are {userroles.ToJson()} ");
        }

        [HttpPost("remove-from-role")]
        public async Task<IActionResult> RemovefromRole(string Username, string RoleName)
        {
            var roleexists = await _roleManager.RoleExistsAsync(RoleName);
            if (!roleexists)
            {
                return BadRequest($"Specified role '{RoleName}' does not exist.");
            }
            var user = await _userManager.FindByNameAsync(Username);
            if (user == null)
            {
                return BadRequest($"User with Specified username '{Username}' does not exist.");
            }
            var IsInrole = await _userManager.IsInRoleAsync(user, RoleName);
            if (!IsInrole)
            {
                return BadRequest($"{Username} is not in {RoleName} role");
            }
            var result = _userManager.RemoveFromRoleAsync(user, RoleName);
            if (result.Result == IdentityResult.Success)
            {
                return Ok($"Removed from role successfully");
            }
            return BadRequest($"Can not remove the role, {result.Result}");
        }

        private async Task<bool> CheckEmailExistAsync(string email)
        {
            return await _userManager.Users.AnyAsync(user => user.Email == email);

        }
        private async Task<bool> sendConfirmEmail(User model)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(model);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(token);
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            var url = $"http://localhost:4200/confirmemail?email={model.Email}&token={token}";


            string buttonStyle = "display: inline-block; padding: 10px 20px; font-size: 16px; font-weight: bold; color: #ffffff; background-color: #007bff; text-decoration: none; border-radius: 5px; text-align: center;";

            var bodyWithAlternative =
                $"<p>Hello {model.FirstName} {model.LastName},</p>" +
                "<p>Thank you for registering. Please confirm your email address by clicking the button below:</p>" +

                $"<p style=\"text-align: center; margin-top: 20px; margin-bottom: 20px;\">" +
                    $"<a href=\"{url}\" style=\"{buttonStyle}\">Confirm Your Email Address</a>" +
                $"</p>" +

                "<p>If you're having trouble clicking the button, you can also confirm your email by copying and pasting the following link into your web browser's address bar:</p>" +
                $"<p><a href=\"{url}\">{url}</a></p>" +


                "<p>If you did not request this email, please ignore it.</p>" +
                "<p>Thank you,</p>" +
                $"<br>{_config["EmailSettings:SenderName"]}";

            Console.WriteLine(bodyWithAlternative);

            var emailSend = new EmailSendDto(model.Email, "Confirm your email", bodyWithAlternative);

            return await _emailService.SendEmailAsync(emailSend);
        }

        private async Task<bool> SendForgotUsernameOrPasswordEmail(User user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"http://localhost:4200/resetPassword?token={token}&email={user.Email}";

            var body = $"<p>Hello: {user.FirstName} {user.LastName}</p>" +
               $"<p>Username: {user.UserName}.</p>" +
               "<p>In order to reset your password, please click on the following link.</p>" +
               $"<p><a href=\"{url}\">Click here</a></p>" +
               "<p>Thank you,</p>" +
               $"<br>{_config["EmailSettings:SenderName"]}";

            var emailSend = new EmailSendDto(user.Email, "Forgot username or password", body);

            return await _emailService.SendEmailAsync(emailSend);
        }

        private async Task<userDto> CreateApplicationUserDto(User user, string userroles )
        {
            return new userDto
            {
                Role = userroles,
                FirstName = user.FirstName,
                LastName = user.LastName,
                JWT = await _jwtService.GenerateToken(user),
            };
        }


    }
}
