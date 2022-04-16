using BidProduct.SL.Abstract;

namespace BidProduct.API.User
{
    public class UserIdProvider : IUserIdProvider
    {
        public UserIdProvider(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User.FindFirst(c => c.Type == "userId")?.Value;
        }

        public string? UserId { get; }
    }
}
