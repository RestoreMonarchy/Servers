using Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Web.Client.Pages
{
    public partial class AccountPage : ComponentBase
    {
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject]
        public HttpClient HttpClient { get; set; }

        private Player player;
        private List<Ticket> tickets;
        private List<PlayerPunishment> punishments;

        private bool isLoaded => player != null && tickets != null && punishments != null;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            player = await HttpClient.GetJsonAsync<Player>("api/players");
            tickets = await HttpClient.GetJsonAsync<List<Ticket>>("api/tickets");
            punishments = await HttpClient.GetJsonAsync<List<PlayerPunishment>>("api/punishments");
        }

        public async Task OnSubmittedAsync(Ticket ticket)
        {
            await PostTicketAsync(ticket);
        }

        public async Task PostTicketAsync(Ticket ticket)
        {
            ticket = await HttpClient.PostJsonAsync<Ticket>("api/tickets", ticket);
            tickets.Add(ticket);
        }

        string GetTicketClass(Ticket ticket)
        {
            return ticket.Status ? "table-active" : string.Empty;
        }

        string GetPunishmentClass(PlayerPunishment punishment)
        {
            if (punishment.Category == "ban" && punishment.ExpiryDate == null || punishment.ExpiryDate > DateTime.Now)
                return "table-danger";
            else
                return string.Empty;
        }
    }
}
