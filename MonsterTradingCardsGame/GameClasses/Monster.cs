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

    public class Monster : Card
    {
        public MonsterType MonsterType { get; set; }

        public Monster(string name, MonsterType monsterType, ElementType elementType, int damage)
            : base(name, elementType, damage)
        {
            MonsterType = monsterType;
        }

        public override BattleResult Battle(Card oppCard)
        {
            MonsterType oppMonsterType = ((Monster) oppCard).MonsterType;

            const string BATTLE_GOBLIN_DRAGON = "Goblins are too afraid of Dragons to attack.";
            const string BATTLE_WIZZARD_ORK = "Wizzard can control Orks so they are not able to damage them.";
            const string BATTLE_FIREELVE_DRAGON = "The FireElves know Dragons since they were little and can evade their attacks.";

            // Check monsters advantages/disadvantages against enemy monster
            switch (MonsterType)
            {
                case MonsterType.Goblin:
                    if (oppMonsterType == MonsterType.Dragon)
                    {
                        Console.WriteLine(BATTLE_GOBLIN_DRAGON);
                        return BattleResult.Lose;
                    }
                    break;

                case MonsterType.Dragon:
                    if (oppMonsterType == MonsterType.Goblin)
                    {
                        Console.WriteLine(BATTLE_GOBLIN_DRAGON);
                        return BattleResult.Win;
                    }
                    if (oppMonsterType == MonsterType.FireElve)
                    {
                        Console.WriteLine(BATTLE_FIREELVE_DRAGON);
                        return BattleResult.Lose;
                    }
                    break;
                        
                case MonsterType.Wizzard:
                    if (oppMonsterType == MonsterType.Ork)
                    {
                        Console.WriteLine(BATTLE_WIZZARD_ORK);
                        return BattleResult.Win;
                    }
                    break;
                        
                case MonsterType.Ork:
                    if (oppMonsterType == MonsterType.Wizzard)
                    {
                        Console.WriteLine(BATTLE_WIZZARD_ORK);
                        return BattleResult.Lose;
                    }
                    break;
                        
                case MonsterType.Knight:
                    // Monster has no advantages or disadvantages against other monsters
                    break;
                        
                case MonsterType.Kraken:
                    // Monster has no advantages or disadvantages against other monsters
                    break;
                        
                case MonsterType.FireElve:
                    if (oppMonsterType == MonsterType.Dragon)
                    {
                        Console.WriteLine(BATTLE_FIREELVE_DRAGON);
                        return BattleResult.Win;
                    }
                    break;
                        
                default:
                    throw new Exception("Unknown MonsterType");
            }

            // Compare damage
            return CompareDamage(Damage, oppCard.Damage);
        }
    }
}
