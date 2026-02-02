namespace SistemaPedidos.API.HttpModels
{
    public class FormatDetails
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int? Status { get; set; }
        public string Detail { get; set; }
        public string Instance { get; set; }
        public Dictionary<string, object> Extensions { get; set; } = new Dictionary<string, object>();

        public FormatDetails() { }
        public FormatDetails(string title, int? status, string detail, string? type = null, string? instance = null)
        {
            Type = type ?? "about:blank";
            Title = title;
            Status = status;
            Detail = detail;
            Instance = instance ?? $"urn:problem:{Guid.NewGuid()}";
        }

        public void AddExtension(string key, object value)
        {
            if (!Extensions.ContainsKey(key))
            {
                Extensions.Add(key, value);
            }
        }
    }

}
