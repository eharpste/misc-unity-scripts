using UnityEngine;
using System.Collections;

public class ScreenShotUtil : MonoBehaviour {
    public KeyCombo hotKeys;

    public string screenShortDir;
    public string filename;
    public int screenShotScale = 1;

    private bool uiUp;
    private bool screenShotting;
    private Rect windowRect = new Rect(0, 0, 150, 100);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(hotKeys.GetKeyCombo()) {
            uiUp = !uiUp;
        }
	}

    void OnGUI() {
        if (uiUp && !screenShotting) {
            windowRect = GUILayout.Window(0, windowRect, SSWindowFunction, "Screen Shot Util");
        }
    }

    void SSWindowFunction(int id) {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("ScreenShot Name:",GUILayout.Width(50));
        filename = GUILayout.TextField(filename);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("ScreenShot Scale:", GUILayout.Width(50));
        string test = screenShotScale + "";
        string temp = GUILayout.TextField(test);
        if (test != temp) {
            int tempInt = 0;
            if (int.TryParse(temp, out tempInt)) {
                screenShotScale = tempInt;
            }
        }
        GUILayout.EndHorizontal();
        
        if (GUILayout.Button("Take ScreenShot")) {
            StartCoroutine(TakeScreenShot());
        }
        
        GUILayout.EndVertical();
    }

    IEnumerator TakeScreenShot() {
        screenShotting = true;
        yield return new WaitForEndOfFrame();
        Application.CaptureScreenshot(screenShortDir + "/" + filename, screenShotScale);
        yield return new WaitForEndOfFrame();
        screenShotting = false;
        yield break;
    }
}
