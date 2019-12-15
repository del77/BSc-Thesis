using SQLite;

namespace Core.Model
{
    public class DataToSend
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Json { get; set; }
        public string Uri { get; set; }

        public DataToSend() { }

        public DataToSend(string json, string uri)
        {
            Json = json;
            Uri = uri;
        }
    }
}