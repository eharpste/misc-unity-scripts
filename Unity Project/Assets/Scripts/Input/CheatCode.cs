using UnityEngine;
using System.Collections;

public class CheatCode : MonoBehaviour {

    public KeyCode[] code;
    public float timeOutAfter = 0;
    public int keydex = 0;
    private float lastKeyTime = 0;

    private System.Action callback;

    public void SetCallback(System.Action callback) {
        this.callback = callback;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKeyDown) {
            if (Input.GetKeyDown(code[keydex])) {
                keydex++;
                if (keydex == code.Length) {
                    FireCode();
                }
                lastKeyTime = Time.time;
            }
            else {
                keydex = 0;
            }
        }

        if (timeOutAfter > 0 && Time.time - lastKeyTime > timeOutAfter) {
            keydex = 0;
        }
	}

    void FireCode() {
        keydex = 0;
        if (callback != null) {
            callback();
        }
    }
}
