using UnityEngine;
using System.Collections;

/// <summary>
/// This is an implementation of ADSR curves that have linnear sub-elements.
/// It makes use of the Input.Axis paradigm for input control.
/// </summary>
public class ADSRAxis : ADSR {
    public float attack;
    public float attackLength;
    public float decayLength;
    public float sustain;
    public float releaseLength;
    
    public string axisName;
    public bool useAxisValue = false;
    private float momentum = 0;
    public override float Value {
        get {
            switch (Current) {
                case Phase.Off:
                    return 0;
                case Phase.Attack:
                    if (!useAxisValue)
                        return Mathf.Lerp(0, attack, (Time.time - transTime) / attackLength) * (Input.GetAxis(axisName) > 0 ? 1 : -1);
                    else
                        return Mathf.Lerp(0, attack, (Time.time - transTime) / attackLength) * Input.GetAxis(axisName);
                case Phase.Decay:
                    if (!useAxisValue)
                        return Mathf.Lerp(attack, sustain, (Time.time - transTime) / decayLength) * (Input.GetAxis(axisName) > 0 ? 1 : -1);
                    else
                        return Mathf.Lerp(attack, sustain, (Time.time - transTime) / decayLength) * Input.GetAxis(axisName);
                case Phase.Sustain:
                    if (!useAxisValue)
                        return sustain * (Input.GetAxis(axisName) > 0 ? 1 : -1);
                    else
                        return sustain * Input.GetAxis(axisName);
                case Phase.Release:
                    return Mathf.Lerp(sustain, 0, (Time.time - transTime) / releaseLength) * momentum;
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
	    switch(Current) {
            case Phase.Off:
                if (Input.GetAxis(axisName) != 0) {
                    Current = Phase.Attack;
                    goto case Phase.Attack;
                }
                break;
            case Phase.Attack:
                if (Input.GetAxis(axisName) == 0) {
                    Current = Phase.Off;
                    goto case Phase.Release;
                }
                else {
                    momentum = Input.GetAxis(axisName) < 0 ? -1 : 1;
                }
                if(transTime + attackLength <= Time.time) {
                    Current = Phase.Decay;
                    goto case Phase.Decay;
                }
                break;
            case Phase.Decay:
                if (Input.GetAxis(axisName) == 0) {
                    Current = Phase.Release;
                    goto case Phase.Release;
                }
                else {
                    momentum = Input.GetAxis(axisName) < 0 ? -1 : 1;
                }
                if (transTime + decayLength <= Time.time) {
                    Current = Phase.Sustain;
                    goto case Phase.Sustain;
                }
                break;
            case Phase.Sustain:
                if (Input.GetAxis(axisName) == 0) {
                    Current = Phase.Release;
                    goto case Phase.Release;
                }
                else {
                    momentum = Input.GetAxis(axisName) < 0 ? -1 : 1;
                }
                break;
            case Phase.Release:
                if (Input.GetAxis(axisName) != 0) {
                    Current = Phase.Sustain;
                    goto case Phase.Sustain;
                }
                else if (transTime + releaseLength <= Time.time) {
                    Current = Phase.Off;
                    momentum = 0;
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
