namespace Core.Model
{
    public class UserData
    {
        public string Username { get; set; }
        public string Token { get; set; }

        public UserData(string username, string token)
        {
            Username = username;
            Token = token;
        }

        public UserData()
        {
            
        }
    }
}