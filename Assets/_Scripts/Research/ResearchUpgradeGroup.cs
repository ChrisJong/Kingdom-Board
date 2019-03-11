namespace KingdomBoard.Research {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;

    public class ResearchUpgradeGroup {

        #region VARIABLE

        private UnitClassType _classType = UnitClassType.NONE;

        private List<ResearchUpgradeCard> _cards;
        private List<ResearchUpgradeCard> _previousCards;

        private ResearchUpgradeCard _previousSelectedCard;

        public ResearchUpgradeCard PreviousSelectedCard {
            get { return this._previousSelectedCard; }
            set { this._previousSelectedCard = value; }
        }

        #endregion

        public ResearchUpgradeGroup(UnitClassType classType, List<ResearchUpgradeCard> cards) {
            this._cards = new List<ResearchUpgradeCard>();
            this._previousCards = new List<ResearchUpgradeCard>();
            this._previousSelectedCard = null;

            this._classType = classType;
            this._cards = cards;
        }

        public List<ResearchUpgradeCard> GetCards() {
            List<ResearchUpgradeCard> temp = new List<ResearchUpgradeCard>();

            if(this._cards.Count <= 3)
                return this._cards;
            else {
                List<int> numbers = new List<int>();

                do {

                    if(numbers.Count >= 3)
                        break;

                    int i = Random.Range(0, this._cards.Count);

                    if(numbers.Count != 0) {

                        if(!numbers.Contains(i))
                            numbers.Add(i);
                        else
                            continue;

                    } else {
                        numbers.Add(i);
                    }

                } while(true);

                for(int i = 0; i < 3; i++)
                    temp.Add(this._cards[numbers[i]]);

            }

            return temp;
        }
    }
}