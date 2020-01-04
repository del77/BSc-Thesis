using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Core.Model;
using Newtonsoft.Json;

namespace Core.OpenStreetMap
{
    public class OsmService
    {
        private const string ApiUrl = "http://lz4.overpass-api.de/api/interpreter";
        private readonly HttpClient _httpClient;
        private const double OffsetInKilometers = 0.01;
        private const int DefaultSurfacePavement = 50;
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
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiUrl)
            };

        }
        public async Task<int> ResolveRouteSurfaceTypeAsync(Route route)
        {
            var tags = (await GetSurfaceTypesAsync(route.Checkpoints)).ToList();

            int pavedCount = tags.Count(t => PavedSurfaces.Contains(t));
            int unpavedCount = tags.Count(t => UnpavedSurfaces.Contains(t));

            var pavedPercent = 1.0 * pavedCount / (pavedCount + unpavedCount) * 100;
            if (double.IsNaN(pavedPercent))
                return DefaultSurfacePavement;

            return (int)Math.Round(pavedPercent);
        }
        private async Task<IEnumerable<string>> GetSurfaceTypesAsync(IEnumerable<Point> points)
        {
            StringBuilder query = new StringBuilder($"?data=[out:json];");
            var responses = new List<OsmResponse>();

            foreach (var point in points)
            {
                var from = Point.GetPointWithGivenKilometersDistanceAndBearingFromStartingPoint(point, OffsetInKilometers, 225);
                var to = Point.GetPointWithGivenKilometersDistanceAndBearingFromStartingPoint(point, 1.0 * OffsetInKilometers / 1000, 45);

                query.Append($"way[surface]({from}, {to});convert e surface = t[\"surface\"]; out;");
            }

            var responseMessage = await _httpClient.GetAsync(query.ToString());
            var jsonResult = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            var res = JsonConvert.DeserializeObject<OsmResponse>(jsonResult);
            responses.Add(res);


            var tags = responses.SelectMany(r => r.Elements).Where(e => e.Tags?.Surface != null).Select(e => e.Tags.Surface);
            return tags;
        }
    }
}