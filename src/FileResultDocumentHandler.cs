using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saxon.Api;
using System.IO;

namespace SaxonExtended
{
    class FileResultDocumentHandler : IResultDocumentHandler
    {
        public XmlDestination HandleResultDocument(string href, Uri baseUri)
        {
            var dest = new Serializer();
            dest.SetOutputFile(Path.Combine(baseUri.LocalPath, href));
            return dest;
        }
    }
}
