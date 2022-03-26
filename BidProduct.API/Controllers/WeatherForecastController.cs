using Microsoft.AspNetCore.Mvc;

namespace BidProduct.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet]
        public int Get()
        {
            return 2;
        }
    }
}
