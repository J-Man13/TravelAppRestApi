using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelAppRestApi.Models.TeleportApiModels
{
    public class TeleportLocationDetailsModel
    {
        public string DetailType { get; set; }
        public ICollection<TeleportLocationDetailModel> TeleportLocationDetailModels { get; set; }
    }
}
