using IIS_WakeUp_Website.Data;
using log4net;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VS2013_Extensions;

namespace IIS_WakeUp_Website
{
    class Program
    {
        public static DTO_AppOptions CMDParameters = new DTO_AppOptions();
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program).ToString());

        static void Main(string[] args)
        {
            try
            {
                if (CommandLine.Parser.Default.ParseArguments(args, CMDParameters))
                {
                    if(CMDParameters.IsBatch)
                    {
                        // Enable file and console logging.
                        CLogger.ConfigureRollingFileAppender();
                        CLogger.AddAppender(typeof(Program).ToString(), CLogger.GetColoredConsoleAppender());
                    }
                    else
                    {
                        // Enable console logging.
                        CLogger.ConfigureColoredConsoleAppender();
                    }

                    Log.Info("===== " + Assembly.GetExecutingAssembly().GetName().Name + " " + Assembly.GetEntryAssembly().GetName().Version + " =====");

                    if (args.Count() == 0)
                    {
                        Console.WriteLine(CMDParameters.GetUsage());
                    }
                    else if (args.Count() == 1 && CMDParameters.IsBatch == true)
                    {
                        Log.Warn("Missing input parameters.");
                    }
                    else
                    {
                        doActionAccordingToParams(args); 
                    }
                }
                else
                {
                    throw new ArgumentNullException(NameOf.nameof(() => CMDParameters), "All CMDParameters should not be null.");
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                if (!CMDParameters.IsBatch)
                {
                    Console.WriteLine("Press the ENTER key to close the program.");
                    Console.ReadLine();                   
                }
                Log.Info("============= " + "END" + " =============");
            }
        }

        /// <summary>
        /// Does the action according to parameters.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void doActionAccordingToParams(string[] args)
        {
            if (CMDParameters.fileLst != null && CMDParameters.fileLst.Count() > 0)
            {
                // One/multiple files with command switch.
                Parallel.ForEach(CMDParameters.fileLst, (file) =>
                {
                    String extension = Path.GetExtension(file);
                    if (String.IsNullOrEmpty(extension) || extension != @".yaml")
                    {
                        throw new Exception("File: " + file + " has wrong extension, it should end with '.yaml'.");
                    }
                });
                CWebClient.WakeUpWebsiteWithYAML(CMDParameters.fileLst);
            }
            else if (CMDParameters.urlLst != null && CMDParameters.urlLst.Count() > 0)
            {
                // It's a manual parameter. (command switch with Urls)
                CWebClient.WakeUpWebsiteManual(CMDParameters.urlLst);
            }
            else if (CMDParameters.fileDragDropLst != null && CMDParameters.fileDragDropLst.Count() > 0)
            {
                // One/multiple files with drag'n'drop.
                Parallel.ForEach(CMDParameters.fileDragDropLst, (file) =>
                {
                    String extension = Path.GetExtension(file);
                    if (String.IsNullOrEmpty(extension) || extension != @".yaml")
                    {
                        throw new Exception("File: " + file + " has wrong extension, it should end with '.yaml'.");
                    }
                });
                CWebClient.WakeUpWebsiteWithYAML(CMDParameters.fileDragDropLst);
            }
            else
            {
                throw new ArgumentNullException(NameOf.nameof(() => CMDParameters), "All CMDParameters should not be null.");
            }
        }
    }
}
