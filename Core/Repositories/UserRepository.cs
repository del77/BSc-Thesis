using System;
using System.IO;
using System.Linq;
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

            _db.DropTable<UserData>();
            _db.CreateTable<UserData>();
        }

        public void CreateUserData(UserData userData)
        {
            _db.Insert(userData);
        }

        public UserData GetUserData()
        {
            return _db.Table<UserData>().SingleOrDefault();
        }
    }
}