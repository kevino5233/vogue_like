using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log(this.transform.localScale);
		float a = this.transform.localScale.x / 2;
		float b = this.transform.localScale.z / 2;
		Debug.Log(a + " " + b);
		float c = Mathf.Sqrt((a * a + b * b) / 2);
		c *= 2 * Mathf.PI;
		Debug.Log(c);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
