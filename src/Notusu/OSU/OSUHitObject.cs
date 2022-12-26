namespace Notusu.OSU;
public class OSUHitObject
{
    private int _rawTrack;
    private int _endPosition;
    private int _sampleVolume;



    public const int TRACK_MAGIC = 64;

    public const int PLACEHOLDER_1 = 192;

    public const int PLACEHOLDER_2 = 0;

    public const string PLACEHOLDER_3 = "0:0:0:";



    public OSUHitObject()
    {
        SampleVolume = 0;
        SampleFile = "";
    }



    public int Track
    {
        get => (_rawTrack + TRACK_MAGIC) / (2 * TRACK_MAGIC);
        set => _rawTrack = (2 * value - 1) * TRACK_MAGIC;
    }

    public int Position { get; set; }

    public NoteType Type { get; set; }

    public int EndPosition
    {
        get => _endPosition;
        set
        {
            if (Type != NoteType.Hold)
            {
                throw new Exception("Only available for hold notes");
            }
            if (value <= Position)
            {
                throw new Exception("Invalid value");
            }
            _endPosition = value;
        }
    }

    public int SampleVolume
    {
        get => _sampleVolume;
        set
        {
            if (value < 0 || value > 100)
            {
                throw new Exception("Volume must be 0-100");
            }
            _sampleVolume = value;
        }
    }

    public string SampleFile { get; set; }



    public override string ToString()
    {
        return $"{_rawTrack},{PLACEHOLDER_1},{Position},{(int)Type},{PLACEHOLDER_2},{(Type == NoteType.Hold ? $"{EndPosition}:" : "")}{PLACEHOLDER_3}{SampleVolume}:{SampleFile}\n";
    }
}

public enum NoteType
{
    Circle = 1, Hold = 128
}