﻿namespace KingdomBoard.Player {

    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;

    using Enum;
    using Manager;

    public class PlayerSound : MonoBehaviour {

        #region VARIABLE
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioClip _currentClip = null;
        #endregion

        #region CLASS
        public void Init(AudioSource source) {
            this._audioSource = source;
            this._audioSource.playOnAwake = false;
        }

        public void PlayClip() {
            if(this._currentClip == null)
                throw new System.NullReferenceException("Null Exception: No Audio Clip Current Loaded");
            else {
                if(this._audioSource.isPlaying)
                    this._audioSource.Stop();

                if(this._audioSource.clip != this._currentClip)
                    this._audioSource.clip = this._currentClip;

                this._audioSource.Play();
            }
        }

        public void PlayAttackPhase() {

            this._currentClip = SoundManager.instance.attackPhase;

            this.PlayClip();
        }

        public void PlayDefencePhase() {

            this._currentClip = SoundManager.instance.defencePhase;

            this.PlayClip();
        }

        public void PlayOpenBook() {

            List<AudioClip> temp = SoundManager.instance.bookOpen;
            int index = Random.Range(0, (temp.Count - 1));
            this._currentClip = temp[index];

            this.PlayClip();
        }

        public void PlayResearchCardTurn() {

            this._currentClip = SoundManager.instance.cardTurn;

            this.PlayClip();
        } 
        #endregion
    }
}