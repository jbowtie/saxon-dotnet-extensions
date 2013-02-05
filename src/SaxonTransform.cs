using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saxon.Api;
using System.Xml;
using System.Xml.Linq;
using log4net;
using net.sf.saxon.trace;

namespace SaxonExtended
{
    /// <summary>
    /// Wrap the Saxon API to make it a bit more convenient to work with
    /// </summary>
    class SaxonTransform
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SaxonTransform));
        XsltTransformer transform;
        Processor processor;
        List<StaticError> errorList = new List<StaticError>();

        /// <summary>
        /// Load the XSLT file
        /// </summary>
        public void Load(string filename, bool profilingEnabled = false)
        {
            //register our eval() function
            processor = new Processor();
            processor.RegisterExtensionFunction(new SaxonEvaluate(processor.NewXPathCompiler()));

            //tracing
            if (profilingEnabled)
            {
                var profile = new TimingTraceListener();
                processor.Implementation.setTraceListener(profile);
                processor.Implementation.setLineNumbering(true);
                processor.Implementation.setCompileWithTracing(true);
                processor.Implementation.getDefaultXsltCompilerInfo().setCodeInjector(new TimingCodeInjector());
                profile.setOutputDestination(new java.io.PrintStream("profile.html"));
            }

            //capture the error information
            var compiler = processor.NewXsltCompiler();
            compiler.ErrorList = errorList;

            //compile the stylesheet
            var relativeFilename = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
            try
            {
                var exec = compiler.Compile(XmlTextReader.Create(relativeFilename));

                //capture xsl:message output
                transform = exec.Load();
                transform.MessageListener = new SaxonMessageListener();
            }
            catch (Exception)
            {
                foreach (StaticError err in compiler.ErrorList)
                {
                    log.ErrorFormat("{0} ({1}, line {2})", err, err.ModuleUri, err.LineNumber);
                }
                throw;
            }
        }

        public string BaseOutputDirectory
        {
            get { return transform.BaseOutputUri.LocalPath; }
            set { transform.BaseOutputUri = new Uri(value); }
        }

        /// <summary>
        /// Load the input file and execute the transformation
        /// </summary>
        public string Run(string inputfile, IResultDocumentHandler handler = null)
        {
            DocumentBuilder docbuilder = processor.NewDocumentBuilder();
            docbuilder.WhitespacePolicy = WhitespacePolicy.StripIgnorable;
            XdmNode input = docbuilder.Build(new Uri(inputfile));

            return Execute(input, handler);
        }

        /// <summary>
        /// Execute the transformation
        /// </summary>
        public string Run(XDocument doc, IResultDocumentHandler handler = null)
        {
            DocumentBuilder docbuilder = processor.NewDocumentBuilder();
            docbuilder.WhitespacePolicy = WhitespacePolicy.StripIgnorable;
            XdmNode input = docbuilder.Build(doc.CreateReader());
            return Execute(input, handler);
        }

        /// <summary>
        /// Add a parameter to the current transformation
        /// </summary>
        public void AddParameter(string name, string value)
        {
            transform.SetParameter(new QName(name), new XdmAtomicValue(value));
        }

        /// <summary>
        /// Add a parameter to the current transformation
        /// </summary>
        public void AddParameter(string name, XDocument value)
        {
            DocumentBuilder docbuilder = processor.NewDocumentBuilder();
            docbuilder.WhitespacePolicy = WhitespacePolicy.StripIgnorable;
            XdmNode input = docbuilder.Build(value.CreateReader());
            transform.SetParameter(new QName(name), input);
        }

        /// <summary>
        /// Actually execute the transformation using the Saxon API
        /// </summary>
        private string Execute(XdmNode input, IResultDocumentHandler handler)
        {
            transform.InitialContextNode = input;
            if(handler != null)
                transform.ResultDocumentHandler = handler;

            var output = new StringBuilder();
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            transform.Run(new TextWriterDestination(XmlWriter.Create(output, settings)));
            return output.ToString();
        }

    }

    static class SaxonExtensions
    {
        static void EnableProfiling(this XsltCompiler compiler)
        {
        }
    }
}
