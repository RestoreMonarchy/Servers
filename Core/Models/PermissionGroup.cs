using System.Collections.Generic;

namespace Core
{
    public class PermissionGroup
    {
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public string GroupColor { get; set; }
        public short GroupPriority { get; set; }

        public virtual List<Permission> Permissions { get; set; }
        public virtual List<PermissionMember> Members { get; set; }

        public class Permission
        {
            public string GroupID { get; set; }
            public string PermissionID { get; set; }

            public virtual PermissionGroup Group { get; set; }
        }

        public class PermissionMember
        {
            public PermissionMember() { }

            public PermissionMember(string groupId, string steamId)
            {
                GroupID = groupId;
                SteamID = steamId;
            }

            public string GroupID { get; set; }
            public string SteamID { get; set; }

            public virtual PermissionGroup Group { get; set; }
        }
    }
}
