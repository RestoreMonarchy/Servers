using System.Collections.Generic;

namespace RestoreMonarchy.SQLPermissions.Models
{
    public class PermissionGroup
    {
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public string GroupColor { get; set; }
        public short Priority { get; set; }

        public virtual List<Permission> Permissions { get; set; }
        public virtual List<PermissionMemeber> Members { get; set; }

        public class Permission
        {
            public string GroupID { get; set; }
            public string PermissionID { get; set; }
        }

        public class PermissionMemeber
        {
            public string GroupID { get; set; }
            public ulong SteamID { get; set; }
        }
    }
}
