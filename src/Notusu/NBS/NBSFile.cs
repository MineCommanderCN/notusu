namespace Notusu.NBS;

/// <summary>
/// Represents a NBS file.
/// </summary>
public class NBSFile
{
    private byte _timeSignature;



    /// <summary>
    /// The factor used in tempo converting between ticks/s and BPM.
    /// </summary>
    public const double TEMPO_FACTOR = 0.15;

    /// <summary>
    /// The byte length of NBS editor data (auto-saving etc.) in the file.
    /// These data are ignored.
    /// </summary>
    public const int EDITOR_DATA_BYTES = 2;

    /// <summary>
    /// The byte length of NBS statistic data (minutes spent in editor, mouse click count, etc.) in the file.
    /// These data are ignored.
    /// </summary>
    public const int STAT_BYTES = 20;

    /// <summary>
    /// The byte length of data about song looping (loop on/off, loop count, etc.) in the file.
    /// These data are ignored.
    /// </summary>
    public const int LOOP_DATA_BYTES = 4;

    /// <summary>
    /// The NBS format version supported by Notusu currently.
    /// </summary>
    public const byte SUPPORTED_VERSION = 5;



    public NBSFile()
    {
        SongName = "";
        SongAuthor = "";
        SourceFileName = "";
        SongOriginalAuthor = "";
        SongDescription = "";
        Layers = Array.Empty<NBSLayer>();
        CustomInstruments = Array.Empty<NBSInstrument>();
    }



    /// <summary>
    /// The version of the new NBS format.
    /// </summary>
    public byte FormatVersion { get; set; }

    /// <summary>
    /// Amount of default instruments when the song was saved.
    /// This is needed to determine at what index custom instruments start.
    /// Usually this is a const value of 16.
    /// </summary>
    public byte VanillaInstrumentCount { get; set; }

    /// <summary>
    /// The length of the song, measured in ticks.
    /// Divide this by the tempo to get the length of the song in seconds.
    /// This value is actually ignored while converting.
    /// </summary>
    public ushort SongLength { get; set; }

    /// <summary>
    /// The last layer with at least one note block in it, or the last layer that has had its name, volume or stereo changed.
    /// </summary>
    public ushort LayerCount { get; set; }

    /// <summary>
    /// The name of the song.
    /// </summary>
    public string SongName { get; set; }

    /// <summary>
    /// The author of the song.
    /// </summary>
    public string SongAuthor { get; set; }

    /// <summary>
    /// The original author of the song.
    /// </summary>
    public string SongOriginalAuthor { get; set; }

    /// <summary>
    /// The description of the song.
    /// </summary>
    public string SongDescription { get; set; }

    /// <summary>
    /// The tempo of the song multiplied by 100 (for example, 1225 instead of 12.25).
    /// Measured in ticks per second.
    /// </summary>
    public ushort SongTempo { get; set; }

    /// <summary>
    /// The tempo of the song measured in BPM.
    /// If there is no note at the first position of Timing layer, the first timing node will use the BPM of the file as its own.
    /// </summary>
    public double SongBPM
    {
        get => (int)SongTempo * TEMPO_FACTOR;
        set
        {
            if (value < 0)
            {
                throw new Exception("BPM must be positive");
            }
            SongTempo = (ushort)(value / TEMPO_FACTOR);
        }
    }

    /// <summary>
    /// The time signature of the song. Ranges from 2-8.
    /// </summary>
    public byte TimeSignature
    {
        get => _timeSignature;
        set
        {
            if (value < 2 || value > 8)
            {
                throw new ArgumentOutOfRangeException("Signature must be 2-8", "TimeSignature");
            }
            _timeSignature = value;
        }
    }

    /// <summary>
    /// If the song has been imported from a .mid or .schematic file or converted from .osu,
    /// that file name is stored here (only the name of the file, not the path).
    /// </summary>
    public string SourceFileName { get; set; }

    /// <summary>
    /// Layers in the song.
    /// </summary>
    public NBSLayer[] Layers { get; private set; }

    /// <summary>
    /// Custom instruments in the song.
    /// </summary>
    public NBSInstrument[] CustomInstruments { get; private set; }


    /// <summary>
    /// Reads file data from <see cref="stream"> and returns a <see cref="NBSFile"> object.
    /// </summary>
    public static NBSFile FromStream(Stream stream)
    {
        if (!stream.CanRead)
        {
            throw new ArgumentException("Stream is unreadable", nameof(stream));
        }
        NBSFile file = new();

        // Part 1: Header
        stream.Position += 2;//first 2 bytes are always zero
        file.FormatVersion = (byte)stream.ReadInt(sizeof(byte));
        if (file.FormatVersion != SUPPORTED_VERSION)
        {
            throw new Exception($"Not suported NBS file format version {file.FormatVersion}, please use {SUPPORTED_VERSION}");
        }
        file.VanillaInstrumentCount = (byte)stream.ReadInt(sizeof(byte));
        file.SongLength = (ushort)stream.ReadInt(sizeof(ushort));
        file.LayerCount = (ushort)stream.ReadInt(sizeof(ushort));
        file.SongName = stream.ReadString();
        file.SongAuthor = stream.ReadString();
        file.SongOriginalAuthor = stream.ReadString();
        file.SongDescription = stream.ReadString();
        file.SongTempo = (ushort)stream.ReadInt(sizeof(ushort));
        stream.Position += EDITOR_DATA_BYTES;
        file.TimeSignature = (byte)stream.ReadInt(sizeof(byte));
        stream.Position += STAT_BYTES;
        file.SourceFileName = stream.ReadString();
        stream.Position += LOOP_DATA_BYTES;

        // Part 2: Note blocks
        file.Layers = new NBSLayer[file.LayerCount];
        for (int i = 0; i < file.LayerCount; i++)
        {
            file.Layers[i] = new();
        }
        int currentTick = -1;
        for (; ; )
        {
            short tickJumps = (short)stream.ReadInt(sizeof(short));
            if (tickJumps == 0)
            {
                break;
            }
            NBSNote note = new();
            currentTick += tickJumps;
            int currentLayer = -1;
            for (; ; )
            {
                short layerJumps = (short)stream.ReadInt(sizeof(short));
                if (layerJumps == 0)
                {
                    break;
                }
                currentLayer += layerJumps;
                note.ReadData(stream);
                file.Layers[currentLayer][currentTick] = note;
            }
        }
        // Part 3: Layers
        for (int i = 0; i < file.LayerCount; i++)
        {
            file.Layers[i].ReadMeta(stream);
        }

        // Part 4: Custom Instruments
        byte instrumentCount = (byte)stream.ReadInt(sizeof(byte));
        file.CustomInstruments = new NBSInstrument[instrumentCount];
        for (int i = 0; i < instrumentCount; i++)
        {
            file.CustomInstruments[i] = NBSInstrument.FromStream(stream);
        }
        return file;
    }
}