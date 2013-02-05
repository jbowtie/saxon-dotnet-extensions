Saxon.NET Extensions
====================

This is a simple collection of classes I frequently reuse when working with the Saxon.NET XSLT compiler.

FileResultDocumentHandler
-------------------------

Writes <xsl:result-document> output to the file system.

TestResultDocumentHandler
-------------------------

Captures <xsl:result-document> output during unit tests.

SaxonMessageListener
--------------------

Captures <xsl:message> output and routes it through log4net

SaxonEvaluate
-------------

A quick-and-dirty re-implementation of the saxon:evaluate extension function for times when Saxon-HE must be used.

SaxonTransform
--------------

Wraps the saxon API in a simple-to-use class that supports logging and optional profiling.
