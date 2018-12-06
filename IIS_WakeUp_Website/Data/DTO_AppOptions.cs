using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;

namespace IIS_WakeUp_Website.Data
{
    class DTO_AppOptions
    {
        [ParserState]
        public IParserState LastParserState { get; set; }

        [OptionList('u', "url", Required = false, Separator = ' ', 
            MetaValue = "URL_1 URL_N", HelpText = "Wake up websites using urls as input.")]
        public List<String> urlLst { get; set; }

        [OptionList('f', "file", Required = false, Separator = ' ', 
            MetaValue = "File_1 File_N", HelpText = "Wake up websites using YAML files as input.")]
        public List<String> fileLst { get; set; }

        [Option('b', "batch", Required = false, 
            HelpText = "Setup the application in batch mode (No user inputs required and logging is enabled).")]
        public Boolean IsBatch { get; set; }

        [ValueList(typeof(List<String>))]
        public List<String> fileDragDropLst { get; set; }

        [HelpOption(HelpText = "Display this help screen.")]
        public String GetUsage()
        {
            return HelpText.AutoBuild(this,
              current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}