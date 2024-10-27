using churchWebAPI.DBContext;
using churchWebAPI.DTOs;
using churchWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace churchWebAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class userController : ControllerBase
    {
        #region Load DBContext in Contructor
        private readonly authDBContext _authContext;

        public userController(authDBContext authContext)
        {
            _authContext = authContext;
        }
        #endregion

        [HttpPut("updateProfile")]
        public async Task<IActionResult> UpdateUserProfile(int userId, UpdateProfileDto dto)
        {
            try
            {
                var user = await _authContext.mstRegister.Where(a=>a.MstUserId == userId && a.IsActive == true).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound(new SuccessFailureDto { IsSuccess = false, Message = "User not found." });
                }

                user.Name = dto.Name;
                user.Address = dto.Address;
                user.PhoneNumber = dto.PhoneNumber;
                user.Gender = dto.Gender;
                user.Country = dto.Country;
                user.City = dto.City;
                user.PostalCode = dto.PostalCode;
                user.Occupation = dto.Occupation;

                _authContext.mstRegister.Update(user);
                await _authContext.SaveChangesAsync();

                return Ok(new SuccessFailureDto { IsSuccess = true, Message = "Profile updated successfully.", Data = user });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(HttpContext, ex);
                return StatusCode(500, new SuccessFailureDto { IsSuccess = false, Message = "Something Went Wrong!!" });
            }
        }

        [HttpPut("updatePassword")]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordDto dto)
        {
            try
            {
                var user = await _authContext.mstRegister.FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive == true);
                if (user == null)
                {
                    return NotFound(new SuccessFailureDto { IsSuccess = false, Message = "User not found." });
                }

                if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.Password))
                {
                    return BadRequest(new SuccessFailureDto { IsSuccess = false, Message = "Old password is incorrect." });
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                _authContext.mstRegister.Update(user);
                await _authContext.SaveChangesAsync();

                return Ok(new SuccessFailureDto { IsSuccess = true, Message = "Password updated successfully." });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(HttpContext, ex);
                return StatusCode(500, new SuccessFailureDto { IsSuccess = false, Message = "Something Went Wrong!!" });
            }
        }

        [HttpGet("getUserById")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var user = await _authContext.mstRegister.Where(a => a.MstUserId == userId && a.IsActive == true).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound(new SuccessFailureDto { IsSuccess = false, Message = "User not found." });
                }

                return Ok(new SuccessFailureDto { IsSuccess = true, Message = "User retrieved successfully.", Data = user });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(HttpContext, ex);
                return StatusCode(500, new SuccessFailureDto { IsSuccess = false, Message = "Something Went Wrong!!" });
            }
        }

        [HttpDelete("deleteUserById")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var user = await _authContext.mstRegister.Where(a => a.MstUserId == userId && a.IsActive == true).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound(new SuccessFailureDto { IsSuccess = false, Message = "User not found." });
                }

                user.IsActive = false;
                _authContext.mstRegister.Update(user);
                await _authContext.SaveChangesAsync();

                return Ok(new SuccessFailureDto { IsSuccess = true, Message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(HttpContext, ex);
                return StatusCode(500, new SuccessFailureDto { IsSuccess = false, Message = "Something Went Wrong!!" });
            }
        }

        [HttpGet("getBirthdayForDays")]
        public async Task<IActionResult> GetBirthdayForDays()
        {
            try
            {
                DateTime today = DateTime.Today;

                DateTime[] dates = new DateTime[]{today.AddDays(-1),today,today.AddDays(1)};

                var vResult = _authContext.mstRegister.Where(a=> a.DateOfBirth.Month == DateTime.Now.Month && dates.Contains(a.DateOfBirth.Date)).ToList();

                if (vResult.Count() ==  0)
                {
                    return NotFound(new SuccessFailureDto { IsSuccess = true, Message = "No results found." });

                }
                var vFinalResult = vResult.Select(a => new
                                    {
                                        userId = a.MstUserId,
                                        userName = a.Name,
                                        gender = a.Gender,
                                        dateOfBirth = a.DateOfBirth.ToString("dd-MM-yyyy"),
                                    }).OrderBy(a=>a.dateOfBirth).ToList();


                return Ok(new SuccessFailureDto { IsSuccess = true, Message = "User retrieved successfully.", Data = vFinalResult });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(HttpContext, ex);
                return StatusCode(500, new SuccessFailureDto { IsSuccess = false, Message = "Something Went Wrong!!" });
            }
        }

        [HttpGet("getCompleteUserList")]
        public async Task<IActionResult> GetCompleteUserList()
        {
            try
            {
                var vUserList = _authContext.mstRegister.Where(a => a.IsActive == true).ToList();
                if (vUserList == null)
                {
                    return NotFound(new SuccessFailureDto { IsSuccess = false, Message = "No results found." });
                }

                return Ok(new SuccessFailureDto { IsSuccess = true, Message = "User retrieved successfully.", Data = vUserList });
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(HttpContext, ex);
                return StatusCode(500, new SuccessFailureDto { IsSuccess = false, Message = "Something Went Wrong!!" });
            }
        }

    }
}
