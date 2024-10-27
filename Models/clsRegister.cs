namespace churchWebAPI.Models
{
    public class clsRegister
    {
        public int MstUserId { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }
        public long PhoneNumber { get; set; }
        public char Gender { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public int? PostalCode { get; set; }
        public string? Occupation { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }

}
