namespace ScibuAPIConnector.Models
{
    public class Client
    {
        public string ClientId { get; set; }

        public string Secret { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public string DatabaseName { get; set; }
    }
}
