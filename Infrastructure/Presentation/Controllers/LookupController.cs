using Blood_Donation.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Cache]
    [AllowAnonymous]
    public class LookupController(IServiceManager _serviceManager) : ApiBaseController
    {
        [HttpGet("blood-types")]
        public async Task<ActionResult<IEnumerable<BloodTypeDTo>>> GetBloodTypes()
        {
            return Ok(await _serviceManager.LookupService.GetAllBloodTypesAsync());
        }

        [Authorize]
        [HttpGet("donation-categories")]
        public async Task<ActionResult<IEnumerable<DonationCategoriesDTo>>> GetDonationCategories()
        {
            return Ok(await _serviceManager.LookupService.GetAllCategoriesAsync());
        }
        [HttpGet("governorates")]
        public async Task<ActionResult<IEnumerable<GovernorateDTo>>> GetAllGovernorates()
        {
            return Ok(await _serviceManager.LookupService.GetAllGovernorateDToAsync());
        }
        [HttpGet("governorate-cities")]
        public async Task<ActionResult<IEnumerable<CityDTo>>> GetAllCities(int id)
        {
            return Ok(await _serviceManager.LookupService.GetAllCitiesDToAsync(id));
        }
    }
}
