using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class UserInfo
    {
        public bool IsAuthenticated { get; set; }
        public Player Player { get; set; }
    }
}
