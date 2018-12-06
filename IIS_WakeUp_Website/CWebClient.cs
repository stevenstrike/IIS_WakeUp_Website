using IIS_WakeUp_Website.Data;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IIS_WakeUp_Website
{
    public static class CWebClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program).ToString());

        /// <summary>
        /// Wakes up website manual asynchronous.
        /// </summary>
        /// <param name="sitesLst">The sites.</param>
        public static void WakeUpWebsiteManual(List<String> sitesLst)
        {
            // Setup parallel breaker variables.
            int currentThreadId = 0;
            object lockCurrentThread = new object();

            Parallel.ForEach(sitesLst, (site) =>
            {
                try
                {
                    // Lock the current thread to wait for the previous threads to finish, thus bringing order.
                    int thisCurrentThread = 0;
                    lock (lockCurrentThread)
                    {
                        thisCurrentThread = currentThreadId;
                        currentThreadId++;
                    }

                    Log.Info(site + " waking up...");

                    // Stopwatch
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    // wakeup url by fetching webpage using a webclient request.
                    wakeUpWebsite(site);

                    sw.Stop();

                    Log.Info(site + " now awake... (took " + sw.Elapsed.Seconds + " seconds)");
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            });
        }

        /// <summary>
        /// Wakes up website with yaml.
        /// </summary>
        /// <param name="filesLst">The files.</param>
        public static void WakeUpWebsiteWithYAML(List<String> filesLst)
        {
            // Setup parallel breaker variables.
            int currentThreadId = 0;
            object lockCurrentThread = new object();

            // Setup parallel breaker variables.
            int currentSubThreadId = 0;
            object lockCurrentSubThread = new object();

            // Parse YAML to Object.
            Parallel.ForEach(filesLst, (file) =>
            {
                // Lock the current thread to wait for the previous threads to finish, thus bringing order.
                int thisCurrentThread = 0;
                lock (lockCurrentThread)
                {
                    thisCurrentThread = currentThreadId;
                    currentThreadId++;
                }
                // Parse YAML File and call method to wake up the website.
                try
                {
                    using (StreamReader reader = File.OpenText(file))
                    {
                        Log.Info("Validating file : " + Path.GetFileName(file));

                        // Deserialize YAML to object, validation is also applied.
                        DTO_Header header = CYamlParser.Deserializer.DeserializeYAML(reader);                       

                        Parallel.ForEach(header.WebSiteLst, (website) =>
                        {
                            // Lock the current thread to wait for the previous threads to finish, thus bringing order.
                            int thisCurrentSubThread = 0;
                            lock (lockCurrentSubThread)
                            {
                                thisCurrentSubThread = currentSubThreadId;
                                currentSubThreadId++;
                            }

                            Log.Info(website.Name + " waking up...");

                            // Stopwatch
                            Stopwatch sw = new Stopwatch();
                            sw.Start();

                            wakeUpWebsite(website.Name, website.Url, website.PortLst);

                            sw.Stop();

                            Log.Info(website.Name + " now awake... (took " + sw.Elapsed.Seconds + " seconds)");
                        });
                    }
                }
                catch (Exception ex)
                {
                    if (!String.IsNullOrEmpty(ex.InnerException.Message))
                    {
                        Log.Error("Error on File: " + Path.GetFileName(file) + " : " + ex.InnerException.Message);
                    }
                    else
                    {
                        Log.Error("Error on File: " + Path.GetFileName(file) + " : " + ex.ToString());
                    }                   
                }
            });
        }

        /// <summary>
        /// Wakes up website.
        /// </summary>
        /// <param name="site">The site.</param>
        private static void wakeUpWebsite(String site)
        {
            // Send HTTP request to revive Website (IP or FQDN):Port).
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.UseDefaultCredentials = true;

                    string _url = treatUrl(site);

                    using (Stream stream = webClient.OpenRead(new Uri(_url, UriKind.Absolute)))
                    {
                        // Do Nothing.
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error waking : " + site);
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// Wakes up website.
        /// </summary>
        /// <param name="AppName">Name of the application.</param>
        /// <param name="Url">The URL.</param>
        /// <param name="PortsLst">The ports.</param>
        private static void wakeUpWebsite(String AppName, String Url, List<String> PortsLst)
        {
            // Setup parallel breaker variables.
            int currentThreadId = 0;
            object lockCurrentThread = new object();

            Parallel.ForEach(PortsLst, (port) =>
            {
                // Lock the current thread to wait for the previous threads to finish, thus bringing order.
                int thisCurrentThread = 0;
                lock (lockCurrentThread)
                {
                    thisCurrentThread = currentThreadId;
                    currentThreadId++;
                }

                // Send HTTP request to revive Website (IP or FQDN):Port).
                try
                {
                    Log.Info(AppName + " on Port : " + port + " waking up...");

                    using (WebClient webClient = new WebClient())
                    {
                        webClient.UseDefaultCredentials = true;

                        string _url = treatUrl(Url);

                        using (Stream stream = webClient.OpenRead(new Uri(_url + ":" + port, UriKind.Absolute)))
                        {
                            // Do nothing.
                        };
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Error waking : " + Url + "Port : " + port);
                    Log.Error(ex.ToString());
                }
            });
        }

        /// <summary>
        /// Treats the URL.
        /// If last char is '/', remove it.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Url : " + url + " is not a valid url.</exception>
        private static string treatUrl(String url)
        {
            Uri uriResult;
            bool isUrl = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (isUrl)
            {
                if (url.Last() == '/')
                {
                    url.Remove(url.Length - 1);
                }
            }
            else
            {
                throw new Exception("Url : " + url + " is not a valid url.");
            }
            return url;
        }
    }
}
