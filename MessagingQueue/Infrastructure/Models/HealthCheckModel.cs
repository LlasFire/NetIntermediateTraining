namespace Infrastructure.Models
{
    public class HealthCheckModel : SettingsModel
    {
        public StatusEnum Status { get; set; }

        public string ClientName { get; set; }
    }
}
