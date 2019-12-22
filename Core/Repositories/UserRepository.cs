using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Model;
using SQLite;

namespace Core.Repositories
{
    public class UserRepository
    {
        private readonly SQLite.SQLiteConnection _db;
        public UserRepository()
        {
            var databasePath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "database.db");
            _db = new SQLiteConnection(databasePath);
            _db.CreateTable<UserData>();
            _db.CreateTable<DataToSend>();
        }

        public void CreateUserData(UserData userData)
        {
            _db.Insert(userData);
        }

        public UserData GetUserData()
        {
            return _db.Table<UserData>().SingleOrDefault();
        }

        public void CreateDataToSend(DataToSend dataToSend)
        {
            _db.Insert(dataToSend);
        }

        public IEnumerable<DataToSend> GetDataToSend()
        {
            return _db.Table<DataToSend>().ToList();
        }

        public void DeleteDataToSend(int id)
        {
            _db.Table<DataToSend>().Delete(x => x.Id == id);
        }
    }
}