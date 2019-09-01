using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelAppRestApi.Models.TeleportApiModels
{
    public class TeleportLocationModel
    {
        public string InfoGeonameIdLink { get; set; }
        public string FullName { get; set; }
        public double Lattitude { get; set; }
        public double Longtitude { get; set; }

        public string CountryLink { get; set; }

        public string CountrySalariesLink { get; set; }

        public string UrbanAreaLink { get; set; }

        public string UrbanAreaSalariesLink { get; set; }
        public string UrbanAreaDetailsLink { get; set; }
        public string UrbanAreaScoresLink { get; set; }
        public string UrbanAreaImagesLink { get; set; }

    }
}
