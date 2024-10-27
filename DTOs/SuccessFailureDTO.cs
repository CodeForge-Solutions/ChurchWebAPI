namespace churchWebAPI.DTOs
{
    public class SuccessFailureDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; } = String.Empty;
    }

}
