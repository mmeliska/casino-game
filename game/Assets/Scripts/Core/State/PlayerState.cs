using System;

namespace Starflare.State
{
    /// <summary>
    /// The slice of the GDD's player state model the blackjack prototype needs. Debts,
    /// heat, scales, NPC state, flags and Pockets join it as their systems are built.
    /// </summary>
    [Serializable]
    public sealed class PlayerState
    {
        public const int StartingBankroll = 500;

        public int BankrollNow = StartingBankroll;

        /// <summary>Most of your own money ever held (cash minus debts). Only goes up; gates venues and acts.</summary>
        public int BestBankroll = StartingBankroll;

        /// <summary>Total owed. Stays 0 until the Wallet and creditors exist.</summary>
        public int Debts;

        public int RoundsPlayed;
        public GameClock Clock = new GameClock();

        public int OwnMoney => BankrollNow - Debts;

        /// <summary>Called by a game module when a round settles.</summary>
        public void ApplyRoundResult(int net, int minutes)
        {
            BankrollNow += net;
            RoundsPlayed++;
            Clock.AdvanceForPlay(minutes);
            RaiseBestBankroll();
        }

        void RaiseBestBankroll()
        {
            if (OwnMoney > BestBankroll) BestBankroll = OwnMoney;
        }
    }
}
