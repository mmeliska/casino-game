using System;
using NUnit.Framework;
using Starflare.Bowling;

namespace Starflare.Tests
{
    /// <summary>Standard tenpins scoring (bowling spec, "Scoring").</summary>
    public class ScoreSheetTests
    {
        static ScoreSheet Bowl(params int[] balls)
        {
            var sheet = new ScoreSheet();
            foreach (int b in balls) sheet.Record(b);
            return sheet;
        }

        [Test]
        public void FrameMarks_SplitTheBallsByBars()
        {
            var sheet = Bowl(7, 3, 1, 2, 10, 0, 5, 0, 10);
            Assert.That(sheet.FrameMarks(1), Is.EqualTo("7|/"));
            Assert.That(sheet.FrameMarks(2), Is.EqualTo("1|2"));
            Assert.That(sheet.FrameMarks(3), Is.EqualTo("X"));
            Assert.That(sheet.FrameMarks(4), Is.EqualTo("-|5"));
            Assert.That(sheet.FrameMarks(5), Is.EqualTo("-|/"), "Ten on the second ball is a spare.");
            Assert.That(sheet.FrameMarks(6), Is.EqualTo(""), "Not bowled yet.");
        }

        [Test]
        public void FrameMarks_InTheTenth_ShowAllThree()
        {
            var sheet = Bowl(10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 7, 3);
            Assert.That(sheet.FrameMarks(10), Is.EqualTo("X|7|/"));
        }

        [Test]
        public void FrameBoxes_ShowAStrikeAlone_AndGiveTheTenthThree()
        {
            var sheet = Bowl(7, 3, 10, 1, 2);
            Assert.That(sheet.FrameBoxes(1), Is.EqualTo(new[] { "7", "/" }));
            Assert.That(sheet.FrameBoxes(2), Is.EqualTo(new[] { "X" }), "A strike is just the X.");
            Assert.That(sheet.FrameBoxes(3), Is.EqualTo(new[] { "1", "2" }));

            var open = Bowl(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 2);
            Assert.That(open.FrameBoxes(10), Is.EqualTo(new[] { "6", "2", "" }), "An open tenth leaves its third box empty.");
            var marked = Bowl(10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 7, 3);
            Assert.That(marked.FrameBoxes(10), Is.EqualTo(new[] { "X", "7", "/" }));
        }

        [Test]
        public void TwelveStrikes_Make300()
        {
            var sheet = Bowl(10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10);
            Assert.That(sheet.IsComplete, Is.True);
            Assert.That(sheet.Score, Is.EqualTo(300));
            Assert.That(sheet.Mark(10, 0) + sheet.Mark(10, 1) + sheet.Mark(10, 2), Is.EqualTo("XXX"));
        }

        [Test]
        public void AllSparesWithFives_Make150()
        {
            var sheet = new ScoreSheet();
            for (int i = 0; i < 21; i++) sheet.Record(5);
            Assert.That(sheet.IsComplete, Is.True);
            Assert.That(sheet.Score, Is.EqualTo(150));
            Assert.That(sheet.Mark(1, 1), Is.EqualTo("/"));
        }

        [Test]
        public void AllGutters_Make0()
        {
            var sheet = new ScoreSheet();
            for (int i = 0; i < 20; i++) sheet.Record(0);
            Assert.That(sheet.IsComplete, Is.True);
            Assert.That(sheet.Score, Is.EqualTo(0));
            Assert.That(sheet.Mark(3, 0), Is.EqualTo("-"));
        }

        [Test]
        public void AnOpenTenth_EndsAfterTwoBalls()
        {
            var sheet = Bowl(new int[18]);
            sheet.Record(3);
            Assert.That(sheet.IsComplete, Is.False);
            sheet.Record(4);
            Assert.That(sheet.IsComplete, Is.True);
            Assert.That(sheet.Score, Is.EqualTo(7));
        }

        [Test]
        public void ASpareInTheTenth_EarnsOneMoreBall_OnAFreshRack()
        {
            var sheet = Bowl(new int[18]);
            sheet.Record(6);
            sheet.Record(4);
            Assert.That(sheet.IsComplete, Is.False);
            Assert.That(sheet.FreshRack, Is.True);
            sheet.Record(10);
            Assert.That(sheet.IsComplete, Is.True);
            Assert.That(sheet.Score, Is.EqualTo(20));
            Assert.That(sheet.Mark(10, 1) + sheet.Mark(10, 2), Is.EqualTo("/X"));
        }

        [Test]
        public void AStrikeInTheTenth_EarnsTwoMore_AndTheRackFollowsThem()
        {
            var sheet = Bowl(new int[18]);
            sheet.Record(10);
            Assert.That(sheet.FreshRack, Is.True);
            sheet.Record(7);
            Assert.That(sheet.FreshRack, Is.False);
            Assert.That(sheet.PinsStanding, Is.EqualTo(3));
            sheet.Record(3);
            Assert.That(sheet.IsComplete, Is.True);
            Assert.That(sheet.Score, Is.EqualTo(20));
            Assert.That(sheet.Mark(10, 0) + sheet.Mark(10, 1) + sheet.Mark(10, 2), Is.EqualTo("X7/"));
        }

        [Test]
        public void AStrike_CountsTheNextTwoBalls_OnceThrown()
        {
            var sheet = Bowl(10, 3);
            Assert.That(sheet.FrameList[0].Total, Is.Null, "Waiting on the second ball after.");
            sheet.Record(4);
            Assert.That(sheet.FrameList[0].Total, Is.EqualTo(17));
            Assert.That(sheet.FrameList[1].Total, Is.EqualTo(24));
            Assert.That(sheet.CurrentFrame, Is.EqualTo(3));
        }

        [Test]
        public void AMidGameSheet_IsNotComplete()
        {
            var sheet = Bowl(10, 7, 2, 9);
            Assert.That(sheet.IsComplete, Is.False);
            Assert.That(sheet.CurrentFrame, Is.EqualTo(3));
            Assert.That(sheet.BallInFrame, Is.EqualTo(1));
            Assert.That(sheet.PinsStanding, Is.EqualTo(1));
            Assert.That(sheet.FreshRack, Is.False);
        }

        [Test]
        public void MorePinsThanAreStanding_IsRefused()
        {
            var sheet = Bowl(7);
            Assert.Throws<ArgumentOutOfRangeException>(() => sheet.Record(4));
            var done = Bowl(new int[20]);
            Assert.Throws<InvalidOperationException>(() => done.Record(0));
        }
    }
}
