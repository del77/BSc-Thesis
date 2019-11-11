namespace Core.Model
{
    public class UserData
    {
        public string PhoneNumber { get; set; }
        public string Nickname { get; set; }

        public UserData(string phoneNumber, string nickname)
        {
            PhoneNumber = phoneNumber;
            Nickname = nickname;
        }

        public UserData()
        {
            
        }
    }
}