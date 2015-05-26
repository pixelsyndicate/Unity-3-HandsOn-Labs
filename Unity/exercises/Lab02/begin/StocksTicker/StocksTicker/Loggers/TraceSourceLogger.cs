//===============================================================================
// Microsoft patterns & practices
// Enterprise Library 6 and Unity 3 Hands-on Labs
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Diagnostics;
using Microsoft.Practices.Unity;

namespace StocksTicker.Loggers
{
    public class TraceSourceLogger : ILogger, IDisposable
    {
        private TraceSource traceSource;

        /// <summary>
        /// This is called in this example as the default constructor, because the signature is identified when I configured UnityContainer to use .RegisterType<ILogger, TraceSourceLogger>("UI", new InjectionConstructor("UI"))
        /// </summary>
        /// <param name="traceSourceName"></param>
        public TraceSourceLogger(string traceSourceName)
            : this(new TraceSource(traceSourceName, SourceLevels.All))
        {
        }

        /// <summary>
        /// Although this constructor was annotated with [InjectionConstructor]
        /// it will be ignored as I have configured UnityContainer to use .RegisterType <!-- ILogger, TraceSourceLogger --> ("UI", new InjectionConstructor("UI"))
        /// </summary>
        /// <param name="traceSource"></param>
        [InjectionConstructor] // this is ignored 
        public TraceSourceLogger(TraceSource traceSource)
        {
            this.traceSource = traceSource;
        }


        public void Log(string message, TraceEventType eventType)
        {
            this.traceSource.TraceEvent(eventType, 0, message);
            // this.traceSource.Flush(); // not needed, as we are using IDisposable, so buffering will now happen.
        }

        public void Dispose()
        {
            var thisTraceSource = this.traceSource;
            // alert us when the logger is being shut down
            if (thisTraceSource != null)
            {
                thisTraceSource.TraceInformation("Shutting down logger");
                thisTraceSource.Close();
                this.traceSource = null;
            }

            // this generates the following in the text.log file
            //--  UI Information: 0 : Shutting down logger
            //--       DateTime=2015-05-26T20:47:16.8384845Z
        }
    }
}
