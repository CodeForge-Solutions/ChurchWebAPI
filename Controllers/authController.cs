using churchWebAPI.DBContext;
using churchWebAPI.DTOs;
using churchWebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;

namespace churchWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class authController : ControllerBase
    {
        #region Load DBContext in Contructor
        private readonly authDBContext _authContext;
        private readonly TokenService _tokenService;

        public authController(authDBContext authContext, TokenService tokenService)
        {
            _authContext = authContext;
            _tokenService = tokenService;
        }
        #endregion

        [HttpPost("userRegister")]
        public async Task<IActionResult> userRegister(RegisterDto registerDto)
        {
            try
            {
                var existingUser = await _authContext.mstRegister.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
                if (existingUser != null)
                {
                    return BadRequest(new SuccessFailureDto { IsSuccess = false, Message = "User with this email already exists." });
                }

                var user = new clsRegister
                {
                    Name = registerDto.Name,
                    DateOfBirth = registerDto.DateOfBirth,
                    Address = registerDto.Address,
                    PhoneNumber = registerDto.PhoneNumber,
                    Gender = registerDto.Gender,
                    Email = registerDto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    ConfirmPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    Country = registerDto.Country,
                    City = registerDto.City,
                    PostalCode = registerDto.PostalCode,
                    Occupation = registerDto.Occupation,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };


                _authContext.mstRegister.Add(user);
                await _authContext.SaveChangesAsync();

                return Ok(new SuccessFailureDto { IsSuccess = true,Message = "Your data is been added to us!!",Data = user});
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(HttpContext, ex);
                return StatusCode(500, new SuccessFailureDto { IsSuccess = false, Message = "Something Went Wrong!!" });
            }           
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginDto dto)
        {
            try
            {
                // Find the user in the database
                var user = await _authContext.mstRegister.Where(u => u.PhoneNumber == Convert.ToInt64(dto.PhoneNumber) && u.IsActive == true).FirstOrDefaultAsync();
                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                {
                    return Unauthorized(new SuccessFailureDto { IsSuccess = false, Message = "Invalid login credentials." });
                }

                // Generate the JWT token
                var token = _tokenService.GenerateJwtToken(user.Email);

                // Return the token in the response
                return Ok(new SuccessFailureDto
                {
                    IsSuccess = true,
                    Message = "Login successful.",
                    Data = new
                    {
                        Token = token,
                        User = user
                    }
                });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(HttpContext, ex);
                return StatusCode(500, new SuccessFailureDto { IsSuccess = false, Message = "Something Went Wrong!!" });
            }
        }

    }
}
