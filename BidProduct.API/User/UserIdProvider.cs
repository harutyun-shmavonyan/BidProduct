using BidProduct.SL.Abstract;

namespace BidProduct.API.User
{
    public class UserIdProvider : IUserIdProvider
    {
        public UserIdProvider(IHttpContextAccessor httpContextAccessor)
        {
            UserId = Convert.ToInt32(httpContextAccessor.HttpContext?.User.FindFirst(c => c.Type == "UserId")?.Value);
        }

        public int UserId { get; }
    }
}
