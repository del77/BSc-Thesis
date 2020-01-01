using System;
using System.IO;
using SQLite;

namespace Core.Repositories.Local
{
    public abstract class LocalRepositoryBase
    {
        protected readonly SQLiteConnection Db;

        protected LocalRepositoryBase()
        {
            var databasePath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "database.db");
            Db = new SQLiteConnection(databasePath);
        }
    }
}