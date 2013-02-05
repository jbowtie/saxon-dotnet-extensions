using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saxon.Api;
using log4net;

namespace SaxonExtended
{
    class SaxonMessageListener:IMessageListener
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SaxonMessageListener));

        public void Message(XdmNode content, bool terminate, IXmlLocation location)
        {
            if (terminate)
                log.ErrorFormat("{0} ({1}, line {2})", content.StringValue, location.BaseUri, location.LineNumber);
            else
                log.Warn(content.StringValue);
        }
    }
}
