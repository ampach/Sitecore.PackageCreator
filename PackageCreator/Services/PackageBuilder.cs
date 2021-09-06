namespace PackageCreator.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using PackageCreator.Models;

    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Install;
    using Sitecore.Install.Configuration;
    using Sitecore.Install.Files;
    using Sitecore.Install.Framework;
    using Sitecore.Install.Items;
    using Sitecore.Install.Utils;
    using Sitecore.Install.Zip;
    using Sitecore.SecurityModel;
    using Sitecore.Sites;

    public static class PackageBuilder
    {
        public static string GeneratePackage(PackageManifest manifest)
        {

            var packageProject = new PackageProject
            {
                Metadata =
                {
                    PackageName = manifest.PackageName,
                    Author = manifest.Author,
                    Version = manifest.Version,
                    Publisher = manifest.Publisher
                }
            };
            
            foreach (var fileSource in manifest.Files)
            {
                if(fileSource == null || fileSource.Entries == null || fileSource.Entries.Count == 0) continue;

                var packageFileSource = new ExplicitFileSource
                {
                    Name = "Files"
                };

                packageFileSource.Converter.Transforms.Add(
                    new InstallerConfigurationTransform(
                        new BehaviourOptions(fileSource.InstallMode, fileSource.MergeMode)));

                foreach (var item in fileSource.Entries)
                {
                    var pathMapped = MainUtil.MapPath(item.Path);

                    packageFileSource.Entries.Add(pathMapped);
                }

                if (packageFileSource.Entries.Count > 0)
                {
                    packageProject.Sources.Add(packageFileSource);
                }
            }

            
            foreach (var itemSource in manifest.Items)
            {
                if (itemSource == null || itemSource.Entries == null || itemSource.Entries.Count == 0) continue;

                List<Item> items = new List<Item>();
                var packageItemSource = new ExplicitItemSource
                {
                    Name = itemSource.Name
                };

                packageItemSource.Converter.Transforms.Add(
                    new InstallerConfigurationTransform(
                        new BehaviourOptions(itemSource.InstallMode, itemSource.MergeMode)));

                using (new SecurityDisabler())
                {
                    
                    foreach (var item in itemSource.Entries)
                    {
                        var db = ResolveDatabase(item.Database);

                        var itemUri = db.Items.GetItem(item.Path);
                        if (itemUri != null)
                        {
                            items.Add(itemUri);

                            if (item.IncludeChildren)
                            {
                                var paths = Sitecore.StringUtil.Split(itemUri.Paths.Path, '/', true).Where(p => p != null & p != string.Empty).Select(p => "#" + p + "#").ToList();
                                string allChildQuery = string.Format("/{0}//*", Sitecore.StringUtil.Join(paths, "/"));
                                var children = db.Items.Database.SelectItems(allChildQuery);

                                if (children != null && children.Length > 0)
                                    items.AddRange(children);
                            }
                        }
                    }

                    foreach (var item in items)
                    {
                        packageItemSource.Entries.Add(new ItemReference(item.Uri, false).ToString());
                    }
                    
                }

                if (packageItemSource.Entries.Count > 0)
                {
                    packageProject.Sources.Add(packageItemSource);
                }
                
            }

            packageProject.SaveProject = true;
            
            var location = MainUtil.MapPath($"{ Sitecore.Configuration.Settings.PackagePath}/{ manifest.PackageName}");
            bool exists = System.IO.Directory.Exists(location);

            if (!exists)
                System.IO.Directory.CreateDirectory(location);

            var packagePath = $"{location}/{manifest.PackageName}.zip"; 
            try
            {
                using (var writer = new PackageWriter(packagePath))
                {
                    using(new SecurityDisabler())
                    {
                        SiteContext targetSiteContext = SiteContext.GetSite("shell");
                        using (var context = new SiteContextSwitcher(targetSiteContext))
                        {
                            writer.Initialize(Installer.CreateInstallationContext());

                            PackageGenerator.GeneratePackage(packageProject, writer);
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Package was not created. Message: {ex.Message}", ex);
            }
            

            return packagePath;
        }

        private static Database ResolveDatabase(string databaseName)
        {
            if (!string.IsNullOrWhiteSpace(databaseName))
            {
                var db = Sitecore.Configuration.Factory.GetDatabase(databaseName);
                if (db != null)
                    return db;
            }
                
            return Sitecore.Configuration.Factory.GetDatabase(
                    Sitecore.Configuration.Settings.GetSetting("SourceDatabase"));

        }
    }
}