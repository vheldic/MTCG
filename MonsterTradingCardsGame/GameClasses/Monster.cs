namespace MonsterTradingCardsGame.GameClasses
{
    public enum MonsterType
    {
        Goblin = 0,
        Dragon = 1,
        Wizzard = 2,
        Ork = 3,
        Knight = 4,
        Kraken = 5,
        FireElve = 6,
    }

    public class Monster : ICardType
    {
        public MonsterType MonsterType { get; set; }

        public Monster() { }

        public Monster(MonsterType monsterType) 
        {
            MonsterType = monsterType;
        }

        public void Battle(Card OppCard)
        {
            ICardType OppCardType = OppCard.CardType;

            // Temp
            //if (OppCardType is ICardType Monster)
            //{
            //    Console.WriteLine("IS MONSTER");
            //    Console.WriteLine(MonsterType + " greift " + OppCardType.GetMonsterType() + " an");
            //}
            //else
            //{
            //    Console.WriteLine("IS SPELL");
            //}

            /*
            switch (MonsterType)
            {
                case MonsterType.Goblin:
                    if (OppCardType is Monster && OppCardType.GetMonsterType() == MonsterType.Dragon)
                    {

                    }
                    break;
                case MonsterType.Dragon:
                    break;
                case MonsterType.Wizzard:
                    break;
                case MonsterType.Ork:
                    break;
                case MonsterType.Knight:
                    break;
                case MonsterType.Kraken:
                    break;
                case MonsterType.FireElve:
                    break;
                default:
                    throw new Exception("Unknown MonsterType");
            }
            */
        }

        public MonsterType GetMonsterType()
        {
            return MonsterType;
        }
    }
}
