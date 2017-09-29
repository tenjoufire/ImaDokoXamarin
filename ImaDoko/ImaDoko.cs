using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;

namespace ImaDoko
{
    public class ImaDoko
    {
        string id;
        string name;
        string place;
        bool hasGone;

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [JsonProperty(PropertyName = "hasGone")]
        public bool HasGone
        {
            get { return hasGone; }
            set { hasGone = value; }
        }

        [JsonProperty(PropertyName = "place")]
        public string Place
        {
            get { return place; }
            set { place = value; }
        }

        [Version]
        public string Version { get; set; }
    }
}
