using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Resources;
using System.Xml;
using Mvp.Xml.Common.Xsl;
using ResEx.Core;
using ResEx.StandardAdapters.Common;
using ResourceSet = ResEx.Core.ResourceSet;

namespace ResEx.StandardAdapters
{
    public class ResXResourceBundleAdapter : IResourceBundleAdapter
    {
        private readonly IFileSystem FileSystem;

        private const string xsltConvertToResx = "ResxConvertTo.xslt";

        private const string xsltConvertFromResx = "ResxConvertFrom.xslt";

        public ResXResourceBundleAdapter(IFileSystem fileSystem)
        {
            this.FileSystem = fileSystem;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This is the only what to get the job done here. Read the comments next to catch statement.")]
        public virtual ResourceBundle Load(string fileName)
        {
            var baseFileInfo = new FileInfo(fileName);
            var basePath = baseFileInfo.DirectoryName ?? string.Empty;

            // if extension of input file is not .resx and an xslt file to convert an xml into valid resx exists, then use it
            // else consider that the input file is .resx already
            string xsltFile = string.Empty;
            if (!string.Equals(baseFileInfo.Extension, ".resx", StringComparison.InvariantCultureIgnoreCase))
            {
                var xsltFullPath = Path.Combine(basePath, xsltConvertToResx);
                xsltFile = FileSystem.FileExists(xsltFullPath) ? xsltFullPath : null;
            }

            var extractCultureFromFile = new ResxExtractCultureFromFileStrategy();

            // define the search patterh (eg. MyFile.*.resx) that will be used to detect files of
            // other cultures, given the input file
            string searchPattern = fileName;
            var cultureOfInputFile = extractCultureFromFile.GetCulture(baseFileInfo.Name);
            if (cultureOfInputFile != ResourceSet.NeutralCulture)
            {
                // if input file contains culture information, then remove it
                searchPattern = extractCultureFromFile.ReplaceCulture(searchPattern, string.Empty);
                baseFileInfo = new FileInfo(searchPattern);
            }

            // put a wildcard before the extension
            searchPattern = Path.GetFileNameWithoutExtension(searchPattern) + "*" + baseFileInfo.Extension;

            var resourceBundle = new ResourceBundle();

            foreach (string file in this.FileSystem.GetFiles(basePath, searchPattern, SearchOption.TopDirectoryOnly))
            {
                try
                {
                    if (baseFileInfo.FullName == extractCultureFromFile.ReplaceCulture(file, string.Empty))
                    {
                        var culture = extractCultureFromFile.GetCulture(file);

                        // If neutral culture was detected for the second time, we are skipping the file
                        // There are two reasons that this can happen:
                        // a) The ResxExtractCultureFromFileStrategy failed to detect a know culture the file name and returned neutral
                        // b) File name is not part of the same resource bundle but happens to have similar name that pas the same prefix and .resx extension
                        // eg. our main file name could be MyFile.resx and this file name could be MyFile.Version2.resx
                        if (culture == ResourceSet.NeutralCulture && resourceBundle.ContainsCulture(culture)) continue;

                        var resourceSet = new ResourceSet(culture);
                        Resx2ResourceSet(resourceSet, file, xsltFile);
                        resourceBundle.Add(resourceSet);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format("Failed to open file '{0}'", file), ex);
                }
            }

            return resourceBundle;
        }

        public virtual void Save(string fileName, ResourceBundle resourceBundle)
        {
            var baseFileInfo = new FileInfo(fileName);
            string basePath = baseFileInfo.DirectoryName ?? string.Empty;

            // if extension of input file is not .resx and an xslt file to convert an xml into valid resx exists, then use it
            // else consider that the input file is .resx already
            string xsltFile = string.Empty;
            if (!string.Equals(baseFileInfo.Extension, ".resx", StringComparison.InvariantCultureIgnoreCase))
            {
                var xsltFullPath = Path.Combine(basePath, xsltConvertFromResx);
                xsltFile = FileSystem.FileExists(xsltFullPath) ? xsltFullPath : null;
            }

            var extractCultureFromFile = new ResxExtractCultureFromFileStrategy();

            // remove culture information from base file name
            // (eg. if MyFile.en.resx then remove en and keep MyFile..resx)
            string baseName = baseFileInfo.Name;
            var cultureOfInputFile = extractCultureFromFile.GetCulture(baseFileInfo.Name);
            if (cultureOfInputFile != ResourceSet.NeutralCulture)
            {
                // if input file contains culture information, then remove it
                baseName = extractCultureFromFile.ReplaceCulture(baseName, string.Empty);
            }

            // remove extension from the base file name
            baseName = Path.GetFileNameWithoutExtension(baseName);
            
            // remove any trailing dots from the base file name
            // (this can happen if base file name was containing culture name which
            // was removed a few lines above)
            baseName = baseName.TrimEnd('.');
            
            // create a array instead of enumerating original collection, 
            // since the code bellow may delete items from the that collection and would break the foreach process
            var resourceSets = resourceBundle.ToArray();

            foreach (var resourceSet in resourceSets)
            {
                string culture = resourceSet.Culture == ResourceSet.NeutralCulture ? string.Empty : "." + resourceSet.Culture;
                string resourceSetFullFileName = Path.Combine(basePath, baseName + culture + baseFileInfo.Extension);
                switch (resourceSet.Status)
                {
                    case ResourceSetStatus.Deleted:
                        // delete file from disk
                        File.Delete(resourceSetFullFileName);

                        // remove from collection
                        resourceBundle.Remove(resourceSet);
                        break;
                    case ResourceSetStatus.New:
                    case ResourceSetStatus.Updated:
                        // save to resource file
                        ResourceSet2Resx(resourceSet, resourceSetFullFileName, xsltFile);
                        break;
                }
            }
        }

        private static void ResourceSet2Resx(ResourceSet resourceSet, string file, string xsltFileName)
        {
            // copy target file (if already exists) to temporary location, just in case operation fails
            string temporaryFileName = null;
            if (File.Exists(file))
            {
                temporaryFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                File.Copy(file, temporaryFileName);
            }

            // delete target file
            File.Delete(file);

            try
            {
                using (var outputStream = File.Open(file, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    ResourceSet2Resx(resourceSet, outputStream, xsltFileName);
                }
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(temporaryFileName))
                {
                    throw new InvalidOperationException(string.Format("Failed to write to '{0}'.", file), ex);
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Failed to write to '{0}'. Original content of the file can be found here '{1}'.", file, temporaryFileName), ex);
                }
            }

            // delete temporary file (if one was created) now that everything went fine
            if (!string.IsNullOrEmpty(temporaryFileName))
            {
                File.Delete(temporaryFileName);
            }
        }

        private static void ResourceSet2Resx(ResourceSet resourceSet, Stream outputStream, string xsltFileName)
        {
            Stream temporaryStream;

            // If xslt was provided then use a temporary memory stream to save
            // content. Later the content will be saved to final output stream after
            // xsl transformation. If xslt was not provided then save to the given output stream.
            if (!string.IsNullOrEmpty(xsltFileName))
            {
                temporaryStream = new MemoryStream();
            }
            else
            {
                temporaryStream = outputStream;
            }

            ResourceSet2Resx(resourceSet, temporaryStream);

            // if xslt was provided then convert the output stream
            // using that xslt file
            if (!string.IsNullOrEmpty(xsltFileName))
            {
                // move current position of temporary stream to beginning because previous method 
                // left it at the end after completing writing to it
                temporaryStream.Position = 0;

                ApplyXslt(temporaryStream, xsltFileName, outputStream);
            }
        }

        internal static void ResourceSet2Resx(ResourceSet resourceSet, Stream stream)
        {
            var writer = new ResXResourceWriter(stream);
            foreach (ResourceItem resourceItem in resourceSet.Values)
            {
                var resourceNode = new ResXDataNode(resourceItem.Name, resourceItem.Value);
                
                // create comment from actual comment and other metadata (maxlenght, locked etc)
                var comment = resourceItem.Comment;

                var resourceStringItem = resourceItem as ResourceStringItem;
                if (resourceStringItem != null && resourceStringItem.MaxLength != ResourceStringItem.UnlimitedLength)
                {
                    comment = resourceStringItem.MaxLength + "#" + comment;
                }
                else if (resourceItem.LockedReason == LockedReason.DeveloperLock)
                {
                    comment = "#" + comment;
                }
                else if (resourceItem.ReviewPending)
                {
                    comment = "?" + comment;
                }

                if (!string.IsNullOrEmpty(comment))
                {
                    resourceNode.Comment = comment;
                }

                // check for empty value. empty values should not exist
                // to resource files so that the corresponding values from the neutral
                // resource file is used instead
                if (!resourceItem.ValueEmpty)
                {
                    writer.AddResource(resourceNode);
                }
            }

            writer.Generate();  // this command behaves like flush in a stream. it writes to the stream behind all pending changes.
        }

        /// <summary>
        /// Loads a .resx file (given it's full file path) into a <see cref="ResourceSet"/>
        /// </summary>
        private static void Resx2ResourceSet(ResourceSet resourceSet, string file, string xsltFile)
        {
            Stream inputStream = File.OpenRead(file);

            // if xslt was provided then do not use the file directly.
            // convert it using xslt and use output instead
            if (!string.IsNullOrEmpty(xsltFile))
            {
                var outputStream = new MemoryStream();
                ApplyXslt(inputStream, xsltFile, outputStream);
                inputStream = outputStream;
            }

            using (inputStream)
            {
                Resx2ResourceSet(resourceSet, inputStream);
            }
        }

        public static void ApplyXslt(Stream inputStream, string xsltFileName, Stream outputStream)
        {
            // open the xslt file
            using (var xslFileReader = XmlReader.Create(xsltFileName))
            {
                // do the transformation
                var xslt = new MvpXslTransform();
                xslt.Load(xslFileReader);
                xslt.Transform(new XmlInput(inputStream), null, new XmlOutput(outputStream));
                outputStream.Position = 0;    // move position of stream to beginning to be ready for reading later
            }
        }

        /// <summary>
        /// Loads a .resx file (given a stream with it's content) into the given <see cref="ResourceSet"/>
        /// </summary>
        internal static void Resx2ResourceSet(ResourceSet resourceSet, Stream stream)
        {
            ResourceItem resourceItem;
            ResourceStringItem resourceStringItem;
            string resourceComment;
            ResXDataNode res;
            object value;
            var reader = new ResXResourceReader(stream);
            reader.UseResXDataNodes = true;

            var assemblyNames = new System.Reflection.AssemblyName[] { };   // this is used as parameter of GetValue since there is no overload that accepts no parameter

            foreach (DictionaryEntry item in reader)
            {
                res = (ResXDataNode) item.Value;

                // resource item has file reference then special care must be taken
                if (res.FileRef == null)
                {
                    value = res.GetValue(assemblyNames);
                }
                else
                {
                    value = res.FileRef;
                }

                resourceComment = res.Comment;

                int sharpPosition = -1;
                if (!string.IsNullOrEmpty(resourceComment))
                {
                    sharpPosition = resourceComment.IndexOf("#", StringComparison.OrdinalIgnoreCase);
                }

                if (resourceComment == null)
                {
                    resourceComment = string.Empty;
                }

                if (value != null)
                {
                    switch (value.GetType().Name)
                    {
                        case "String":
                            resourceStringItem = new ResourceStringItem();
                            resourceItem = resourceStringItem;

                            if (sharpPosition > 0)
                            {
                                int tempValue;

                                if (int.TryParse(resourceComment.Substring(0, sharpPosition), out tempValue))
                                {
                                    resourceStringItem.MaxLength = tempValue;
                                    resourceComment = resourceComment.Substring(sharpPosition + 1);
                                }
                            }

                            break;

                        default:
                            resourceItem = new ResourceItem();
                            break;
                    }
                }
                else
                {
                    resourceItem = new ResourceItem();
                }

                resourceItem.Name = res.Name;
                resourceItem.Value = value;
                LockedReason resourceLockedReason = LockedReason.Unknown;

                // lock local value if base comment begins with #
                bool resourceLocked;
                if (sharpPosition == 0)
                {
                    resourceLocked = true;
                    resourceLockedReason = LockedReason.DeveloperLock;
                    resourceComment = resourceComment.Substring(1);
                }
                else
                {
                    resourceLocked = false;
                }

                if (resourceItem.Name.StartsWith(">>", StringComparison.OrdinalIgnoreCase))
                {
                    resourceLocked = true;
                    resourceLockedReason = LockedReason.FrameworkLock;
                }

                if (resourceComment.StartsWith("@", StringComparison.OrdinalIgnoreCase))
                {
                    resourceLocked = true;
                    resourceLockedReason = LockedReason.ResexMetadata;
                }

                if (resourceComment.StartsWith("?", StringComparison.OrdinalIgnoreCase))
                {
                    resourceComment = resourceComment.Substring(1);
                    resourceItem.ReviewPending = true;
                }

                resourceItem.Comment = resourceComment;
                resourceItem.Locked = resourceLocked;
                resourceItem.LockedReason = resourceLockedReason;

                // add item to collection
                resourceSet.Add(resourceItem.Name, resourceItem);
            }
        }
    }
}