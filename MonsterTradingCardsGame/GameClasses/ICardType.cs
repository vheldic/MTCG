namespace MonsterTradingCardsGame.GameClasses
{
    public interface ICardType
    {
        void Battle(Card OppCard);
        MonsterType GetMonsterType();
    }
}
