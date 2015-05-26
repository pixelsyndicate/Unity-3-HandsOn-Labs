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
using Microsoft.Practices.Unity;
using StocksTicker.Loggers;
using StocksTicker.StockQuoteServices;
using StocksTicker.UI;

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

            using (IUnityContainer container = new UnityContainer())
            {

                // test - create and examine the ResolvedParameter<ILogger>("UI") to be used in creating a InjectionProperty for StocksTickerPresenter
                var resovedParm = new ResolvedParameter<ILogger>("UI"); 
                // resovles to :  (((Microsoft.Practices.Unity.TypedInjectionValue)(resovedParm)).ParameterType).FullName = StocksTicker.Logger.ILogger

                // test - create and examine new InjectionProperty(stringName, resovedparm)
                var injecProp = new InjectionProperty("Logger", resovedParm);

                container
                    .RegisterType<IStocksTickerView, StocksTickerForm>()
                    //.RegisterType<IStockQuoteService, RandomStockQuoteService>()
                    .RegisterType<IStockQuoteService, RandomStockQuoteService>(new InjectionProperty("Logger"))
                    // indicates property with name "Logger" should be injected. replaces [Dependency]
                    .RegisterType<ILogger, ConsoleLogger>()
                    // .RegisterType<ILogger, TraceSourceLogger>("UI") // this uses the default constructor

                    // .RegisterType<ILogger, TraceSourceLogger>("UI", new InjectionConstructor("UI")) // this points to a specific constructor based on signature
                    // this overides the default constructor AND [InjectionConstructor], finding the one using the parameter 'signagure'
                    // .RegisterInstance(new TraceSource("UI", SourceLevels.All)) -- this isn't needed anymore as we can instantiate TraceSoruce in the above constructor
                    // the StocksTickerPresenter needs a Logger too, so ...force injection, and identify instance needed ()

                    .RegisterType<ILogger, TraceSourceLogger>("UI", new ContainerControlledLifetimeManager(), new InjectionConstructor("UI")) // this keeps the lifetype to the container

                    // comment out to try using the resovedParm in place of new ResolvedParameter<ILogger>("UI");
                    // .RegisterType<StocksTickerPresenter>(new InjectionProperty("Logger", new ResolvedParameter<ILogger>("UI")));
                    .RegisterType<StocksTickerPresenter>(injecProp);

                StocksTickerPresenter presenter
                    = container.Resolve<StocksTickerPresenter>();

                Application.Run((Form)presenter.View);
            }
        }
    }
}