using RestoreMonarchy.Moderation.Database;
using RestoreMonarchy.Moderation.Extensions;
using Rocket.API;
using Rocket.Core.Utils;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Framework.Utilities;
using SDG.Unturned;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace RestoreMonarchy.Moderation.Commands
{
    public class CheckownerCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "checkowner";

        public string Help => "Checks building or vehicle owner";

        public string Syntax => string.Empty;

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (PhysicsUtility.raycast(new Ray(player.Player.look.aim.position, player.Player.look.aim.forward), out RaycastHit hit, 10f, RayMasks.BARRICADE_INTERACT))
            {
                byte x;
                byte y;

                ushort plant;
                ushort index;

                BarricadeRegion bRegion;
                StructureRegion sRegion;

                InteractableVehicle vehicle;

                ulong ownerId;

                var instance = ModerationPlugin.Instance;

                if (BarricadeManager.tryGetInfo(hit.transform, out x, out y, out plant, out index, out bRegion))
                {
                    ownerId = bRegion.barricades[index].owner;
                } else if (StructureManager.tryGetInfo(hit.transform, out x, out y, out index, out sRegion))
                {
                    ownerId = sRegion.structures[index].owner;
                } else if ((vehicle = hit.transform.GetComponent<InteractableVehicle>()) != null && vehicle.isLocked) 
                {
                    ownerId = vehicle.lockedOwner.m_SteamID;
                } else
                {
                    UnturnedChat.Say(caller, instance.Translate("CheckownerFail"));
                    return;
                }

                ThreadPool.QueueUserWorkItem((i) =>
                {
                    var owner = instance.DatabaseManager.GetPlayer(ownerId);

                    if (owner == null)
                    {
                        owner = instance.DatabaseManager.CreatePlayer(new Models.Player(ownerId, ownerId.GetIP()));
                    }

                    TaskDispatcher.QueueOnMainThread(() =>
                    {
                        UnturnedChat.Say(caller, instance.Translate("CheckownerSuccess", owner.PlayerName, owner.PlayerId, owner.IsBanned(out var ban)));
                    });
                });
            }
        }
    }
}
