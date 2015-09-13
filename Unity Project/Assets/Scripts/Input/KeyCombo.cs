using UnityEngine;
using System.Collections;

[System.Serializable]
public class KeyCombo {
    public KeyCode[] keys;

    public bool GetKeyCombo() {
        foreach (KeyCode k in keys) {
            if (!Input.GetKey(k)) {
                return false;
            }
        }
        return true;
    }

    public bool GetKeyComboDown() {
        foreach (KeyCode k in keys) {
            if (!Input.GetKeyDown(k)) {
                return false;
            }
        }
        return true;
    }

    public bool GetKeyComboUp() {
        foreach (KeyCode k in keys) {
            if (!Input.GetKeyUp(k)) {
                return false;
            }
        }
        return true;
    }
}
