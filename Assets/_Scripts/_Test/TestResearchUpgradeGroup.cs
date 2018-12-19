namespace Test {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;

    public class TestResearchUpgradeGroup {

        #region VARIABLE

        private ClassType _classType = ClassType.NONE;

        private List<TestResearchUpgradeCard> _upgradeCards;

        private TestResearchUpgradeCard _previousSelectedCard;
        private List<TestResearchUpgradeCard> _previousCards;

        public TestResearchUpgradeCard PreviousCard {
            get { return this._previousSelectedCard; }
            set { this._previousSelectedCard = value; }
        }

        public TestResearchUpgradeGroup(ClassType classType, List<TestResearchUpgradeCard> cards) {
            this._upgradeCards = new List<TestResearchUpgradeCard>();
            this._previousCards = new List<TestResearchUpgradeCard>();

            this._previousSelectedCard = null;

            this._classType = classType;
            this._upgradeCards = cards;
        }

        public List<TestResearchUpgradeCard> GetUpgradeCards() {
            List<TestResearchUpgradeCard> temp = new List<TestResearchUpgradeCard>();

            if(this._upgradeCards.Count <= 3)
                return this._upgradeCards;
            else {
                List<int> numbers = new List<int>();

                do {
                    if(numbers.Count >= 3)
                        break;

                    int i = Random.Range(0, this._upgradeCards.Count);

                    if(numbers.Count != 0) {
                        if(!numbers.Contains(i))
                            numbers.Add(i);
                        else
                            continue;
                    } else {
                        numbers.Add(i);
                    }

                } while(true);

                for(int i = 0; i < 3; i++) {
                    temp.Add(this._upgradeCards[numbers[i]]);
                }

            }

            return temp;
        }

        public void DebugList() {
            foreach(TestResearchUpgradeCard card in this._upgradeCards) {
                card.DebugCard();
            }
        }

        #endregion

    }
}