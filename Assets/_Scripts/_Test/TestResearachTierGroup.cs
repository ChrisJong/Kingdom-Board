namespace Test {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    public class TestResearachTierGroup {

        #region VARIABLE
        [SerializeField] private int _currentTier = 0;
        [SerializeField] private int _previousTier = 0;

        private Dictionary<int, List<TestResearchCard>> _unitCards;

        public int TierLevel {
            get { return this._currentTier; }
        }

        #endregion

        #region CLASS
        public TestResearachTierGroup(List<TestResearchCard> cards, int tierLevel = 0) {

            this._currentTier = tierLevel;
            this._unitCards = new Dictionary<int, List<TestResearchCard>>();

            // loop through the max tier count for each class.
            for(int i = 1; i < 3; i++) {

                List<TestResearchCard> tierCards = new List<TestResearchCard>();

                for(int j = 0; j < cards.Count; j++) {
                    if(cards[j].TierLevel > i)
                        break;

                    if(cards[j].TierLevel == i)
                        tierCards.Add(cards[j]);
                }

                this._unitCards.Add(i, tierCards);

            }
        }

        public List<TestResearchCard> GetLockedCards() {
            List<TestResearchCard> temp = new List<TestResearchCard>();

            Debug.Log("Tier Count: " + this._unitCards.Count.ToString());

            // Check the currentTierLevel for cards.
            foreach(TestResearchCard card in this._unitCards[this._currentTier]) {
                if(!card.Unlocked)
                    temp.Add(card);
            }

            // If the currentTierLevel has cards then just return the array, and don't go any further.
            // Else if the currentTierLevel is empty move onto the next tier.
            if(temp.Count != 0)
                return temp;
            else
                this._currentTier++;

            // Check to see if our currentTierLevel isnt over the max tierLEvel. then cap it at max.
            if(this._currentTier >= 3) {
                this._currentTier = 2;
            } else {
                this._previousTier = this._currentTier;
            }

            // Search the next tier for cards.
            foreach(TestResearchCard card in this._unitCards[this._currentTier]) {
                if(!card.Unlocked)
                    temp.Add(card);
            }

            return temp; // it can return a empty list of cards which tellse us there isnt any cards left.
        }

        public bool AnyCardsToUnlock() {
            foreach(int tier in this._unitCards.Keys) {
                foreach(TestResearchCard card in this._unitCards[tier]) {
                    if(!card.Unlocked)
                        return true; // there are still cards to unlock.
                }
            }

            return false; // there are no more cards to unlock.
        }

        public bool IsBaseTierUnlocked() {
            foreach(TestResearchCard card in this._unitCards[1]) {
                if(card.Unlocked)
                    return true;
            }

            return false;
        }

        public void DebugList() {
            foreach(int tierLevel in this._unitCards.Keys) {
                List<TestResearchCard> cards = this._unitCards[tierLevel];

                foreach(TestResearchCard card in cards) {
                    Debug.Log("Tier: " + tierLevel.ToString() + " - " + card.UnitType.ToString());
                }
            }
        }
        #endregion
    }
}