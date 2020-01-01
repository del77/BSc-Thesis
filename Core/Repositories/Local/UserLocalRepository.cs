using System.Collections.Generic;
using System.Linq;
using Core.Model;

namespace Core.Repositories.Local
{
    public class UserLocalRepository : LocalRepositoryBase
    {
        public UserLocalRepository()
        {
            Db.CreateTable<UserData>();
            Db.CreateTable<DataToSend>();
        }

        public void CreateUserData(UserData userData)
        {
            Db.Insert(userData);
        }

        public UserData GetUserData()
        {
            return Db.Table<UserData>().SingleOrDefault();
        }

        public void CreateDataToSend(DataToSend dataToSend)
        {
            Db.Insert(dataToSend);
        }

        public IEnumerable<DataToSend> GetDataToSend()
        {
            return Db.Table<DataToSend>().ToList();
        }

        public void DeleteDataToSend(int id)
        {
            Db.Table<DataToSend>().Delete(x => x.Id == id);
        }
    }
}