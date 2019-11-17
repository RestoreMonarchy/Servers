using RestoreMonarchy.Database.FileDatabase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace RestoreMonarchy.Database
{
    public class DatabaseManager : IDatabaseManager
    {   
        private readonly string _connectionString;
        private readonly string _fileDatabaseDir;

        public DatabaseManager(string connectionString, string fileDatabaseDir)
        {
            _connectionString = connectionString;
            _fileDatabaseDir = fileDatabaseDir;
            FileDatabases = new List<IFileDatabase>();
        }

        public IDbConnection Connection => new SqlConnection(_connectionString);
        public List<IFileDatabase> FileDatabases { get; private set; }

        public void CreateFileDatabase(string name, EFileDatabaseType fileDatabaseType)
        {
            switch (fileDatabaseType)
            {
                case EFileDatabaseType.JsonFileDatabase:
                    FileDatabases.Add(new JsonFileDatabase(_fileDatabaseDir, name));
                    break;
            }
        }

        public IFileDatabase GetFileDatabase(string name)
        {
            return FileDatabases.FirstOrDefault(x => x.NameIdentifier.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
