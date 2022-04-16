



using BidProduct.IdentityManagement.Quickstart.Consent;

namespace BidProduct.IdentityManagement.Quickstart.Device
{
    public class DeviceAuthorizationInputModel : ConsentInputModel
    {
        public string UserCode { get; set; }
    }
}