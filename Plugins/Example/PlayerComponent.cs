using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace Example
{
    public class PlayerComponent : MonoBehaviour
    {
        public Player Player { get; set; }

        void Awake()
        {
            Player = GetComponent<Player>();
        }

        public void OnDeath(EDeathCause cause, ELimb limb, UnturnedPlayer killer)
        {
            // Do smthing
        }
    }
}