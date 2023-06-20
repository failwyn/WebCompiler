using System;
using System.IO;
using System.Linq;

namespace WebCompiler
{
    /// <summary>
    /// Helper class for file interactions
    /// </summary>
    public static class FileHelpers
    {
        /// <summary>
        /// Finds the relative path between two files.
        /// </summary>
        public static string MakeRelative(string baseFile, string file)
        {
            //Uri baseUri = new Uri(baseFile, UriKind.RelativeOrAbsolute);
            //Uri fileUri = new Uri(file, UriKind.RelativeOrAbsolute);

            //return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fileUri).ToString());

            var basePath = Path.GetFullPath( baseFile );
            var filePath = Path.GetFullPath( file );

            // remove the common ancestory of the two paths
            var basePathParts = basePath.Split(Path.DirectorySeparatorChar).ToList();
            var filePathParts = filePath.Split(Path.DirectorySeparatorChar).ToList();
            while( basePathParts.Any() && filePathParts.Any() && basePathParts.FirstOrDefault() == filePathParts.FirstOrDefault() )
            {
                basePathParts.RemoveAt(0);
                filePathParts.RemoveAt(0);
            }

            if ( basePathParts.Count == 0 && filePathParts.Count == 0)
            {
                return String.Join( Path.DirectorySeparatorChar, ".", Path.GetFileName(file) );
            }

            if ( basePathParts.Count == 1 && filePathParts.Count == 1)
            {
                return filePathParts.Single();
            }

            if ( basePathParts.Count == 0)
            {
                filePathParts.Insert(0, ".");
                return String.Join( Path.DirectorySeparatorChar, filePathParts );
            }

            if ( basePathParts.Count > filePathParts.Count )
            { 
                return String.Join( Path.DirectorySeparatorChar, Enumerable.Repeat("..",  basePathParts.Count - filePathParts.Count).Concat(filePathParts));
            }

            throw new NotSupportedException();
         }

        /// <summary>
        /// If a file has the read-only attribute, this method will remove it.
        /// </summary>
        public static void RemoveReadonlyFlagFromFile(string fileName)
        {
            FileInfo file = new FileInfo(fileName);

            if (file.Exists && file.IsReadOnly)
                file.IsReadOnly = false;
        }

        /// <summary>
        /// If a file has the read-only attribute, this method will remove it.
        /// </summary>
        public static void RemoveReadonlyFlagFromFile(FileInfo file)
        {
            RemoveReadonlyFlagFromFile(file.FullName);
        }

        /// <summary>
        /// Checks if the content of a file on disk matches the newContent
        /// </summary>
        public static bool HasFileContentChanged(string fileName, string newContent)
        {
            if (!File.Exists(fileName))
                return true;

            string oldContent = File.ReadAllText(fileName);

            return oldContent != newContent;
        }
    }
}
