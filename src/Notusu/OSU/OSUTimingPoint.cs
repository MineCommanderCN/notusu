namespace Notusu.OSU;
public class OSUTimingPoint
{
    private double _gap;
    private double _rawVelocity;
    private int _timeSignature;
    private int _sampleSubset;
    private int _volume;



    public const double VELOCITY_BASE = -100;

    public const double BPM_FACTOR = 60000;

    public const int ELEMENT_COUNT = 8;



    public OSUTimingPoint(bool isInherit)
    {
        IsInherit = isInherit;
        SampleSet = SampleSet.Soft;
        SampleSubset = 0;
        OmitFirstBarLine = false;
    }



    /// <summary>
    /// The position of timing point, measured in ms.
    /// </summary>
    public double Offset { get; set; }

    /// <summary>
    /// Only available for normal timing points(<see cref="IsInherit"/> is false).
    /// The gap of two beats, measured in ms.
    /// </summary>
    public double Gap
    {
        get => _gap;
        set
        {
            if (value < 0)
            {
                throw new Exception("Gap must be positive");
            }
            if (IsInherit)
            {
                throw new Exception("Only available for normal timing points");
            }
            _gap = value;
        }
    }

    /// <summary>
    /// Only available for normal timing points(<see cref="IsInherit"/> is false).
    /// The BPM of the timing point.
    /// This property is sync with <see cref="Gap"/>.
    /// </summary>
    public double BPM
    {
        get => BPM_FACTOR / _gap;
        set
        {
            if (value < 0)
            {
                throw new Exception("BPM must be positive");
            }
            if (IsInherit)
            {
                throw new Exception("Only available for normal timing points");
            }
            _gap = BPM_FACTOR / value;
        }
    }

    /// <summary>
    /// Only available for inherit timing points(<see cref="IsInherit"/> is true).
    /// The slider velocity multiplier of the timing point, stored as <see cref="VELOCITY_BASE"/>/<see cref="SliderVelocity"/>.
    /// </summary>
    public double RawSliderVelocity
    {
        get => _rawVelocity;
        set
        {
            if (value < -10000 || value > -10)
            {
                throw new Exception("Velocity out of range");
            }
            if (!IsInherit)
            {
                throw new Exception("Only available for inherit timing points");
            }
            _rawVelocity = value;
        }
    }

    /// <summary>
    /// Only available for inherit timing points(<see cref="IsInherit"/> is true).
    /// The slider velocity multiplier of the timing point.
    /// This property is sync with <see cref="RawSliderVelocity"/>.
    /// </summary>
    public double SliderVelocity
    {
        get => VELOCITY_BASE / _rawVelocity;
        set
        {
            if (value < 0.01 || value > 10)
            {
                throw new Exception("Velocity out of range");
            }
            if (!IsInherit)
            {
                throw new Exception("Only available for inherit timing points");
            }
            _rawVelocity = VELOCITY_BASE / value;
        }
    }

    /// <summary>
    /// The time signature of the timing point. Ranges from 2-8.
    /// </summary>
    public int TimeSignature
    {
        get => _timeSignature;
        set
        {
            if (value < 2 || value > 8)
            {
                throw new Exception("Signature must be 2-8");
            }
            _timeSignature = value;
        }
    }

    /// <summary>
    /// The sample set of the timing point.
    /// </summary>
    public SampleSet SampleSet { get; set; }

    /// <summary>
    /// The index of sample subset of the timing point.
    /// </summary>
    public int SampleSubset
    {
        get => _sampleSubset;
        set
        {
            if (value < 0)
            {
                throw new Exception("Subset index must be non-negative");
            }
            _sampleSubset = value;
        }
    }

    /// <summary>
    /// The volume of sample of the timing point. Ranges from 1-100.
    /// </summary>
    public int Volume
    {
        get => _volume;
        set
        {
            if (value < 0 || value > 100)
            {
                throw new Exception("Volume must be 0-100");
            }
            _volume = value;
        }
    }

    /// <summary>
    /// Determine whether the timing point is normal(change BPM) or inherit(change slider velocity).
    /// </summary>
    public bool IsInherit { get; set; }

    /// <summary>
    /// Only available for the first timing point.
    /// When enabled, remove the first bar line from this section.
    /// </summary>
    public bool OmitFirstBarLine { get; set; }



    public override string ToString()
    {
        return $"{Offset},{Gap},{TimeSignature},{(int)SampleSet},{SampleSubset},{Volume},{(IsInherit ? 0 : 1)},{(OmitFirstBarLine ? 8 : 0)}\n";
    }

    public static OSUTimingPoint FromString(string str)
    {
        OSUTimingPoint timingPoint = new(false);
        var elements = str.Split(',', ELEMENT_COUNT, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (elements.Length < ELEMENT_COUNT)
        {
            throw new ArgumentException("Bad syntax", nameof(str));
        }
        for (int i = 0; i < ELEMENT_COUNT; i++)
        {
            switch (i)
            {
                case 0:
                    double valueOffset;
                    if (!double.TryParse(elements[i], out valueOffset))
                    {
                        throw new ArgumentException("Bad syntax", nameof(str));
                    }
                    timingPoint.Offset = valueOffset;
                    break;
                case 1:
                    double valueGap;
                    if (!double.TryParse(elements[i], out valueGap))
                    {
                        throw new ArgumentException("Bad syntax", nameof(str));
                    }
                    timingPoint.Gap = valueGap;
                    break;
                case 2:
                    int valueTs;
                    if (!int.TryParse(elements[i], out valueTs))
                    {
                        throw new ArgumentException("Bad syntax", nameof(str));
                    }
                    timingPoint.TimeSignature = valueTs;
                    break;
                case 3:
                    int valueSampleset;
                    if (!int.TryParse(elements[i], out valueSampleset))
                    {
                        throw new ArgumentException("Bad syntax", nameof(str));
                    }
                    timingPoint.SampleSet = (SampleSet)valueSampleset;
                    break;
                case 4:
                    int valueSampleSubset;
                    if (!int.TryParse(elements[i], out valueSampleSubset))
                    {
                        throw new ArgumentException("Bad syntax", nameof(str));
                    }
                    timingPoint.SampleSubset = valueSampleSubset;
                    break;
                case 5:
                    int valueVol;
                    if (!int.TryParse(elements[i], out valueVol))
                    {
                        throw new ArgumentException("Bad syntax", nameof(str));
                    }
                    timingPoint.Volume = valueVol;
                    break;
                case 6:
                    int valueType;
                    if (!int.TryParse(elements[i], out valueType))
                    {
                        throw new ArgumentException("Bad syntax", nameof(str));
                    }
                    timingPoint.IsInherit = valueType == 0;
                    break;
                case 7:
                    int valueOfbl;
                    if (!int.TryParse(elements[i], out valueOfbl))
                    {
                        throw new ArgumentException("Bad syntax", nameof(str));
                    }
                    timingPoint.OmitFirstBarLine = valueOfbl != 0;
                    break;
            }
        }
        return timingPoint;
    }
}

public enum SampleSet
{
    Normal = 1, Soft = 2, Drum = 3
}