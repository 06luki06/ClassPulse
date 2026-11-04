namespace At.luki0606.ClassPulse.Data
{
    /// <summary>
    /// User preferences stored in <c>settings.json</c>.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets the theme: <c>System</c>, <c>Light</c> or <c>Dark</c>.
        /// </summary>
        public string Theme { get; set; } = "System";
        /// <summary>
        /// Gets or sets the two-letter UI language code: <c>de</c> or <c>en</c>.
        /// </summary>
        public string Language { get; set; } = "de";
    }
}
