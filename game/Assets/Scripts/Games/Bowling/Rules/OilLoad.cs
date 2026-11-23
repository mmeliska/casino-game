namespace Starflare.Bowling
{
    /// <summary>
    /// One pass of the oiling machine: so many units from one foot to another across a span of
    /// boards (inclusive). A pattern is a list of these, laid down in order and summed.
    /// </summary>
    public readonly struct OilLoad
    {
        public readonly double FromFt;
        public readonly double ToFt;
        public readonly int FromBoard;
        public readonly int ToBoard;
        public readonly double Units;

        public OilLoad(double fromFt, double toFt, int fromBoard, int toBoard, double units)
        {
            FromFt = fromFt;
            ToFt = toFt;
            FromBoard = fromBoard;
            ToBoard = toBoard;
            Units = units;
        }
    }
}
