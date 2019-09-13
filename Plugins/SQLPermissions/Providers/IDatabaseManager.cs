using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.SQLPermissions.Providers
{
    public interface IDatabaseManager
    {
        DbConnection Connection { get; }
    }
}
