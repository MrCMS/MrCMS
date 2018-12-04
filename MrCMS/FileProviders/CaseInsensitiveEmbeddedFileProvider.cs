using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Embedded;
using Microsoft.Extensions.Primitives;

namespace MrCMS.FileProviders
{
    /// <summary>
    ///     Looks up files using embedded resources in the specified assembly.
    ///     This file provider is case sensitive.
    /// </summary>
    public class CaseInsensitiveEmbeddedFileProvider : IFileProvider
    {
        private static readonly char[] _invalidFileNameChars = Path.GetInvalidFileNameChars().Where(c =>
        {
            if (c != '/')
                return c != '\\';
            return false;
        }).ToArray();

        private readonly Assembly _assembly;
        private readonly string _baseNamespace;
        private readonly DateTimeOffset _lastModified;
        private readonly HashSet<string> _resourceNames;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:MrCMS.FileProviders.CaseInsensitiveEmbeddedFileProvider" /> class
        ///     using the specified
        ///     assembly with the base namespace defaulting to the assembly name.
        /// </summary>
        /// <param name="assembly">The assembly that contains the embedded resources.</param>
        public CaseInsensitiveEmbeddedFileProvider(Assembly assembly)
            : this(assembly, (object)assembly != null ? assembly.GetName()?.Name : null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:MrCMS.FileProviders.CaseInsensitiveEmbeddedFileProvider" /> class
        ///     using the specified
        ///     assembly and base namespace.
        /// </summary>
        /// <param name="assembly">The assembly that contains the embedded resources.</param>
        /// <param name="baseNamespace">The base namespace that contains the embedded resources.</param>
        private CaseInsensitiveEmbeddedFileProvider(Assembly assembly, string baseNamespace)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            _baseNamespace = string.IsNullOrEmpty(baseNamespace) ? string.Empty : baseNamespace + ".";
            _assembly = assembly;
            _lastModified = DateTimeOffset.UtcNow;
            if (string.IsNullOrEmpty(_assembly.Location))
                return;
            try
            {
                _lastModified = File.GetLastWriteTimeUtc(_assembly.Location);
            }
            catch (PathTooLongException ex)
            {
            }
            catch (UnauthorizedAccessException ex)
            {
            }

            _resourceNames = _assembly.GetManifestResourceNames().ToHashSet(StringComparer.OrdinalIgnoreCase); // we're making this case insensitive to allow lookups
        }

        /// <summary>Locates a file at the given path.</summary>
        /// <param name="subpath">The path that identifies the file. </param>
        /// <returns>
        ///     The file information. Caller must check Exists property. A
        ///     <see cref="T:Microsoft.Extensions.FileProviders.NotFoundFileInfo" /> if the file could
        ///     not be found.
        /// </returns>
        public IFileInfo GetFileInfo(string subpath)
        {
            if (string.IsNullOrEmpty(subpath))
                return new NotFoundFileInfo(subpath);
            subpath = TidyPath(subpath, false);
            var stringBuilder = new StringBuilder(_baseNamespace.Length + subpath.Length);
            stringBuilder.Append(_baseNamespace);
            if (subpath.StartsWith("/", StringComparison.Ordinal))
                stringBuilder.Append(subpath, 1, subpath.Length - 1);
            else
                stringBuilder.Append(subpath);
            for (var length = _baseNamespace.Length; length < stringBuilder.Length; ++length)
                if (stringBuilder[length] == '/' || stringBuilder[length] == '\\')
                    stringBuilder[length] = '.';
            var str = stringBuilder.ToString();
            if (HasInvalidPathChars(str))
                return new NotFoundFileInfo(str);

            var fileName = Path.GetFileName(subpath);
            if (!_resourceNames.TryGetValue(str, out string resourceName))
            {
                return new NotFoundFileInfo(fileName);
            }

            return new EmbeddedResourceFileInfo(_assembly, resourceName, fileName, _lastModified);
        }

        /// <summary>
        ///     Enumerate a directory at the given path, if any.
        ///     This file provider uses a flat directory structure. Everything under the base namespace is considered to be one
        ///     directory.
        /// </summary>
        /// <param name="subpath">The path that identifies the directory</param>
        /// <returns>
        ///     Contents of the directory. Caller must check Exists property. A
        ///     <see cref="T:Microsoft.Extensions.FileProviders.NotFoundDirectoryContents" /> if no
        ///     resources were found that match <paramref name="subpath" />
        /// </returns>
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            if (subpath == null)
                return NotFoundDirectoryContents.Singleton;
            subpath = TidyPath(subpath, false);
            if (subpath.Length != 0 && !string.Equals(subpath, "/", StringComparison.Ordinal))
                return NotFoundDirectoryContents.Singleton;
            var fileInfoList = new List<IFileInfo>();
            foreach (var manifestResourceName in _resourceNames)
                if (manifestResourceName.StartsWith(_baseNamespace, StringComparison.OrdinalIgnoreCase))
                    fileInfoList.Add(new EmbeddedResourceFileInfo(_assembly, manifestResourceName,
                        manifestResourceName.Substring(_baseNamespace.Length), _lastModified));
            return new EnumerableDirectoryContents(fileInfoList);
        }

        /// <summary>Embedded files do not change.</summary>
        /// <param name="pattern">This parameter is ignored</param>
        /// <returns>A <see cref="T:Microsoft.Extensions.FileProviders.NullChangeToken" /></returns>
        public IChangeToken Watch(string pattern)
        {
            return NullChangeToken.Singleton;
        }

        private static bool HasInvalidPathChars(string path)
        {
            return path.IndexOfAny(_invalidFileNameChars) != -1;
        }


        protected string TidyPath(string path, bool isDirectory)
        {
            var segments = path.Split('/', '\\');

            // Loop through the segments of the provided Uri.
            for (var i = 0; i < segments.Length; i++)
            {
                // Find the first occurrence of the dot character.
                var dotPosition = segments[i].IndexOf('.');

                // Check if this segment is a folder segment.
                if (i < segments.Length - 1 || isDirectory) // all directory segments are folder segments
                {
                    // A dash in a folder segment will cause each following dot occurrence to be appended with an underscore.
                    int findPosition;
                    if ((findPosition = segments[i].IndexOf('-')) != -1 && dotPosition != -1)
                        segments[i] = segments[i].Substring(0, findPosition + 1) +
                                      segments[i].Substring(findPosition + 1).Replace(".", "._");

                    // A dash is replaced with an underscore when no underscores are in the name or a dot occurrence is before it.
                    segments[i] = segments[i].Replace('-', '_');
                }
            }

            return string.Join("/", segments);
        }
    }
}