using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace IIS_WakeUp_Website
{
    public class CLogger
    {
        /// <summary>
        /// Configures the file appender.
        /// </summary>
        public static void ConfigureRollingFileAppender()
        {
            var rollingFileAppender = getRollingFileAppender();
            BasicConfigurator.Configure(rollingFileAppender);
            ((Hierarchy)LogManager.GetRepository()).Root.Level = Level.Debug;
        }

        /// <summary>
        /// Configures the colored console appender.
        /// </summary>
        public static void ConfigureColoredConsoleAppender()
        {
            var ccAppender = _getColoredConsoleAppender();
            BasicConfigurator.Configure(ccAppender);
            ((Hierarchy)LogManager.GetRepository()).Root.Level = Level.Debug;
        }

        /// <summary>
        /// Adds the appender.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        /// <param name="appender">The appender.</param>
        public static void AddAppender(string loggerName, IAppender appender)
        {
            ILog log = LogManager.GetLogger(loggerName);
            Logger l = (Logger)log.Logger;

            l.AddAppender(appender);
        }

        /// <summary>
        /// Gets the colored console appender.
        /// </summary>
        /// <returns></returns>
        public static IAppender GetColoredConsoleAppender()
        {
            return _getColoredConsoleAppender();
        }

        /// <summary>
        /// Gets the file appender.
        /// </summary>
        /// <returns></returns>
        private static IAppender getRollingFileAppender()
        {
            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%date [%thread] - %message%newline";
            patternLayout.ActivateOptions();

            // Rolling File
            RollingFileAppender roller = new RollingFileAppender();
            roller.AppendToFile = true;
            roller.File = @"EventLog.log";
            roller.Layout = patternLayout;
            roller.MaxSizeRollBackups = 5;
            roller.MaximumFileSize = "50MB";
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.StaticLogFileName = true;            
            roller.ActivateOptions();

            return roller;
        }

        /// <summary>
        /// Gets the colored console appender.
        /// </summary>
        /// <returns></returns>
        private static IAppender _getColoredConsoleAppender()
        {
            // Colored Console
            ColoredConsoleAppender coloredConsoleAppender = new ColoredConsoleAppender
            {
                Threshold = Level.All,
                Layout = new PatternLayout("%d{HH:mm:ss} - %message%newline"),
            };
            coloredConsoleAppender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Debug,
                ForeColor = ColoredConsoleAppender.Colors.Purple
                    | ColoredConsoleAppender.Colors.HighIntensity
            });
            coloredConsoleAppender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Info,
                ForeColor = ColoredConsoleAppender.Colors.Green
                    | ColoredConsoleAppender.Colors.HighIntensity
            });
            coloredConsoleAppender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Warn,
                ForeColor = ColoredConsoleAppender.Colors.Yellow
                    | ColoredConsoleAppender.Colors.HighIntensity
            });
            coloredConsoleAppender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Error,
                ForeColor = ColoredConsoleAppender.Colors.Red
                    | ColoredConsoleAppender.Colors.HighIntensity
            });
            coloredConsoleAppender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Fatal,
                ForeColor = ColoredConsoleAppender.Colors.White
                    | ColoredConsoleAppender.Colors.HighIntensity,
                BackColor = ColoredConsoleAppender.Colors.Red
            });
            coloredConsoleAppender.ActivateOptions();

            return coloredConsoleAppender;
        }
    }
}
