using Notusu.NBS;
using Notusu.OSU;

namespace Notusu.Converter;
public class NBSConverter
{
    public const string LABLE_TIMING = "Timing";

    public const string LABLE_TRACK = "Track";

    public const int CIRCLE_KEY_MIN = 33;

    public const int CIRCLE_KEY_MAX = 57;

    public NBSConverter(BeatmapProperties bp)
    {
        BeatmapProperties = bp;
    }



    public BeatmapProperties BeatmapProperties { get; set; }



    public OSUFile Convert(NBSFile nbsFile, BeatmapProperties bp, int offset = 0)
    {
        OSUFile osuFile = new();
        osuFile.Title = nbsFile.SongName;
        osuFile.Artist = nbsFile.SongOriginalAuthor;
        osuFile.Creator = nbsFile.SongAuthor;
        osuFile.Keys = bp.Keys;
        osuFile.Version = bp.Difficulty;

        NBSLayer[] trackLayers = new NBSLayer[bp.Keys + 1];
        NBSLayer? timingLayer = null;
        bool[] trackExist = new bool[bp.Keys + 1];
        Array.Fill<bool>(trackExist, false);
        foreach (var layer in nbsFile.Layers)
        {
            if (layer.Name == LABLE_TIMING)
            {
                timingLayer = layer;
            }
            if (layer.Name.StartsWith(LABLE_TRACK))
            {
                int trackNum;
                if (!int.TryParse(layer.Name.Substring(LABLE_TRACK.Length), out trackNum))
                {
                    continue;
                }
                trackLayers[trackNum] = layer;
                trackExist[trackNum] = true;
            }
        }
        if (timingLayer is null)
        {
            throw new TimingLayerNotFoundException("Timing layer not found");
        }
        for (int i = 1; i <= bp.Keys; i++)
        {
            if (!trackExist[i])
            {
                throw new NoEnoughTracksException($"No enough tracks. Track #{i} does not exist.");
            }
        }

        int pos = 0;
        double ms = bp.Offset;
        OSUTimingPoint tp = new(false);
        tp.BPM = nbsFile.SongBPM;
        tp.TimeSignature = nbsFile.TimeSignature;
        tp.Offset = bp.Offset;
        NBSNote? timingNode = null;
        NBSNote? hitNote = null;
        for (; pos < nbsFile.SongLength; pos++, ms += tp.Gap / 4)
        {
            timingNode = timingLayer[pos];
            if (timingNode is null && pos == 0)
            {
                osuFile.TimingPoints.Add(tp);
            }

            for (int track = 1; track <= bp.Keys; track++)
            {
                hitNote = trackLayers[track][pos];
                if (hitNote is not null)
                {
                    OSUHitObject ho = new();
                    ho.Type = (hitNote.Key >= CIRCLE_KEY_MIN && hitNote.Key <= CIRCLE_KEY_MAX) ? NoteType.Circle : NoteType.Hold;
                    ho.Position = (int)ms;
                    ho.Track = track;
                    osuFile.HitObjects.Add(ho);
                }
            }

            if (timingNode is not null)
            {
                tp = new(false);
                tp.BPM = Math.Abs(timingNode.Pitch);
                tp.Offset = ms;
                tp.TimeSignature = nbsFile.TimeSignature;
                osuFile.TimingPoints.Add(tp);
            }
        }

        return osuFile;
    }
}

[System.Serializable]
public class NBSConverterException : Exception
{
    public NBSConverterException() { }
    public NBSConverterException(string message) : base(message) { }
    public NBSConverterException(string message, System.Exception inner) : base(message, inner) { }
    protected NBSConverterException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

[System.Serializable]
public class TimingLayerNotFoundException : NBSConverterException
{
    public TimingLayerNotFoundException() { }
    public TimingLayerNotFoundException(string message) : base(message) { }
    public TimingLayerNotFoundException(string message, Exception inner) : base(message, inner) { }
    protected TimingLayerNotFoundException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

[System.Serializable]
public class NoEnoughTracksException : NBSConverterException
{
    public NoEnoughTracksException() { }
    public NoEnoughTracksException(string message) : base(message) { }
    public NoEnoughTracksException(string message, Exception inner) : base(message, inner) { }
    protected NoEnoughTracksException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}