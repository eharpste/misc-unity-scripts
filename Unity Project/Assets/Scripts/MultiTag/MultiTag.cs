using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode()]
public class MultiTag : MonoBehaviour {

    #region ================ Static Properties ======================

    private static Dictionary<string, List<GameObject>> dict = new Dictionary<string,List<GameObject>>();

    private static bool AddToMap(string tag, GameObject go) {
        if (!dict.ContainsKey(tag)) {
            dict[tag] = new List<GameObject>();
        }
        if (!dict[tag].Contains(go)) {
            dict[tag].Add(go);
            return true;
        }
        return false;
    }

    private static bool RemoveFromMap(string tag, GameObject go) {
        if (dict.ContainsKey(tag)) {
            if (dict[tag].Remove(go)) {
                if (dict[tag].Count == 0) {
                    dict.Remove(tag);
                }
                return true;
            }
        }
        return false;
    }

    public static GameObject FindGameObjectWithTag(string tag) {
        return FindAGameObjectWithTag(tag);
    }

    public static GameObject[] FindGameObjectsWithTag(string tag) {
        return FindAllGameObjectsWithTag(tag);
    }

    public static GameObject FindAGameObjectWithTag(string tag) {
        if (dict.ContainsKey(tag)) {
            return dict[tag][0];
        }
        return null;
    }

    public static GameObject[] FindAllGameObjectsWithTag(string tag) {
        if(dict.ContainsKey(tag)) {
            return dict[tag].ToArray();
        }
        return null;
    }

    public static string[] SortedTagOptions() {
        List<string> keys = new List<string>();
        foreach (string k in dict.Keys) {
            keys.Add(k);
        }
        keys.Sort();
        return keys.ToArray();
    }


    #endregion ======================================================

    [SerializeField()]
    public List<string> multitags = new List<string>();

    public void AddMultiTag(string tag) {
        if (!multitags.Contains(tag)) {
            AddToMap(tag, this.gameObject);
            multitags.Add(tag);
        }
    }

    public void AddMultiTags(string[] tags) {
        foreach (string s in tags) {
            if (!multitags.Contains(s)) {
                AddToMap(s, this.gameObject);
                multitags.Add(s);
            }
        }
    }

    public void RemoveMultiTag(string tag) {
        if (multitags.Contains(tag) && dict.ContainsKey(tag)) {
            RemoveFromMap(tag, this.gameObject);
            multitags.Remove(tag);
        }
    }

    public void RemoveMultiTags(string[] tags) {
        foreach (string s in tags) {
            multitags.Remove(s);
            RemoveFromMap(s,this.gameObject);
        }
    }

    public void ClearMultiTags() {
        foreach (string s in multitags) {
            RemoveFromMap(s, this.gameObject);
        }
        multitags.Clear();
    }

    void Awake() {
        
    }


    void OnEnable() {
        //this.multitags = this.multitags.Distinct().ToList<string>();
        foreach (string s in multitags) {
            AddToMap(s, this.gameObject);
        }
    }

    void OnDestroy() {
        ClearMultiTags();
    }
}
