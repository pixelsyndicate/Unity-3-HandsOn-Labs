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
using System.Windows.Forms;

using StocksTicker.Loggers;
using StocksTicker.StockQuoteServices;
using StocksTicker.UI;
using Microsoft.Practices.Unity;

namespace StocksTicker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // TODO use a container to create the objects here
            // Unity containers implement the IDisposable interface. 
            // This enables the new code to take advantage of the using statement to dispose the new container.
            using (IUnityContainer container = new UnityContainer())
            {
                // -- replace the 'presenter'
                // -- because the autowiring can't determine the constroctor for StocksTickerPresenter with signature IStocksTickerView, IStockQuoteService
                // -- map the StocksTickerForm to the IStocksTickerView
                // -- the expected constructor: 
                // StocksTickerPresenter(IStocksTickerView view,IStockQuoteService stockQuoteService)

                // -- configure mapping to resolve needed constructor objects that aren't named the same as the interface
                // -- do this for properties that are injected.
                container
                    .RegisterType<IStocksTickerView, StocksTickerForm>()
                    .RegisterType<IStockQuoteService, RandomStockQuoteService>()
                    //ILogger must be registered because it's marked as [Dependency]
                    .RegisterType<ILogger, ConsoleLogger>()
                    // ILogger for the StocksTickerPresenter has been decorated with [Dependency("UI")]
                    .RegisterType<ILogger, TraceSourceLogger>("UI")
                    // a .NET framework object is needed for a constructor, so we can pre-build and register the instance for use
                    .RegisterInstance(new TraceSource("UI", SourceLevels.All)); 

                // -- These are not needed, because its resolved in the StocksTickerPresenter() constructor
                // -- -------------------------------------------------------------------
                // StocksTickerForm view = new StocksTickerForm();
                // RandomStockQuoteService service = new RandomStockQuoteService();



                // -- replaced by Unity // 
                // -- StocksTickerPresenter presenter = new StocksTickerPresenter(view, service);
                StocksTickerPresenter presenter = container.Resolve<StocksTickerPresenter>();

                // the default for the present.logger is nulllogger

                // service.Logger = new ConsoleLogger();

                Application.Run((Form)presenter.View);
            }



            // replaced by Unity //  presenter.Logger = new TraceSourceLogger("UI");

            // replaced by Unity //  Application.Run((Form)presenter.View);
        }
    }
}