namespace Notusu.NBS;
public class NBSInstrument
{
    private NBSInstrument()
    {
        Name = "";
        File = "";
    }

    public string Name { get; set; }

    public string File { get; set; }

    public byte Pitch { get; set; }

    public bool PressKey { get; set; }



    public static NBSInstrument FromStream(Stream stream)
    {
        if (!stream.CanRead)
        {
            throw new ArgumentException("Stream is unreadable", nameof(stream));
        }

        NBSInstrument instrument = new();
        instrument.Name = stream.ReadString();
        instrument.File = stream.ReadString();
        instrument.Pitch = (byte)stream.ReadInt(sizeof(byte));
        instrument.PressKey = (byte)stream.ReadInt(sizeof(byte)) != 0;
        return instrument;
    }
}