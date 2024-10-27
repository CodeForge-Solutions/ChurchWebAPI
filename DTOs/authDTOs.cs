namespace churchWebAPI.DTOs
{
    public class authDTOs
    {
    }

    public class RegisterDto
    {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }
        public long PhoneNumber { get; set; }
        public char Gender { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public int? PostalCode { get; set; }
        public string? Occupation { get; set; }
    }

    public class UpdatePasswordDto
    {
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class LoginDto
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }

    public class UpdateProfileDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public long PhoneNumber { get; set; }
        public char Gender { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int PostalCode { get; set; }
        public string Occupation { get; set; }
    }

}
