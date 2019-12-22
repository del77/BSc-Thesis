using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Core.Extensions;
using Core.Model;
using Newtonsoft.Json;

namespace Core.OpenStreetMap
{
    public class OsmService
    {
        private const string ApiUrl = "http://lz4.overpass-api.de/api/interpreter";
        private readonly HttpClient _httpClient;
        private const double OffsetInKilometers = 0.01;
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

            return (int)Math.Round(pavedPercent);
        }
        HttpResponseMessage responseMessage;
        string jsonResult;
        OsmResponse res;
        private async Task<IEnumerable<string>> GetSurfaceTypesAsync(IEnumerable<Point> points)
        {
            StringBuilder query = new StringBuilder($"?data=[out:json];");
            var responses = new List<OsmResponse>();
            points = new List<Point>
            {
                new Point(51.752812, 19.443531, 0),
                new Point(51.753191, 19.443938, 0),
                new Point(51.753815, 19.444035, 0),
                new Point(51.754535, 19.443889, 0),
                new Point(51.754960, 19.443792, 0),
                new Point(51.755188, 19.443438, 0),
                new Point(51.755355, 19.442881, 0),
                new Point(51.755434, 19.442151, 0),
                new Point(51.755447, 19.441508, 0),
                new Point(51.755248, 19.439899, 0),
                new Point(51.755076, 19.439011, 0),
                new Point(51.754810, 19.438247, 0),
                new Point(51.754531, 19.437764, 0),
                new Point(51.753840, 19.437378, 0),
                new Point(51.753362, 19.437410, 0),
                new Point(51.752904, 19.437775, 0),
                new Point(51.752612, 19.438257, 0),
                new Point(51.752406, 19.438868, 0),
                new Point(51.752230, 19.439626, 0),
                new Point(51.752190, 19.440430, 0),
                new Point(51.752217, 19.441374, 0),

                new Point(51.948656, 19.206382, 0)
            };


            foreach (var point in points)
            {
                var from = Point.ComputeOffset(point, OffsetInKilometers, 225);
                var from2 = Point.GetPointWithGivenKilometersDistanceAndBearingFromStartingPoint(point, OffsetInKilometers, 225);

                var to = Point.ComputeOffset(point, OffsetInKilometers, 45);
                var to2 = Point.GetPointWithGivenKilometersDistanceAndBearingFromStartingPoint(point, 1.0 * OffsetInKilometers / 1000, 45);

                query.Append($"way[surface]({from}, {to});convert e surface = t[\"surface\"]; out;");
            }

            responseMessage = await _httpClient.GetAsync(query.ToString());
            jsonResult = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            res = JsonConvert.DeserializeObject<OsmResponse>(jsonResult);
            responses.Add(res);


            var tags = responses.SelectMany(r => r.Elements).Where(e => e.Tags?.Surface != null).Select(e => e.Tags.Surface);
            return tags;
        }
    }
}