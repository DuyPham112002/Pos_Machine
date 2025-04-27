namespace Client_ViewModel.Fingerprint
{
    public class GetBrowserInfoViewModel
    {
        // Thông tin từ Front-End (ClientJS)
        public string? OS { get; set; } = string.Empty;
        public string? OSVersion { get; set; } = string.Empty;
        public string? TimeZone { get; set; } = string.Empty;
        public string? AvailableResolution { get; set; } = string.Empty;
        public string? Language { get; set; } = string.Empty;
        public string? Browser { get; set; } = string.Empty;
        public string? BrowserVersion { get; set; } = string.Empty;
        public string? Engine { get; set; } = string.Empty;
        public string? Plugins { get; set; } = "";

        // Thông tin từ Middleware
        public string? UserAgent { get; set; } = string.Empty;  // User Agent từ request header
        public string? AcceptLanguage { get; set; } = string.Empty; // Ngôn ngữ ưu tiên từ header
        public string? Scheme { get; set; } = string.Empty;    // Scheme của request (http/https)
    }
}
