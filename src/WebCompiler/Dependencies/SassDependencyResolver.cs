using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace WebCompiler
{
    class SassDependencyResolver : DependencyResolverBase
    {
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
                var includedFiles = new List<FileInfo>();

                //collect files from @import rules
                //note: the regex doesn't cover all possible valid @import rules, but since @import is now deprecated in sass,
                //there's probably no need to perfect it.

                //match both <@<type> "myFile.scss";> and <@<type> url("myFile.scss");> syntax (where supported)
                var matches = Regex.Matches(content, @"(?<=@import(?:[\s]+))(?:(?:\(\w+\)))?\s*(?:url)?(?<url>[^;]+)", RegexOptions.Multiline);
                foreach (Match match in matches)
                {
                    string url = match.Groups["url"].Value.Replace("'", "\"").Replace("(", "").Replace(")", "").Replace(";", "").Trim();

                    foreach (string name in url.Split(new[] { "\"," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string value = name.Replace("\"", "").Replace("/", "\\").Trim();
                        var fileInfo = GetFileInfo(info.DirectoryName, value);
                        if (fileInfo != null)
                            includedFiles.Add(fileInfo);
                    }
                }

                //collect files from @use and @forward rules
                matches = Regex.Matches(content, @"(?<=^@use|^@forward)\s+([""'])(?<url>.+?[^\\])\1", RegexOptions.Multiline);
                foreach (Match match in matches)
                {
                    var fileInfo = GetFileInfo(info.DirectoryName, match.Groups["url"].Value);
                    if (fileInfo != null)
                        includedFiles.Add(fileInfo);
                }

                foreach (FileInfo includedfile in includedFiles)
                {
                    var theFile = includedfile;

                    //if the file doesn't end with the correct extension, an import statement without extension is probably used, to re-add the extension (#175)
                    if (string.Compare(includedfile.Extension, FileExtension, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        theFile = new FileInfo(includedfile.FullName + this.FileExtension);
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

        private static FileInfo GetFileInfo(string directoryName, string name)
        {
            FileInfo fileInfo = null;
            try
            {
                fileInfo = new FileInfo(Path.Combine(directoryName, name));
            }
            catch (Exception ex)
            {
                // Not a valid file name
                System.Diagnostics.Debug.Write(ex);
            }
            return fileInfo;
        }
    }
}
