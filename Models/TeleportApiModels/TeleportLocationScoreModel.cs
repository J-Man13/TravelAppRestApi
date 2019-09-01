using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelAppRestApi.Models.TeleportApiModels
{
    public class TeleportLocationScoreModel
    {
        public string Color { get; set; }
        public string ScoreName { get; set; }
        public int ScoreOutOfTen { get; set; }
    }
}
