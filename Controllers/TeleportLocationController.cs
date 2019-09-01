using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TravelAppRestApi.Models;
using TravelAppRestApi.Services;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Http;
using TravelAppRestApi.Models.ModelsDTO;
using TravelAppRestApi.Models.TeleportApiModels;

namespace TravelAppRestApi.Controllers
{
    [Route("api/teleport/locations/")]
    [ApiController]
    public class TeleportLocationController : ControllerBase
    {

        private TeleportLocationService TeleportLocationService { get; set; }

        public TeleportLocationController(TeleportLocationService TeleportLocationService)
        {
            this.TeleportLocationService = TeleportLocationService;
        }

        [HttpGet("{locationName}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public ActionResult<ICollection<TeleportLocationModel>> findAllTeleportLocations([FromRoute] string locationName)
        {
            ICollection<TeleportLocationModel> teleportLocationModels = TeleportLocationService.GetLocationsByLocationName(locationName);
            if(teleportLocationModels == null || teleportLocationModels.Count == 0)
                return NotFound();
            return new JsonResult(teleportLocationModels);
        }

        [HttpPost("atributes/country")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public ActionResult<TeleportLocationCountryModel> findLocationCountryInfo([FromBody] CountryLinkDTO countryLinkDTO)
        {
            TeleportLocationCountryModel teleportCountryInfo = TeleportLocationService.GetTeleportLocationCountryModel(countryLinkDTO.CountryLink);
            if(teleportCountryInfo == null)
                return NotFound();
            return new JsonResult(teleportCountryInfo);
        }

        [HttpPost("atributes/details")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public ActionResult<ICollection<TeleportLocationDetailsModel>> findLocationDetails([FromBody] UrbanAreaDetailsLinkDTO urbanAreaDetailsLink)
        {
            ICollection<TeleportLocationDetailsModel> teleportLocationDetailsModels = TeleportLocationService.GetTeleportLocationDetailsModels(urbanAreaDetailsLink.UrbanAreaDetailsLink);
            if(teleportLocationDetailsModels == null || teleportLocationDetailsModels.Count == 0)
                return NotFound();
            return new JsonResult(teleportLocationDetailsModels);
        }


        [HttpPost("atributes/scores")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public ActionResult<TeleportLocationScoresModel> findLocationScores([FromBody] UrbanAreaScoresLinkDTO urbanAreaScoresLink)
        {
            //System.Diagnostics.Debug.WriteLine("urbanAreaScoresLink : "+urbanAreaScoresLink.UrbanAreaScoresLink);
            TeleportLocationScoresModel teleportSearchedCityDistrictScoresInfo = TeleportLocationService.GetTeleportLocationScoresModel(urbanAreaScoresLink.UrbanAreaScoresLink);
            if(teleportSearchedCityDistrictScoresInfo == null)
                return NotFound();
            return teleportSearchedCityDistrictScoresInfo;
        }

        [HttpPost("atributes/image")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Consumes("application/json")]
        [Produces("image/jpg")]
        public ActionResult findLocationImage([FromBody] UrbanAreaImagesLinkDTO urbanAreaImagesLinkDTO)
        {
            byte[] imageBytes = TeleportLocationService.GetSearchedCityImageBytes(urbanAreaImagesLinkDTO.UrbanAreaImagesLink);
            if (imageBytes != null)
                return File(imageBytes, "image/jpeg");
            else
                return NotFound(); 
        }
    }
}