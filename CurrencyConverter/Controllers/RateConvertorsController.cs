using CurrencyConverter.Models;
using CurrencyConverter.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.Controllers
{
    /// <summary>
    /// Controller <see cref="RateConvertorsController"/> displays best possible conversion rate that yield the highest possible amount of currency that you are converting
    /// </summary>
    [ApiController]    
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class RateConvertorsController : ControllerBase
    {        
        private readonly ILogger<RateConvertorsController> _logger;
        private readonly IRatesService _ratesService;

        /// <summary>
        /// Constructor for <see cref="RateConvertorsController"/>
        /// </summary>
        /// <param name="logger"><see cref="ILogger{RateConvertorsController}"/></param>
        /// <param name="ratesService"><see cref="IRatesService"/></param>
        public RateConvertorsController(ILogger<RateConvertorsController> logger, 
                                        IRatesService ratesService)
        {
            _logger = logger;
            _ratesService = ratesService;
        }

        /// <summary>
        /// Provides list of possible conversion rate that yield the highest possible amount of currency
        /// </summary>
        /// <param name="baseCurrency">Currency e.g. CAD</param>
        /// <param name="amount">Amount e.g. 100</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ConversionRate>))]
        public async Task<ActionResult> Get(string baseCurrency, int amount)
        {
            try
            {
                var result = await _ratesService.GetRates(baseCurrency, amount);
                return StatusCode(200, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, "Internal error.");
            }
        }
    }
}