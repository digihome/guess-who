namespace GuessWho.Library
{
    /// <summary>
    /// Detection result class.
    /// </summary>
    public class DetectionResult
    {
        public List<string> Technologies { get; set; } = new();
        public string Display => Technologies.Count > 0 ? string.Join(", ", Technologies.OrderBy(t => t)) : "Unknown";
    }
}