using Newtonsoft.Json;

namespace Notusu.NBS;
public class NBSLayer
{
    [JsonProperty]
    private Dictionary<int, NBSNote> _notes;

    public NBSLayer()
    {
        _notes = new();
        Name = "";
        Lock = false;
        Volume = 0;
        Stereo = 100;
    }

    public string Name { get; set; }

    public bool Lock { get; set; }

    public byte Volume { get; set; }

    public byte Stereo { get; set; }

    public int NoteCount { get => _notes.Count; }

    public Dictionary<int, Notusu.NBS.NBSNote>.KeyCollection NotePositions { get => _notes.Keys; }

    public int Length { get; private set; } = 0;

    public NBSNote? this[int index]
    {
        get
        {
            if (!_notes.ContainsKey(index))
            {
                return null;
            }
            return _notes[index];
        }
        set
        {
            if (value is null)
            {
                _notes.Remove(index);
                if (index == Length)
                {
                    Length = NotePositions.Max();
                }
            }
            else
            {
                _notes[index] = value;
                if (index > Length)
                {
                    Length = index;
                }
            }
        }
    }

    public void ReadMeta(Stream stream)
    {
        Name = stream.ReadString();
        Lock = (byte)stream.ReadInt(sizeof(byte)) != 0;
        Volume = (byte)stream.ReadInt(sizeof(byte));
        Stereo = (byte)stream.ReadInt(sizeof(byte));
    }
}