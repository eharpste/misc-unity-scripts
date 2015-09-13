using UnityEngine;
using System.Collections;

/// <summary>
/// This is an implementation of ADSR curves that have linnear sub-elements.
/// It makes use of the Input.Axis paradigm for input control.
/// </summary>
public class ADSRCurve : ADSR {
    public float sustain = 1;
    public AnimationCurve attack;
    public AnimationCurve release;

    private float attackLength {
        get {
            return attack[attack.length-1].time - attack[0].time;
        }
    }

    private float releaseLength {
        get {
            return release[release.length-1].time - release[2].time;
        }
    }

    private float timeSince {
        get {
            return Time.time - transTime;
        }
    }


    public string axisName;
    public bool ignoreAxisValue;

    private bool momentum = false;

    public override float Value {
        get {
            switch (Current) {
                case Phase.Off:
                    return 0;
                case Phase.Attack:
                    if (ignoreAxisValue) {
                        return attack.Evaluate(timeSince) * sustain * (Input.GetAxis(axisName) > 0 ? 1 : -1);
                    }
                    else {
                        return attack.Evaluate(timeSince) * sustain * Input.GetAxis(axisName);
                    }
                case Phase.Sustain:
                    if (ignoreAxisValue) {
                        return sustain * (Input.GetAxis(axisName) > 0 ? 1 : -1);
                    }
                    else {
                        return sustain * Input.GetAxis(axisName);
                    }
                case Phase.Release:
                    return release.Evaluate(timeSince) * sustain * (momentum ? 1 : -1);
                default:
                    return 0;
            }
        }
    }

	// Use this for initialization
	void Start () {
        if(attack.Evaluate(0) != 0) {
            Debug.LogWarning("Attack curve must start at 0, overriding the key value.");
            Keyframe firstAttack = attack[0];
            attack.MoveKey(0,new Keyframe(0,0,firstAttack.inTangent,firstAttack.outTangent));
        }
        if (attack.Evaluate(attackLength) != 1) {
            Debug.LogWarning("Attack curve must end at 1, overriding the key value.");
            Keyframe lastAttack = attack[attack.length - 1];
            attack.MoveKey(attack.length - 1, new Keyframe(lastAttack.time, 1, lastAttack.inTangent, lastAttack.outTangent));
        }
        if (release.Evaluate(0) != 0) {
            Debug.LogWarning("Release curve must start at 1, overriding the key value.");
            Keyframe firstRelease = release[0];
            release.MoveKey(0, new Keyframe(0, 1, firstRelease.inTangent, firstRelease.outTangent));
        }
        if (release.Evaluate(releaseLength) != 0) {
            Debug.LogWarning("Release curve must end at 0, overriding the key value.");
            Keyframe lastRelease = release[release.length - 1];
            release.MoveKey(release.length - 1, new Keyframe(lastRelease.time, 0, lastRelease.inTangent, lastRelease.outTangent));
        }
	}

	// Update is called once per frame
	void Update () {
        if(Input.GetAxis(axisName) != 0) {
            momentum = Value > 0;
        }
	    switch(Current) {
            case Phase.Off:
                if (Input.GetAxis(axisName) != 0) {
                    Current = Phase.Attack;
                    goto case Phase.Attack;
                }
                break;
            case Phase.Attack:
                if (Input.GetAxis(axisName) == 0) {
                    Current = Phase.Release;
                    goto case Phase.Release;
                }
                if(transTime + attackLength <= Time.time) {
                    Current = Phase.Sustain;
                    goto case Phase.Sustain;
                }
                break;
            case Phase.Sustain:
                if (Input.GetAxis(axisName) == 0) {
                    Current = Phase.Release;
                    goto case Phase.Release;
                }
                break;
            case Phase.Release:
                if (Input.GetAxis(axisName) != 0) {
                    Current = Phase.Sustain;
                    goto case Phase.Sustain;
                }
                else if (transTime + releaseLength <= Time.time) {
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
