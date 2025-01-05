using MonsterTradingCardsGame.GameClasses;

namespace MonsterTradingCardsGame.Tests
{
    [TestClass]
    public class CardTests
    {
        [TestMethod]
        public void Card_Initialization_ParameterizedConstructor()
        {
            Card card = new Monster("SomeName", MonsterType.Dragon, ElementType.Fire, 500);

            Assert.IsNotNull(card);
            Assert.AreEqual("SomeName", card.Name);
            Assert.IsTrue(card is Monster);
            Assert.AreEqual(ElementType.Fire, ((Monster) card).ElementType);
            Assert.AreEqual(500, card.Damage);
        }

        [TestMethod]
        public void Card_Getter_Setter()
        {
            Card card = new Monster("SomeName", MonsterType.Dragon, ElementType.Fire, 500);

            card.Name = "SomeOtherName";
            ((Monster) card).MonsterType = MonsterType.Wizzard;
            card.ElementType = ElementType.Water;

            Assert.IsNotNull(card);
            Assert.IsTrue(card is Monster);
            Assert.AreEqual("SomeOtherName", card.Name);
            Assert.AreEqual(ElementType.Water, card.ElementType);
            Assert.AreEqual(500, card.Damage);
        }

        [TestMethod]
        public void Card_GetMonsterType()
        {
            Card card = new Monster("SomeName", MonsterType.Dragon, ElementType.Fire, 500);

            Assert.IsTrue(card is Monster);
            Assert.AreEqual(MonsterType.Dragon, ((Monster)card).MonsterType);

        }

        [TestMethod]
        public void Card_Battle_Compare_Damage()
        {
            Card card_Dragon = new Monster("SomeDragon", MonsterType.Dragon, ElementType.Fire, 500);
            Card card_Wizzard = new Monster("SomeWizzard", MonsterType.Wizzard, ElementType.Water, 700);
            Card card_Knight = new Monster("SomeKnight", MonsterType.Knight, ElementType.Normal, 700);

            Assert.AreEqual(BattleResult.Lose, card_Dragon.Battle(card_Wizzard));
            Assert.AreEqual(BattleResult.Draw, card_Knight.Battle(card_Wizzard));
            Assert.AreEqual(BattleResult.Win, card_Knight.Battle(card_Dragon));
        }

        [TestMethod]
        public void Card_Battle_Natural_Enemy()
        {
            Card card_Dragon = new Monster("SomeDragon", MonsterType.Dragon, ElementType.Fire, 500);
            Card card_Goblin = new Monster("SomeGoblin", MonsterType.Goblin, ElementType.Normal, 300);
            Card card_Wizzard = new Monster("SomeWizzard", MonsterType.Wizzard, ElementType.Water, 700);
            Card card_Ork = new Monster("SomeOrk", MonsterType.Ork, ElementType.Normal, 300);
            Card card_FireElve = new Monster("SomeFireElve", MonsterType.FireElve, ElementType.Fire, 400);

            Assert.AreEqual(BattleResult.Lose, card_Goblin.Battle(card_Dragon));
            Assert.AreEqual(BattleResult.Lose, card_Ork.Battle(card_Wizzard));
            Assert.AreEqual(BattleResult.Lose, card_Dragon.Battle(card_FireElve));
        }

        [TestMethod]
        public void Card_Battle_Spell()
        {
            Card card_waterSpell = new Spell("SomeWaterSpell", ElementType.Water, 300);
            Card card_fireSpell = new Spell("SomeFireSpell", ElementType.Fire, 300);
            Card card_normalSpell = new Spell("SomeNormalSpell", ElementType.Normal, 300);

            Assert.AreEqual(BattleResult.Win, card_waterSpell.Battle(card_fireSpell));
            Assert.AreEqual(BattleResult.Win, card_fireSpell.Battle(card_normalSpell));
            Assert.AreEqual(BattleResult.Lose, card_waterSpell.Battle(card_normalSpell));
            Assert.AreEqual(BattleResult.Draw, card_normalSpell.Battle(card_normalSpell));
        }

        [TestMethod]
        public void Card_Battle_Mix()
        {
            Card card_Dragon = new Monster("SomeDragon", MonsterType.Dragon, ElementType.Fire, 500);
            Card card_Knight = new Monster("SomeKnight", MonsterType.Knight, ElementType.Normal, 700);
            Card card_Kraken = new Monster("SomeKraken", MonsterType.Kraken, ElementType.Water, 600);
            Card card_waterSpell = new Spell("SomeWaterSpell", ElementType.Water, 300);
            Card card_fireSpell = new Spell("SomeFireSpell", ElementType.Fire, 300);

            Assert.AreEqual(BattleResult.Win, card_waterSpell.Battle(card_Knight));
            Assert.AreEqual(BattleResult.Lose, card_fireSpell.Battle(card_Kraken));
            Assert.AreEqual(BattleResult.Win, card_waterSpell.Battle(card_Dragon));
            Assert.AreEqual(BattleResult.Lose, card_fireSpell.Battle(card_Dragon));
        }
    }
}