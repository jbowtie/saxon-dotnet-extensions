using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Saxon.Api;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Xml.Linq;

namespace SaxonExtended.Tests
{
    /// <summary>
    /// Captures <xsl:result-document> output during unit tests
    /// </summary>
    class TestResultDocumentHandler : IResultDocumentHandler
    {
        Dictionary<string, StringBuilder> output = new Dictionary<string, StringBuilder>();

        public XmlDestination HandleResultDocument(string href, Uri baseUri)
        {
            var outputBuffer = new StringBuilder();
            var writer = XmlWriter.Create(outputBuffer);
            var dest = new TextWriterDestination(writer);
            output[href] = outputBuffer;
            return dest;
        }

        /// <summary>
        /// Assert that a particular document was created.
        /// </summary>
        public void AssertContains(string href)
        {
            Assert.Contains(href, output.Keys, string.Format("Result document {0} was not created.", href));
        }

        /// <summary>
        /// Assert that a particular document was NOT created.
        /// </summary>
        public void AssertExcludes(string href)
        {
            Assert.That(output.Keys, Is.Not.Contains(href), string.Format("Result document {0} should not have been created", href));
        }

        /// <summary>
        /// Verify that expected number of documents were created.
        /// </summary>
        public void AssertCount(int expected)
        {
            Assert.AreEqual(expected, output.Count());
        }

        internal string GetOutput(string key)
        {
            return output[key].ToString();
        }

        internal XDocument Document(string key)
        {
            return XDocument.Parse(output[key].ToString());
        }
    }
}
