using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelAppRestApi.Models.TeleportApiModels
{
    public class TeleportLocationScoresModel
    {
        public string Summary { get; set; }
        public int CityScore { get; set; }
        public ICollection<TeleportLocationScoreModel> TeleportSearchedCityDistrictScores { get; set; }
    }
}
