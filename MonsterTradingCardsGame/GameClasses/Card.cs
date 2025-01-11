namespace MonsterTradingCardsGame.GameClasses
{
    public enum CardTypes
    {
        Monster = 0,
        Spell = 1,
    }
    public enum ElementType
    {
        Normal = 0,
        Water = 1,
        Fire = 2,
    }
    public enum BattleResult
    {
        Win = 1,
        Draw = 0,
        Lose = -1
    };

    public abstract class Card
    {
        public string Name { get; set; }
        public ElementType ElementType { get; set; }
        public int Damage { get; set; }

        public Card(string name, ElementType elementType, int damage)
        {
            Name = name;
            ElementType = elementType;
            Damage = damage;
        }

        /// <summary>
        ///     Compares the damage of the battling cards to decide the winner
        /// </summary>
        /// <param name="oppDamage">Damage of the opposing card</param>
        /// <returns>Result of the fight (Win, Lose or Draw)</returns>
        public static BattleResult CompareDamage(int damage, int oppDamage)
        {
            if (damage > oppDamage) return BattleResult.Win;
            if (damage < oppDamage) return BattleResult.Lose;
            return BattleResult.Draw;
        }
        
        /// <summary>
        ///     Lets own card and opponents card battle to determine the winner
        /// </summary>
        /// <param name="oppCard">Selected card of opponent for battle</param>
        /// <returns>Result of battle</returns>
        public abstract BattleResult Battle(Card oppCard);
    }
}
