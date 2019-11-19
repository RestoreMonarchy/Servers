using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Server.Services
{
    public class EventsHandler
    {
        public delegate void PlayerCreated(Player player);
        public static event PlayerCreated OnPlayerCreated = delegate { };
        
        public static void RaisePlayerCreated(Player player)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("I was called hello?");
            OnPlayerCreated(player);
        }
    }
}
