using RestoreMonarchy.Database.FileDatabase;
using System.Collections.Generic;
using System.Data;

namespace RestoreMonarchy.Database
{
    public interface IDatabaseManager
    {
        IDbConnection Connection { get; }
        List<IFileDatabase> FileDatabases { get; }
        void CreateFileDatabase(string name, EFileDatabaseType fileDatabaseType);
        /// <summary>
        ///     Finds the first file database in FileDatabases with given name. Name case is ignored.
        /// </summary>
        IFileDatabase GetFileDatabase(string name);
    }
}
