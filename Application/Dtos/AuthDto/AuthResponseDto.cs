namespace Application.Dtos.AuthDto
{
    public class AuthResponseDto
    {
        public required string Token { get; set; }
        public DateTime TokenExpiresOn { get; set; }

    }
}
