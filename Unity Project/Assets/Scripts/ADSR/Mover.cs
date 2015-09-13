using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Mover : MonoBehaviour {

    ADSR adsr;

	// Use this for initialization
	void Start () {
        adsr = GetComponent(typeof(ADSR)) as ADSR;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        rigidbody2D.velocity = transform.right * adsr.Value;
	}
}
