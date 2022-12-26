namespace Notusu;
public struct BeatmapProperties
{
    public const char END_KEY = '=';

    public const char END_VALUE = '\n';

    public const string KEY_OFFSET = "offset";

    public const string KEY_KEYS = "keys";

    public const string KEY_DIFF = "diff";

    public const string DEFAULT_DIFF = "Notusu";

    public const int DEFAULT_KEYS = 4;


    public BeatmapProperties()
    {
        Offset = 0;
        Keys = DEFAULT_KEYS;
        Difficulty = DEFAULT_DIFF;
    }


    public double Offset { get; set; }

    public int Keys { get; set; }

    public string Difficulty { get; set; }



    public static BeatmapProperties FromString(string str)
    {
        BeatmapProperties bp = new();
        var pairs = str.Split(END_VALUE, StringSplitOptions.RemoveEmptyEntries);
        foreach (var pair in pairs)
        {
            var li = pair.Split(END_KEY, 2);
            string key = li[0];
            string value = li[1];
            switch (key)
            {
                case KEY_OFFSET:
                    double valueOffset;
                    if (!double.TryParse(value, out valueOffset))
                    {
                        throw new ArgumentException("Bad syntax", nameof(str));
                    }
                    bp.Offset = valueOffset;
                    break;
                case KEY_KEYS:
                    int valueKeys;
                    if (!int.TryParse(value, out valueKeys))
                    {
                        throw new ArgumentException("Bad syntax", nameof(str));
                    }
                    bp.Keys = valueKeys;
                    break;
                case KEY_DIFF:
                    bp.Difficulty = value;
                    break;
            }
        }
        return bp;
    }

    public override string ToString()
    {
        return $"{KEY_OFFSET}{END_KEY}{Offset}{END_VALUE}{KEY_KEYS}{END_KEY}{Keys}{END_VALUE}{KEY_DIFF}{END_KEY}{Difficulty}{END_VALUE}";
    }
}