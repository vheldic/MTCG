using MonsterTradingCardsGame.GameClasses;

namespace MonsterTradingCardsGame.Tests
{
    [TestClass]
    public class CardTests
    {
        [TestMethod]
        public void Card_Initialization_ParameterizedConstructor()
        {
            Card Card = new Card("SomeName", new Monster(), ElementType.Fire, 500);

            Assert.IsNotNull(Card);
            Assert.AreEqual("SomeName", Card.Name);
            Assert.IsTrue(Card.CardType is Monster);
            Assert.AreEqual(ElementType.Fire, Card.ElementType);
            Assert.AreEqual(500, Card.Damage);
        }

        [TestMethod]
        public void Card_Getter_Setter()
        {
            Card Card = new Card("SomeName", new Monster(), ElementType.Fire, 500);

            Card.Name = "SomeOtherName";
            Card.CardType = new Spell();
            Card.ElementType = ElementType.Water;
            Card.Damage = 150;

            Assert.IsNotNull(Card);
            Assert.AreEqual("SomeOtherName", Card.Name);
            Assert.IsTrue(Card.CardType is Spell);
            Assert.AreEqual(ElementType.Water, Card.ElementType);
            Assert.AreEqual(150, Card.Damage);
        }
    
        [TestMethod]
        public void Card_GetMonsterType()
        {
            Card Card = new Card("SomeName", new Monster(MonsterType.Dragon), ElementType.Fire, 500);

            Assert.IsTrue(Card.CardType is Monster);
            Assert.AreEqual(MonsterType.Dragon, Card.CardType.GetMonsterType()); // Interface neue Meth: Funktioniert, aber soll anders gehen
            //Assert.AreEqual(MonsterType.Dragon, CardType.MonsterType); // Monster new Monster: Soll auch anders als das sein
        }
    }
}