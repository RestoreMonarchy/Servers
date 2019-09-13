using Rocket.Core.Plugins;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;

namespace Example
{
    public class ExamplePlugin : RocketPlugin
    {
        protected override void Load()
        {
            UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
        }

        private void OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            player.Player.GetComponent<PlayerComponent>().OnDeath(cause, limb, UnturnedPlayer.FromCSteamID(murderer));
        }
    }
}
