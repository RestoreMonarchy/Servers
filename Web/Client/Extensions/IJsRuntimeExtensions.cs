using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Web.Client.Extensions
{
    public static class IJSRuntimeExtensions
    {
        public static ValueTask DisplayMessage(this IJSRuntime js, string message)
        {
            return js.InvokeVoidAsync("Swal.fire", message);
        }

        public static ValueTask DisplayMessage(this IJSRuntime js, string title, string content, string icon)
        {
            return js.InvokeVoidAsync("Swal.fire", title, content, icon);
        }

        public static ValueTask<bool> Confirm(this IJSRuntime js, string title, string content, string icon)
        {
            return js.InvokeAsync<bool>("ConfirmAlert", title, content, icon);
        }
        
        public static ValueTask Notify(this IJSRuntime js, string title, string icon, int duration)
        {
            return js.InvokeVoidAsync("NotifyAlert", title, icon, duration);
        }
    }
    
    public sealed class AlertIcon
    {
        public static readonly string SUCCESS = "success";
        public static readonly string ERROR = "error";
        public static readonly string WARNING = "warning";
        public static readonly string INFO = "info";
        public static readonly string QUESTION = "question";
    }

}
