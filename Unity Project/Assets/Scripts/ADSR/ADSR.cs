using UnityEngine;
using System.Collections;


public abstract class ADSR : MonoBehaviour {
    public enum Phase {
        Off, Attack, Decay, Sustain, Release
    }

    public Phase Current {
        get {
            return current;
        }
        set {
            current = value;
            transTime = Time.time;
            dbgOnce = false;
        }
    }

    public abstract float Value {
        get;
    }

    protected Phase current;
    protected float transTime = 0f;

    public bool debug = false;
    protected bool dbgOnce = false;

}
