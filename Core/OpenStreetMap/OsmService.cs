using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Extensions;
using Core.Model;
using Newtonsoft.Json;

namespace Core.OpenStreetMap
{
    public class OsmService
    {
        private readonly HttpClient _httpClient;
        private const int Offset = 10;
        private static readonly string[] PavedSurfaces = 
        {
            "paved", "asphalt", "concrete", "concrete:lanes", "concrete:plates", "paving_stones", "sett", "unhewn_cobblestone",
            "cobblestone", "metal", "wood", "metal_grid"
        };

        public static readonly string[] UnpavedSurfaces = 
        {
            "unpaved", "compacted", "fine_gravel", "gravel", "pebblestone", "dirt", "earth", "grass", "grass_paver",
            "ground", "mud", "sand", "woodchips", "snow", "ice", "salt", "clay", "tartan", "artifical_turf", "decoturf", "carpet"
        };

        public OsmService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://overpass-api.de/api/interpreter");
        }
        public async Task<int> ResolveRouteSurfaceTypeAsync(Route route)
        {
            route = Route.GetNewRoute();
            var tags = (await GetSurfaceTypesAsync(route.Checkpoints)).ToList();

            
            int pavedCount = tags.Count(t=>PavedSurfaces.Contains(t));
            int unpavedCount = tags.Count(t => UnpavedSurfaces.Contains(t));

            var pavedPercent = 1.0 * pavedCount / (pavedCount + unpavedCount) * 100;

            return pavedPercent.RoundToClosest10();
        }

        private async Task<IEnumerable<string>> GetSurfaceTypesAsync(IEnumerable<Point> points)
        {
            var responses = new List<OsmResponse>();
            points = new List<Point>
            {
                (new Point(51.752623, 19.438215, null)),
                (new Point(51.753553, 19.444103, null))
            };

            foreach (var point in points)
            {
                var from = Point.ComputeOffset(point, Offset, 225);
                var to = Point.ComputeOffset(point, Offset, 45);
                var responseMessage = await _httpClient.GetAsync($"?data=[out:json];way({from}, {to}); out;");
                var jsonResult = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                var res = JsonConvert.DeserializeObject<OsmResponse>(jsonResult);
                responses.Add(res);
            }

            var tags = responses.SelectMany(r => r.Elements).Where(e => e.Tags?.Surface != null).Select(e => e.Tags.Surface);
            return tags;
        }
    }
}