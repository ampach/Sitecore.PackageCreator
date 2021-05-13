namespace PackageCreator.Models
{
    using System.Collections.Generic;

    using Sitecore.Install.Utils;

    public class AttachmentSourceModule
    {
        public string Name { get; set; }
        public MergeMode MergeMode { get; set; }
        public InstallMode InstallMode { get; set; }
        public List<AttachmentModel> Entries { get; set; }
    }
}