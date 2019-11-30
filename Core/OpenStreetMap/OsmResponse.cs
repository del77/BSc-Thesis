using System.Collections;
using System.Collections.Generic;

namespace Core.OpenStreetMap
{
    public class OsmResponse
    {
        public IEnumerable<Element> Elements { get; set; }
    }

    public class Element
    {
        public Tags Tags { get; set; }
    }

    public class Tags
    {
        public string Surface { get; set; }
    }
}