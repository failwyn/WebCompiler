﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebCompiler
{
    class SassDependencyResolver : DependencyResolverBase
    {
        private Regex importsReg = new Regex(@"(?<=@import|@use|@forward(?:[\s]+))(?:(?:\(\w+\)))?\s*(?:url)?(?<url>[^;]+)", RegexOptions.Compiled|RegexOptions.Multiline);
        private static Regex filesReg = new Regex(@"(?:(?!\bas\b|\bwith\b)[\s\S])*", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override string[] SearchPatterns
        {
            get { return new[] { "*.scss", "*.sass" }; }
        }

        public override string FileExtension
        {
            get
            {
                return ".scss";
            }
        }


        /// <summary>
        /// Updates the dependencies of a single file
        /// </summary>
        /// <param name="path"></param>
        public override void UpdateFileDependencies(string path)
        {
            if (this.Dependencies != null)
            {
                FileInfo info = new FileInfo(path);
                path = info.FullName.ToLowerInvariant();

                if (!Dependencies.ContainsKey(path))
                    Dependencies[path] = new Dependencies();

                //remove the dependencies registration of this file
                this.Dependencies[path].DependentOn = new HashSet<string>();
                //remove the dependentfile registration of this file for all other files
                foreach (var dependenciesPath in Dependencies.Keys)
                {
                    var lowerDependenciesPath = dependenciesPath.ToLowerInvariant();
                    if (Dependencies[lowerDependenciesPath].DependentFiles.Contains(path))
                    {
                        Dependencies[lowerDependenciesPath].DependentFiles.Remove(path);
                    }
                }

                string content = File.ReadAllText(info.FullName);

                //match both <@<type> "myFile.scss";> and <@<type> url("myFile.scss");> syntax (where supported)
                foreach (Match match in importsReg.Matches(content))
                {
                    var importedfiles = GetFileInfos(info, match);

                    foreach (FileInfo importedfile in importedfiles)
                    {
                        if (importedfile == null)
                            continue;

                        var theFile = importedfile;

                        //if the file doesn't end with the correct extension, an import statement without extension is probably used, to re-add the extension (#175)
                        if (string.Compare(importedfile.Extension, FileExtension, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            theFile = new FileInfo(importedfile.FullName + this.FileExtension);
                        }

                        var dependencyFilePath = theFile.FullName.ToLowerInvariant();

                        if (!File.Exists(dependencyFilePath))
                        {
                            // Trim leading underscore to support Sass partials
                            var dir = Path.GetDirectoryName(dependencyFilePath);
                            var fileName = Path.GetFileName(dependencyFilePath);
                            var cleanPath = Path.Combine(dir, "_" + fileName);

                            if (!File.Exists(cleanPath))
                                continue;

                            dependencyFilePath = cleanPath.ToLowerInvariant();
                        }

                        if (!Dependencies[path].DependentOn.Contains(dependencyFilePath))
                            Dependencies[path].DependentOn.Add(dependencyFilePath);

                        if (!Dependencies.ContainsKey(dependencyFilePath))
                            Dependencies[dependencyFilePath] = new Dependencies();

                        if (!Dependencies[dependencyFilePath].DependentFiles.Contains(path))
                            Dependencies[dependencyFilePath].DependentFiles.Add(path);
                    }
                }
            }
        }

        private static IEnumerable<FileInfo> GetFileInfos(FileInfo info, System.Text.RegularExpressions.Match match)
        {
            var url = filesReg.Matches(match.Groups["url"].Value)
                .OfType<Match>()
                .FirstOrDefault()?
                .Value
                .Replace("'", "\"").Replace("(", "").Replace(")", "").Replace(";", "") ?? string.Empty;

            var list = new List<FileInfo>();

            foreach (string name in url.Split(new[] { "\"," }, StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    string value = name.Replace("\"", "").Replace("/", "\\").Trim();
                    list.Add(new FileInfo(Path.Combine(info.DirectoryName, value)));
                }
                catch (Exception ex)
                {
                    // Not a valid file name
                    System.Diagnostics.Debug.Write(ex);
                }
            }

            return list;
        }
    }
}
