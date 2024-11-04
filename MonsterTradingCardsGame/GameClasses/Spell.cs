namespace MonsterTradingCardsGame.GameClasses
{
    public class Spell : Card
    {

        public Spell(string name, ElementType elementType, int damage)
            :base(name, elementType, damage)
        { }

        public override BattleResult Battle(Card oppCard)
        {
            if (oppCard is Monster)
            {
                return MixedFight(oppCard);
            }
            return SpellFight(oppCard);
        }

        /// <summary>
        ///     Battle for fight against a monster
        /// </summary>
        /// <param name="oppCard">Selected card of opponent for battle</param>
        /// <returns>Result of battle</returns>
        private BattleResult MixedFight(Card oppCard)
        {
            MonsterType oppMonsterType = ((Monster)oppCard).MonsterType;

            const string BATTLE_KNIGHT_WATERSPELL = "The armor of Knights is so heavy that WaterSpells make them drown them instantly";
            const string BATTLE_KRAKEN_SPELL = "The Kraken is immune against spells.";

            // Check spells advantages/disadvantages against enemy monster
            switch (oppMonsterType)
            {
                case MonsterType.Knight:
                    if (ElementType == ElementType.Water)
                    {
                        Console.WriteLine(BATTLE_KNIGHT_WATERSPELL);
                        return BattleResult.Win;
                    }
                    break;

                case MonsterType.Kraken:
                    Console.WriteLine(BATTLE_KRAKEN_SPELL);
                    return BattleResult.Lose;

                default:
                    // Opponent monster has no weakness against spell
                    break;
            }

            // Compare damage
            return CompareDamage(CheckElementEffectiveness(oppCard.ElementType), oppCard.Damage);
        }

        /// <summary>
        ///     Battle for fight against enemy spell
        /// </summary>
        /// <param name="oppCard">Selected card of opponent for battle</param>
        /// <returns>Result of Battle</returns>
        private BattleResult SpellFight(Card oppCard)
        {
            // Compare damage 
            return CompareDamage(CheckElementEffectiveness(oppCard.ElementType), oppCard.Damage);
        }

        /// <summary>
        ///     Checks the effectiveness of elements used in the fight
        /// </summary>
        /// <param name="oppElementType">Element of opponent card</param>
        /// <returns>New calculated damage based on effectiveness</returns>
        private int CheckElementEffectiveness(ElementType oppElementType)
        {
            // No Effect
            if (ElementType == oppElementType)
                return Damage;
            // Effective
            if ((ElementType == ElementType.Water && oppElementType == ElementType.Fire) ||
                (ElementType == ElementType.Fire && oppElementType == ElementType.Normal) ||
                ElementType == ElementType.Normal && oppElementType == ElementType.Water)
            {
                Console.WriteLine($"Effective attack: {ElementType} -> {oppElementType}");
                return Damage * 2;
            }
            // Not effective
            Console.WriteLine($"Effective attack: {oppElementType} -> {ElementType}");
            return Damage / 2;
        }
    }
}
