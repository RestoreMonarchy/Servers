using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Client.Shared
{
    public partial class NavMenu : ComponentBase
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public NavigationManager Navigation { get; set; }

        public async Task GoAsync(string uri, bool forceLoad = false)
        {
            Navigation.NavigateTo(uri, forceLoad);
            await JSRuntime.InvokeVoidAsync("HideNavdrawer", "navdrawerDefault");
        }
    }
}
