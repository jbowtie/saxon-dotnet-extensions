using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saxon.Api;

namespace SaxonExtended
{
    /// <summary>
    /// Re-implement saxon:evaluate since it is not suppported in Saxon-HE.
    /// Put in its own namespace per advice from Saxon documentation.
    /// </summary>
    class SaxonEvaluate : ExtensionFunctionDefinition
    {
        private XPathCompiler compiler;
        public SaxonEvaluate(XPathCompiler compiler)
        {
            this.compiler = compiler;
            // look into making namespace declarations as needed
            //compiler.DeclareNamespace("fo", "http://www.w3.org/1999/XSL/Format");
        }

        public override XdmSequenceType[] ArgumentTypes
        {
            get { return new XdmSequenceType[] {new XdmSequenceType(XdmAtomicType.BuiltInAtomicType(QName.XS_STRING), '?')};  }
        }

        public override QName FunctionName
        {
            get { return new QName("http://github.com/jbowtie/saxon-dotnet-extensions", "evaluate"); }
        }

        public override ExtensionFunctionCall MakeFunctionCall()
        {
            return new SaxonEvaluateCall(compiler);
        }

        public override int MaximumNumberOfArguments
        {
            get { return 1; }
        }

        public override int MinimumNumberOfArguments
        {
            get { return 1; }
        }

        public override XdmSequenceType ResultType(XdmSequenceType[] ArgumentTypes)
        {
            return new XdmSequenceType(XdmAnyNodeType.Instance, '*');
        }

        public class SaxonEvaluateCall : ExtensionFunctionCall
        {
            private XPathCompiler compiler;
            public SaxonEvaluateCall(XPathCompiler compiler)
            {
                this.compiler = compiler;
            }

            public override IXdmEnumerator Call(IXdmEnumerator[] arguments, DynamicContext context)
            {
                arguments[0].MoveNext();
                var arg = (XdmAtomicValue)arguments[0].Current;
                string path = arg.Value.ToString();
                if(string.IsNullOrWhiteSpace(path))
                    return EmptyEnumerator.INSTANCE;
                var nodeset = compiler.Evaluate(path, context.ContextItem);
                return (IXdmEnumerator)nodeset.GetEnumerator();
            }
        }
    }
}
