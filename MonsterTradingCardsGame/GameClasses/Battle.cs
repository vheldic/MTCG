namespace MonsterTradingCardsGame.GameClasses
{
    public class Battle
    {
        public string Log { get; set; }
        private string RunningLog { get; set; }
        public User PlayerA { get; set; }
        public User PlayerB { get; set; }
        public Database.Database Database { get; set; }

        public Battle(User user, Database.Database db)
        {
            RunningLog = "";
            PlayerA = user;
            Database = db;
        }

        /// <summary>
        ///     Adds user as opponent for battle
        /// </summary>
        /// <param name="user">User as opponent</param>
        public void AddPlayer(User user)
        {
            PlayerB = user;
        }

        /// <summary>
        ///     Starts the battle between two users and their decks
        /// </summary>
        public void StartBattle()
        {
            RunningLog += $"{PlayerA.Username} ({PlayerA.Elo} ELO) vs {PlayerB.Username} ({PlayerB.Elo} ELO)\n";

            // Get Decks
            List<Card?> deckA = Database.GetUsersDeck(PlayerA.Username);
            List<Card?> deckB = Database.GetUsersDeck(PlayerB.Username);

            int round = 1;
            do
            {
                RunningLog += $"Round {round}: ";
                // Get random cards from deck
                Card? cardA = deckA[new Random().Next(deckA.Count)];
                Card? cardB = deckB[new Random().Next(deckB.Count)];

                // Let cards fight
                BattleResult battleResult = Fight(cardA, cardB);

                // Winner gets losers card added to his deck, if draw no changes
                if (battleResult == BattleResult.Win)
                {
                    deckA.Add(cardB);
                    deckB.Remove(cardB);
                }
                else if (battleResult == BattleResult.Lose)
                {
                    deckB.Add(cardA);
                    deckA.Remove(cardA);
                }

                round++;
            } while (deckA.Count != 0 && deckB.Count != 0 && round <= 100);

            // Update user stats (Win: +3 ELO, Lose: -5 ELO, Draw: +0 ELO)
            const int ADD_ELO = +3;
            const int REMOVE_ELO = -5;

            if (deckA.Count == 0)
            {
                RunningLog += $"{PlayerB.Username} wins!\n";
                Database.UpdateUserStats(PlayerB.Username, PlayerB.Elo + ADD_ELO, PlayerB.Wins + 1, PlayerB.Draws, PlayerB.Losses, PlayerB.GamesPlayed + 1);
                Database.UpdateUserStats(PlayerA.Username, PlayerA.Elo + REMOVE_ELO, PlayerA.Wins, PlayerA.Draws, PlayerA.Losses + 1, PlayerA.GamesPlayed + 1);
            }
            else if (deckB.Count == 0)
            {
                RunningLog += $"{PlayerA.Username} wins!\n";
                Database.UpdateUserStats(PlayerA.Username, PlayerA.Elo + ADD_ELO, PlayerA.Wins + 1, PlayerA.Draws, PlayerA.Losses, PlayerA.GamesPlayed + 1);
                Database.UpdateUserStats(PlayerB.Username, PlayerB.Elo + REMOVE_ELO, PlayerB.Wins, PlayerB.Draws, PlayerB.Losses + 1, PlayerB.GamesPlayed + 1);
            }
            else
            {
                RunningLog += "No winner!\n";
                Database.UpdateUserStats(PlayerA.Username, PlayerA.Elo, PlayerA.Wins, PlayerA.Draws + 1, PlayerA.Losses, PlayerA.GamesPlayed + 1);
                Database.UpdateUserStats(PlayerB.Username, PlayerB.Elo, PlayerB.Wins, PlayerB.Draws + 1, PlayerB.Losses, PlayerB.GamesPlayed + 1);
            }

            Log = RunningLog;
        }

        /// <summary>
        ///     Lets the two selected cards fight each other
        /// </summary>
        /// <param name="card1">Selected card from playerA</param>
        /// <param name="card2">Selected card from playerB</param>
        /// <returns>Result of the battle</returns>
        public BattleResult Fight(Card card1, Card card2)
        {
            // If spell is involved, use spell as attacker
            if (card1 is not Spell && card2 is Spell)
                return Fight(card2, card1);

            RunningLog += $"{card1.Name} ({card1.Damage} Damage) vs {card2.Name} ({card2.Damage} Damage)\n";

            BattleResult battleResult = card1.Battle(card2);
            // TODO: get Monster natural enemy/element effectivenes for clean log

            if (battleResult == BattleResult.Win)
                RunningLog += $"{card1.Name} wins!\n";
            else if (battleResult == BattleResult.Lose)
                RunningLog += $"{card2.Name} wins!\n";
            else
                RunningLog += "Draw (no action)\n";

            return battleResult;
        }
    }
}
