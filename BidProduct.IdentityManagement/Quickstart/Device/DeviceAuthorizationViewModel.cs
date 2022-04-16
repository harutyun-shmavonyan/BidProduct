



using BidProduct.IdentityManagement.Quickstart.Consent;

namespace BidProduct.IdentityManagement.Quickstart.Device
{
    public class DeviceAuthorizationViewModel : ConsentViewModel
    {
        public string UserCode { get; set; }
        public bool ConfirmUserCode { get; set; }
    }
}