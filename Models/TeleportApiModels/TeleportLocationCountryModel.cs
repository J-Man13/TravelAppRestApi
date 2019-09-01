using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelAppRestApi.Models.TeleportApiModels
{
    public class TeleportLocationCountryModel
    {
        public String Name { get; set; }
        public String CurrencyCode { get; set; }
        public String Iso_alpha2 { get; set; }
        public String Iso_alpha3 { get; set; }
        public int Population { get; set; }

        public ICollection<TeleportLocationCountrySalaryModel> TeleportCountrySalaries { get; set; }
    }
}
