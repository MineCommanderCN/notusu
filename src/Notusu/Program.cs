#pragma warning disable 8618 // Unullable properties must contain non-null value after return
using CommandLine;
using CommandLine.Text;
using Notusu.Converter;
using Notusu.NBS;
using Newtonsoft.Json;

namespace Notusu;
class Program
{
    public const string VERSION = "0.0.1-alpha";

    class Options
    {
        readonly static string[] VALID_TARGETS = { "nbs", "osu", "auto" };

        private string _target = "";

        [Option('i', "input", HelpText = "The input .osu or .nbs file.", Required = true)]
        public string InputFile { get; set; }
        [Option('t', "target", Required = false, HelpText = "The target file format. 'osu' converts .nbs to .osu, and 'nbs' converts .osu to .nbs. 'auto' detects input file extension.", Default = "auto")]
        public string Target
        {
            get => _target;
            set
            {
                if (!VALID_TARGETS.Contains(value))
                {
                    throw new ArgumentException("Not a valid target", "target");
                }
                _target = value;
            }
        }
        [Option('o', "offset", Required = false, HelpText = "Override the offset value in song properties.", Default = null)]
        public double? Offset { get; set; }

        [Value(0, MetaName = "destination", HelpText = "The destination directory.", Required = false, Default = ".")]
        public string Destination { get; set; }
    }

    public static int Main(string[] args)
    {
        Options options = new();
        Parser parser = new CommandLine.Parser(with => with.HelpWriter = null);
        var parserResult = parser.ParseArguments<Options>(args);
        bool parseSuccess = false;
        parserResult
            .WithParsed<Options>(o => { options = o; parseSuccess = true; })
            .WithNotParsed(errs => DisplayHelp(parserResult, errs));
        if (parseSuccess)
        {
            //do stuff here
            if (!File.Exists(options.InputFile))
            {
                Console.Error.WriteLine("'{0}': No such file or invalid path", options.InputFile);
                return 1;
            }
            if (options.Target == "auto")
            {
                switch (Path.GetExtension("options.InputFile"))
                {
                    case ".osu": options.Target = "nbs"; break;
                    case ".nbs": options.Target = "osu"; break;
                    default: Console.Error.WriteLine("Unknown file extension! Please specific the target using --target option."); return 1;
                }
            }
            if (options.Target == "osu")
            {
                FileStream fin = new(options.InputFile, FileMode.Open, FileAccess.Read);
                NBSFile nbsFile = NBSFile.FromStream(fin);
                //Console.WriteLine(JsonConvert.SerializeObject(nbsFile));
                BeatmapProperties bp = BeatmapProperties.FromString(nbsFile.SongDescription);
                if (options.Offset is not null)
                {
                    bp.Offset = (double)options.Offset;
                }
                NBSConverter converter = new(bp);
                var osuFile = converter.Convert(nbsFile, bp);
                //Console.WriteLine(JsonConvert.SerializeObject(osuFile));
                string osuFileName = $"{nbsFile.SongOriginalAuthor} - {nbsFile.SongName} ({nbsFile.SongAuthor}) [{bp.Difficulty}].osu";
                File.WriteAllText($"{options.Destination}{Path.DirectorySeparatorChar}{osuFileName}", osuFile.ToString());
            }
            if (options.Target == "nbs")
            {
                Console.Error.WriteLine("Sorry, this function is not implemented yet :(");
                return 1;
            }
            return 0;
        }
        else
        {
            return 1;
        }
    }

    public static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errors)
    {

        string helpText = "";
        helpText = errors.IsVersion() ?
            VERSION
            : HelpText.AutoBuild(result, h =>
                {
                    h.AdditionalNewLineAfterOption = false;
                    h.Heading = $"Notusu {VERSION}";
                    return HelpText.DefaultParsingErrorsHandler(result, h);
                }, e => e);
        Console.WriteLine(helpText);
    }
}