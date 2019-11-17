using System;
using System.Collections.Generic;
using System.Text;

namespace RestoreMonarchy.Database.FileDatabase
{
    public interface IFileDatabase
    {
        string NameIdentifier { get; }
        void SaveObject(object obj);
        ///<summary>
        ///   We want to pass exception to the caller, therefore T is out parameter and return is boolean.
        ///</summary>
        List<T> ReadObject<T>();
    }
}
