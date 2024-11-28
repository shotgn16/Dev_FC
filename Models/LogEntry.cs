namespace ForestChurches.Models
{
    public class LogEntry
    {
        public string Timestamp { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string Url { get; set; }
        public string Action { get; set; }
    }
}
