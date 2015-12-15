using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour {

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float dx = 0.0f;
		float dy = 0.0f;
		if(Input.GetKey(KeyCode.W)){
			dy += 0.4f;
			float gap = 90 - transform.rotation.eulerAngles.z;
			transform.Rotate(0,0,gap);
		}
		if(Input.GetKey(KeyCode.S)){
			dy -= 0.4f;
			float gap = -90 - transform.rotation.eulerAngles.z;
			transform.Rotate(0,0,gap);
		}
		if(Input.GetKey(KeyCode.A)){
			dx -= 0.4f;
			float gap = 180 - transform.rotation.eulerAngles.z;
			transform.Rotate(0,0,gap);
		}
		if(Input.GetKey(KeyCode.D)){
			dx += 0.4f;
			float gap = 0 - transform.rotation.eulerAngles.z;
			transform.Rotate(0,0,gap);
		}

		if(Input.GetKey(KeyCode.W) && Input.GetKey (KeyCode.A)){
			float gap = 135 - transform.rotation.eulerAngles.z;
			transform.Rotate(0,0,gap);
		}
		if(Input.GetKey(KeyCode.W) && Input.GetKey (KeyCode.D)){
			float gap = 45 - transform.rotation.eulerAngles.z;
			transform.Rotate(0,0,gap);
		}
		if(Input.GetKey(KeyCode.S) && Input.GetKey (KeyCode.D)){
			float gap = -45 - transform.rotation.eulerAngles.z;
			transform.Rotate(0,0,gap);
		}
		if(Input.GetKey(KeyCode.S) && Input.GetKey (KeyCode.A)){
			float gap = -135 - transform.rotation.eulerAngles.z;
			transform.Rotate(0,0,gap);
		}

		if (transform.rotation.eulerAngles.z > 180) {
			transform.Rotate(0,0,-360);
		}
		if (transform.rotation.eulerAngles.z < -180) {
			transform.Rotate (0,0,360);
		}
		float speed = Mathf.Sqrt( (dx*dx) + (dy*dy) );

		Vector3 pos = new Vector3(transform.position.x + dx, transform.position.y + dy, speed);

		transform.position = pos;

	}
}
