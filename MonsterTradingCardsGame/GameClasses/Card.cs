namespace MonsterTradingCardsGame.GameClasses
{
    public enum ElementType
    {
        Normal = 0,
        Water = 1,
        Fire = 2,
    }

    public class Card
    {
        public string Name { get; set; }
        public ICardType CardType { get; set; }
        public ElementType ElementType { get; set; }
        public int Damage { get; set; }

        public Card(string name, ICardType cardType, ElementType elementType, int damage)
        {
            Name = name;
            CardType = cardType;
            ElementType = elementType;
            Damage = damage;
        }

        public void Battle(Card OppCard)
        {
            CardType.Battle(OppCard);
        }
    }
}
