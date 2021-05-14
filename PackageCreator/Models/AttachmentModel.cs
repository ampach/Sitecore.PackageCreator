namespace PackageCreator.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class AttachmentModel
    {
        public string Path { get;set; }
        public string Database { get;set; }
        public bool IncludeChildren { get;set; }

    }
}