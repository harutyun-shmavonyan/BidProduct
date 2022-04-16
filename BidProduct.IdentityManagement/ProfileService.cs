using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BidProduct.IdentityManagement.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace BidProduct.IdentityManagement
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<BidProductUser> _userManager;

        public ProfileService(UserManager<BidProductUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);

            var claims = new List<Claim>
            {
                new Claim("userId", user.Id),
            };

            context.IssuedClaims.AddRange(claims);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}