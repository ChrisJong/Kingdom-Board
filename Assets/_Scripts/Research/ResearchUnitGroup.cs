namespace KingdomBoard.Research {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    public class ResearchUnitGroup {

        #region VARIABLE

        private int _currentTier = 0; // 0 Doesn't belong to any tier class.
        private int _preivousTier = 0;

        private Dictionary<int, List<ResearchUnitCard>> _cards;

        public int CurrentTier {
            get { return this._currentTier; }
        }

        public int PreviousTier {
            get { return this._preivousTier; }
        }

        #endregion

        #region CLASS

        public ResearchUnitGroup(List<ResearchUnitCard> cards, int tierLevel = 0) {
            this._currentTier = tierLevel;
            this._preivousTier = tierLevel;

            this._cards = new Dictionary<int, List<ResearchUnitCard>>();

            // Loop through till the highest tier count is reached which is 3.
            for(int i = 1; i < 3; i++) {

                List<ResearchUnitCard> temp = new List<ResearchUnitCard>();

                for(int j = 0; j < cards.Count; j++) {

                    if(cards[j].ClassTier > i)
                        continue;

                    if(cards[j].ClassTier == i)
                        temp.Add(cards[j]);

                }

                this._cards.Add(i, temp);
            }
        }

        public List<ResearchUnitCard> GetCards() {
            List<ResearchUnitCard> temp = new List<ResearchUnitCard>();

            foreach(ResearchUnitCard card in this._cards[this._currentTier]) {
                if(!card.Unlocked)
                    temp.Add(card);
            }

            if(temp.Count != 0)
                return temp;
            else
                this._currentTier++;

            if(this._currentTier >= 3)
                this._currentTier = 2;
            else
                this._preivousTier = this._currentTier;

            foreach(ResearchUnitCard card in this._cards[this._currentTier]) {
                if(!card.Unlocked)
                    temp.Add(card);
            }

            return temp;
        }

        public bool AnyCardsToUnlock() {

            foreach(int tier in this._cards.Keys) {
                foreach(ResearchUnitCard card in this._cards[tier]) {
                    if(!card.Unlocked)
                        return true;
                }
            }

            return false;
        }

        public bool IsBaseTierUnlocked() {

            foreach(ResearchUnitCard card in this._cards[1]) {
                if(card.Unlocked)
                    return true;
            }

            return false;
        }
        #endregion
    }
}
