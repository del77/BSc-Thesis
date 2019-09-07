using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Core.Model
{
    public class Route
    {
        public string Name { get; set; }
        public string Distance { get; set; } = "123";
        public List<string> Checkpoints { get; set; }


        public static Route GetNewRoute()
        {
            return new Route
            {
                Name = "Nowa trasa",
                Checkpoints = new List<string>()
            };
        }
    }
}