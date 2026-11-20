using System;
using System.Collections.Generic;

namespace Starflare.State
{
    /// <summary>The four steps of heat the player sees; Ed is their face (GDD, "Heat").</summary>
    public enum HeatStep { None, Low, Medium, High }

    /// <summary>
    /// How much the police know about your illegal play, as hidden points under four steps
    /// (Mike, 2026-11-20; starting values to tune). Each night you play an illegal game adds
    /// that place's points once, however many rounds you play: 1 for the Millstone back lot,
    /// 1 for Kasia's back room. A week with no illegal play drops a step, to the top of the step
    /// below. Sweeps, raids and Ed read the step; they come later.
    /// </summary>
    [Serializable]
    public sealed class Heat
    {
        public const int LowFrom = 1;
        public const int MediumFrom = 4;
        public const int HighFrom = 8;

        /// <summary>"One step per week of licensed-only play."</summary>
        public const int NightsToCoolAStep = 7;

        public int Points;

        /// <summary>The night the current week of cooling counts from: the last illegal play, or the last step dropped.</summary>
        public int CoolingFrom;

        /// <summary>The night <see cref="PlayedTonight"/> is for (<see cref="GameClock.Night"/>).</summary>
        public int Night;

        /// <summary>Places whose points have counted on <see cref="Night"/>, so a second sitting there adds none.</summary>
        public List<string> PlayedTonight = new List<string>();

        public HeatStep Step => StepAt(Points);

        public static HeatStep StepAt(int points) =>
            points >= HighFrom ? HeatStep.High
            : points >= MediumFrom ? HeatStep.Medium
            : points >= LowFrom ? HeatStep.Low
            : HeatStep.None;

        /// <summary>
        /// Playing an illegal game at <paramref name="place"/> on <paramref name="night"/>.
        /// Only <see cref="PlayerState.PlayIllegalGame"/> calls it. False if the place has
        /// already counted tonight.
        /// </summary>
        internal bool Raise(string place, int points, int night)
        {
            if (string.IsNullOrEmpty(place)) throw new ArgumentException("A place is required.", nameof(place));
            if (points < 0) throw new ArgumentOutOfRangeException(nameof(points));
            Cool(night);
            if (night != Night)
            {
                Night = night;
                PlayedTonight.Clear();
            }
            if (PlayedTonight.Contains(place)) return false;
            PlayedTonight.Add(place);
            Points += points;
            CoolingFrom = night;
            return true;
        }

        /// <summary>The clock has moved to <paramref name="night"/>: drop a step for each whole week with no illegal play.</summary>
        internal void Cool(int night)
        {
            while (Points > 0 && night - CoolingFrom >= NightsToCoolAStep)
            {
                Points = TopOfStepBelow(Step);
                CoolingFrom += NightsToCoolAStep;
            }
        }

        static int TopOfStepBelow(HeatStep step) => step switch
        {
            HeatStep.High => HighFrom - 1,
            HeatStep.Medium => MediumFrom - 1,
            _ => 0,
        };
    }
}
