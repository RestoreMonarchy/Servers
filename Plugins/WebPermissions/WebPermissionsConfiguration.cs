using Rocket.API;

namespace WebPermissions
{
    public class WebPermissionsConfiguration : IRocketPluginConfiguration
    {
        public string WebApiKey { get; set; }
        public string ApiUrl { get; set; }
        
        public void LoadDefaults()
        {
            WebApiKey = "123";
            ApiUrl = "http://localhost:5000/api/permissions";
        }
    }
}
