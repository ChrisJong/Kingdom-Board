namespace Unit { 

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public struct UnitData
    {
        private enum attackType
        {
            NONE = 0,
            PHYSICAL,
            MAGIC
        }

        private enum attackRange
        {
            NONE = 0,
            MELEE,
            RANGE
        }

        private attackType curType;
        private attackType res;
        private attackType weak;

        private float resAmount;
        private float weakAmount;

        private float curDamage;
        private float minDamage;
        private float maxDamage;

        private float curHealth;
        private float minHealth;
        private float maxHealth;

        private float curMobility;
        private float minMobility;
        private float maxMobility;

        private float unitCost;

        private float walkSpeed;

        public void SetDamage(int damage, int minDamage, int maxDamage)
        {
            this.curDamage = (float)damage;
            this.minDamage = (float)minDamage;
            this.maxDamage = (float)maxDamage;
        }

        public void SetHealth(int health, int minHealth, int maxHealth)
        {
            this.curHealth = (float)health;
            this.minHealth = (float)minHealth;
            this.maxHealth = (float)maxHealth;
        }

        public void Reset()
        {

        }

        public float CurHealth
        {
            set { this.curHealth = value; }
            get { return this.curHealth; }
        }

        public float MinHealth
        {
            set { this.minHealth = value; }
            get { return this.MinHealth; }
        }

        public float MaxHealth
        {
            set { this.maxHealth = value; }
            get { return this.maxHealth; }
        }
    }
}