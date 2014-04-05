using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace StaticProxy.Fody.Tests
{
    public class VersionReader
    {
        public decimal FrameworkVersionAsNumber;
        public string FrameworkVersionAsString;
        public string TargetFrameworkProfile;
        public bool IsSilverlight;

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
            this.FrameworkVersionAsString = xDocument.Descendants("TargetFrameworkVersion")
                .Select(c => c.Value)
                .First();
            this.FrameworkVersionAsNumber = decimal.Parse(this.FrameworkVersionAsString.Remove(0, 1), CultureInfo.InvariantCulture);
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