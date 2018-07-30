using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Testing : MonoBehaviour {

    [SerializeField]
    protected float _endOfAttack = 0.8f;
    protected Animator _animator = null;
    private AnimationClip _animClip = null;
    private AnimationEvent _animEvent = null;

    // Use this for initialization
    void Start () {
        this._animator = this.GetComponent<Animator>();
        if(this._animator == null)
            throw new ArgumentNullException("Unit Animator Is Missing");

        this.SetupAttackEventAnimation();
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Space)) {
            this._animator.Play("Attack");
        }
	}

    public void Attack() {
        Debug.Log("Attacking");
    }

    protected virtual void SetupAttackEventAnimation() {
        this._animEvent = new AnimationEvent();
        this._animClip = new AnimationClip();

        if(this._endOfAttack <= 0.0f)
            throw new ArgumentException("End of Attack Animation Timer Needs to be Set, Cannot Be 0");

        this._animEvent.time = this._endOfAttack;
        this._animEvent.functionName = "Attack";

        foreach(AnimationClip clip in this._animator.runtimeAnimatorController.animationClips) {
            if(clip.name.Contains("Attack")) {
                this._animClip = clip;
                break;
            }
        }

        foreach(AnimationEvent evt in this._animClip.events) {
            if(evt.Equals(this._animEvent)) {
                Debug.Log("Found A SImilar Event - " + this._animEvent.ToString());
            }
        }

        this._animClip.AddEvent(this._animEvent);
    }
}
