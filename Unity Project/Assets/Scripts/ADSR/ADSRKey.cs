using UnityEngine;
using System.Collections;

/// <summary>
/// This is an implementation of ADSR curves that have linnear sub-elements
/// It also relies on KeyCodes rather than other input mechanisms.
/// </summary>
public class ADSRKey : ADSR {
    public float attack;
    public float attackLength;
    public float decayLength;
    public float sustain;
    public float releaseLength;

    public KeyCode positiveKey;
    public KeyCode negativeKey;

    private KeyCode lastDown = KeyCode.None;
    private bool momentum = false;

    public override float Value {
        get {
            switch (Current) {
                case Phase.Off:
                    return 0;
                case Phase.Attack:
                    return Mathf.Lerp(0, attack, (Time.time-transTime) / attackLength) * (negativeKey != KeyCode.None && lastDown == negativeKey ? -1 : 1);
                case Phase.Decay:
                    return Mathf.Lerp(attack, sustain, (Time.time-transTime) / decayLength) * (negativeKey != KeyCode.None && lastDown == negativeKey ? -1 : 1);
                case Phase.Sustain:
                    return sustain * (negativeKey != KeyCode.None && lastDown == negativeKey ? -1 : 1);
                case Phase.Release:
                    return Mathf.Lerp(sustain, 0, (Time.time-transTime) / releaseLength) * (momentum ? 1 : -1);
                default:
                    return 0;
            }
        }
    }

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
        momentum = Value > 0;
        if (lastDown == KeyCode.None) {
            if (Input.GetKeyDown(positiveKey))
                lastDown = positiveKey;
            else if (Input.GetKeyDown(negativeKey))
                lastDown = negativeKey;
        }  
        else if (lastDown == positiveKey) {
            if(Input.GetKeyDown(negativeKey))
                lastDown = negativeKey;
            else if (Input.GetKeyUp(positiveKey)) {
                if(Input.GetKey(negativeKey))
                    lastDown = negativeKey;
                else
                    lastDown = KeyCode.None;
            }
        }
        else if (lastDown == negativeKey) {
            if(Input.GetKeyDown(positiveKey))
                lastDown = positiveKey;
            else if (Input.GetKeyUp(negativeKey)) {
                if (Input.GetKey(positiveKey))
                    lastDown = positiveKey;
                else
                    lastDown = KeyCode.None;
            }
        }

	    switch(Current) {
            case Phase.Off:
                if (lastDown != KeyCode.None) {
                    Current = Phase.Attack;
                    goto case Phase.Attack;
                }
                break;
            case Phase.Attack:
                if(transTime + attackLength < Time.time) {
                    Current = Phase.Decay;                   
                    goto case Phase.Decay;
                }
                
                break;
            case Phase.Decay:
                if (lastDown == KeyCode.None) {
                    Current = Phase.Release;
                    goto case Phase.Release;
                }
                else if (transTime + decayLength < Time.time) {
                    Current = Phase.Sustain;
                    goto case Phase.Sustain;
                }
                break;
            case Phase.Sustain:
                if (!(positiveKey != KeyCode.None && Input.GetKey(positiveKey)
                || negativeKey != KeyCode.None && Input.GetKey(negativeKey))) {
                    Current = Phase.Release;
                    goto case Phase.Release;
                }
                break;
            case Phase.Release:
                if (lastDown != KeyCode.None) {
                    Current = Phase.Sustain;
                    goto case Phase.Sustain;
                }
                if (transTime + releaseLength < Time.time) {
                    Current = Phase.Off;
                    goto case Phase.Off;
                }
                break;
        }

        if (debug) {
            if(!dbgOnce)
                Debug.Log("ADSR state: " + Current + " mom:" + momentum + " val: " + Value);
            if (Value == 0 || Value == sustain || Value == -sustain)
                dbgOnce = true;
            else
                dbgOnce = false;
        }
        
	}
}
