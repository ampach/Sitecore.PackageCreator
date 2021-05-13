using System.Collections.Generic;

namespace PackageCreator.Models
{
    

    public class PackageManifest
    {
        public string PackageName { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Publisher { get; set; }
        public string Comments { get; set; }

        public List<AttachmentSourceModule> Items { get; set; }
        public List<AttachmentSourceModule> Files { get; set; }
    }
}