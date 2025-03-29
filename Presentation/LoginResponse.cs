using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BattleShip.Presentation
{
    class LoginResponse
    {
        [JsonProperty("odata.metadata")]
        public String ODataMetadata { get; set; }

        [JsonProperty("value")]
        public List<InitResponseObject> Value { get; set; }
    }
}
