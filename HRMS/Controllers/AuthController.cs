using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HRMS.DbContexts;
using HRMS.Dtos.Auth;
using HRMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HRMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly HRMSContext _dbcontext;
        public AuthController(HRMSContext context)
        {
            _dbcontext = context;
        }

        [HttpPost("Login")]
        public IActionResult Login ( [FromBody] LoginDto loginDto)
        {
            try 
            {
                // Admin, admin, ADMIN so er use toupper()
                var user = _dbcontext.users.FirstOrDefault(x => x.UserName.ToUpper() == loginDto.UserName.ToUpper());
                
                if (user == null) // the firstOrDefault will return null (here we verify that the username is in the database)
                {
                    return BadRequest("Invalid Username Or Password");
                }

                // here we checkeed if the password is correct 
                // we need to decrypt the hashed password to compare it with the user input 
                if(!(BCrypt.Net.BCrypt.Verify(loginDto.Password, user.HashedPassword)))
                {
                    return BadRequest("Invalid Username Or Password");
                }

                var token = GenerateJwtToken(user); // Create Token 

                return Ok(token);



            } 

            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }


        // this is not an endpoint 
        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>(); // user information 

            // key ==> value 
            // id --> 1 
            // Name --> admin  

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())); // instead of writing "id" we will use this class named ClaimTypes.NamrIdentifier
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));

            // Role --> Hr, Manager , Developer, Admin 
            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "admin"));
            }
            else
            {
                var employee = _dbcontext.Employees.Include(x => x.Lookup).FirstOrDefault(x => x.UserId == user.Id);
                claims.Add(new Claim(ClaimTypes.Role, employee.Lookup.Name));
            }


            // Secret Key = WHAFWEI#!@S!!112312WQEQW@RWQEQW432
            // [] we have to convert each charactar to its ascii code and then give it to the funcion  
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("WHAFWEI#!@S!!112312WQEQW@RWQEQW432"));

            // signing the token and to ensure it's a valid token 
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenSettings = new JwtSecurityToken(
                claims: claims, // user info 
                signingCredentials: cred,  // Encryption settings 
                expires: DateTime.Now.AddDays(1) // token expiry date 
                );

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenSettings);

            return token; 
        }




    }
}
