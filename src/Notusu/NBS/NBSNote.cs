namespace Notusu.NBS;
public class NBSNote
{
    public ushort HorizonalJump { get; set; }

    public ushort VerticalJump { get; set; }

    public byte Instrument { get; set; }

    public byte Key { get; set; }

    public byte Velocity { get; set; }

    public byte Panning { get; set; }

    public short Pitch { get; set; }

    /// <summary>
    /// Reads data from <see cref="stream"/>.
    /// This method only reads instrument, key, velocity, panning and pitch data,
    /// not including tick and layer jumps, since the latter 2 numbers are read in <see cref="NBSFile.FromStream(Stream)"/>.
    /// </summary>
    /// <param name="stream"></param>
    public void ReadData(Stream stream)
    {
        HorizonalJump = 0;
        VerticalJump = 0;
        Instrument = (byte)stream.ReadInt(sizeof(byte));
        Key = (byte)stream.ReadInt(sizeof(byte));
        Velocity = (byte)stream.ReadInt(sizeof(byte));
        Panning = (byte)stream.ReadInt(sizeof(byte));
        Pitch = (short)stream.ReadInt(sizeof(short));
    }
}