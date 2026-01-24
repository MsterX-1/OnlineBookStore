namespace Application.Dtos.AuthDto
{
    public class AuthResponseDto
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public int Userid { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? First_Name { get; set; }
        public string? Last_Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpiresOn { get; set; }
    }
}
