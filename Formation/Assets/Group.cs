using UnityEngine;
using System.Collections;

public class Group : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 all = new Vector3 (0, 0, 0);
		int num = 0;
		foreach (Transform child in transform) {
			num++;
			all += child.position;
		}

		Vector3 avg = all / num;
		transform.position = avg;
	}
}
