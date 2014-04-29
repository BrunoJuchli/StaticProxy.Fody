namespace StaticProxy.Fody.Tests
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;

    public class VersionReader
    {
        public decimal FrameworkVersionAsNumber;
        public string FrameworkVersionAsString;
        public string TargetFrameworkProfile;
        public bool IsSilverlight;

        private const string Dot = ".";

        public VersionReader(string projectPath)
        {
            var xDocument = XDocument.Load(projectPath);
            xDocument.StripNamespace();
            this.GetTargetFrameworkIdentifier(xDocument);
            this.GetFrameworkVersion(xDocument);
            this.GetTargetFrameworkProfile(xDocument);
        }

        void GetFrameworkVersion(XDocument xDocument)
        {
            // todo improve version reading and merge it to Fody Test utilities!
            this.FrameworkVersionAsString = xDocument.Descendants("TargetFrameworkVersion")
                .Select(c => c.Value)
                .First();

            string majorMinor = this.FrameworkVersionAsString.TrimStart('v');

            int firstDotIndex = majorMinor.IndexOf(Dot, StringComparison.InvariantCulture);
            if (firstDotIndex != majorMinor.LastIndexOf(Dot, StringComparison.InvariantCulture))
            {
                int length = majorMinor.IndexOf(Dot, firstDotIndex + 1, StringComparison.InvariantCulture);
                majorMinor = majorMinor.Substring(0, length);
            }

            this.FrameworkVersionAsNumber = decimal.Parse(majorMinor, CultureInfo.InvariantCulture);
        }

        void GetTargetFrameworkProfile(XDocument xDocument)
        {
            this.TargetFrameworkProfile = xDocument.Descendants("TargetFrameworkProfile")
                .Select(c => c.Value)
                .FirstOrDefault();
        }

        void GetTargetFrameworkIdentifier(XDocument xDocument)
        {
            var targetFrameworkIdentifier = xDocument.Descendants("TargetFrameworkIdentifier")
                .Select(c => c.Value)
                .FirstOrDefault();
            if (string.Equals(targetFrameworkIdentifier, "Silverlight", StringComparison.OrdinalIgnoreCase))
            {
                this.IsSilverlight = true;
            }
        }
    }
}