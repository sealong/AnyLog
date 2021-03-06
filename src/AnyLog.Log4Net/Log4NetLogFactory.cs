﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using log4net;
using log4net.Config;
using log4net.Repository;
using System.Collections.Concurrent;
using System.ComponentModel.Composition;

namespace AnyLog.Log4Net
{
    /// <summary>
    /// Log4NetLogFactory
    /// </summary>
    [Export(typeof(ILogFactory))]
    [LogFactoryMetadata("Log4Net", ConfigFileName = "log4net.config", Priority = 1)]
    public class Log4NetLogFactory : LogFactoryBase
    {
        /// <summary>
        /// Initializes the specified configuration files, we only use the first found one
        /// </summary>
        /// <param name="configFiles">All the potential configuration files, order by priority.</param>
        /// <returns></returns>
        public override bool Initialize(string[] configFiles)
        {
            if (!base.Initialize(configFiles))
                return false;

            log4net.Config.XmlConfigurator.Configure(new FileInfo(ConfigFile));
            return true;
        }

        /// <summary>
        /// Gets the log by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override ILog GetLog(string name)
        {
            return new Log4NetLog(LogManager.GetLogger(name));
        }

        protected override ILogInventory CreateLogInventory()
        {
            return new LogInventory<ILoggerRepository>(
                (name) => GetRepositoryConfigFile(name),
                (name, file) =>
                {
                    var repository = LogManager.CreateRepository(name);
                    log4net.Config.XmlConfigurator.ConfigureAndWatch(repository, new FileInfo(file));
                    return repository;
                },
                (resp, name) => new Log4NetLog(LogManager.GetLogger(resp.Name, name)));
        }
    }
}
