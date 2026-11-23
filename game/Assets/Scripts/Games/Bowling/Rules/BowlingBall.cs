namespace Starflare.Bowling
{
    /// <summary>
    /// A ball: its size, its weight and how its cover grips, on the oil and off it. The house ball
    /// is plastic and barely grips anywhere, so it hooks a little at full meter. Juan's counter
    /// sells a used urethane one, which grips early and arcs, and a reactive pearl, which skids
    /// through the oil and grips hard where it ends (Mike, 2026-11-23).
    /// </summary>
    public sealed class BowlingBall
    {
        public readonly string Id;
        public readonly string Name;

        /// <summary>8.5 in across.</summary>
        public readonly double Radius;

        /// <summary>15 lb.</summary>
        public readonly double Mass;

        /// <summary>Multiplies the lane's friction on oil: 1 for urethane.</summary>
        public readonly double OilGrip;

        /// <summary>Multiplies the lane's friction where there's no oil: 1 for urethane.</summary>
        public readonly double BareGrip;

        public BowlingBall(string id, string name, double oilGrip, double bareGrip, double radius = 0.108, double mass = 6.8)
        {
            Id = id;
            Name = name;
            OilGrip = oilGrip;
            BareGrip = bareGrip;
            Radius = radius;
            Mass = mass;
        }

        /// <summary>A solid sphere's moment of inertia, 2/5 m R².</summary>
        public double Inertia => 0.4 * Mass * Radius * Radius;

        public static readonly BowlingBall House = new BowlingBall("house", "House ball", 0.8, 0.15);
        public static readonly BowlingBall Urethane = new BowlingBall("urethane", "Urethane ball", 1.2, 1.0);
        public static readonly BowlingBall Reactive = new BowlingBall("reactive", "Reactive ball", 0.7, 1.4);

        /// <summary>A ball by its id, the house ball for anything else.</summary>
        public static BowlingBall ById(string id) =>
            id == Urethane.Id ? Urethane : id == Reactive.Id ? Reactive : House;
    }
}
