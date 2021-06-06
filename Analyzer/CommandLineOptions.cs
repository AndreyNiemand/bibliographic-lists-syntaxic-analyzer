using CommandLine;

namespace bibliographic_lists_syntaxic_analyzer
{
    class CommandLineOptions
    {
        [Value(index: 0, Required = false, HelpText = "Name of used standard.", Default = "ГОСТ-Р-7.0.5-2008")]
        public string StandardName { get; set; }

        [Option(shortName: 'f', longName: "file", Required = false, HelpText = "File path to bibliographic references.")]
        public string File { get; set; }

        [Option(shortName: 'r', longName: "reference", Required = false, HelpText = "Single reference.")]
        public string SingleReference { get; set; }

        [Option(shortName: 'o', longName: "output", Required = false, HelpText = "Define the output to a file or to a console.", Default = "console")]
        public string Output { get; set; }
    }
}
