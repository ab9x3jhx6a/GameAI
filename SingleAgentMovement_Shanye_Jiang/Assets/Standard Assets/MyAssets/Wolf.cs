using UnityEngine;
using System.Collections;

public class Wolf : MonoBehaviour {

	public float max_speed;
	private float orientation;
	private float rotate;
	private float current_speed; 
	private float speedx;
	private float speedy;

	//variable that tracks duration of movements for freewondering
	private float duration;
	//targets
	private GameObject red;
	private GameObject hunter;
	private Vector3 hunter_pos;
	private Vector3 red_pos;
	//movements variables
	private bool pursue;
	private bool alert;
	private bool caught;
	private bool prey;
	private bool breaking;

	// Use this for initialization
	void Start () {
		max_speed = 0.2f;
		current_speed = 0;
		speedx = 0;
		speedy = 0;
		orientation = 180;
		rotate = 0;

		//time trackers
		duration = 0;

		//movement variables
		pursue = false;
		alert = false;
		caught = false;
		prey = false;

		//adjust the sprite
		transform.Rotate (0, 0, 180);
	}
	
	// Update is called once per frame
	void Update () {
		current_speed = Mathf.Sqrt(Mathf.Pow (speedx,2)+Mathf.Pow (speedy,2));

		hunter = GameObject.FindWithTag ("Hunter");
		red = GameObject.FindWithTag ("Red");


		if (caught) {
			duration += Time.deltaTime;
			if(duration > 3){
				caught = false;
				breaking = true;
				duration = 0;
			}
		} else if (prey) {
			duration += Time.deltaTime;
			if(duration > 5f){
				prey = false;
				duration = 0;
			}
		} else {
			DetectHunter (hunter);
			DetectRed (red);
			//If nothing in sight, free wondering
			if (!pursue && !alert) {
				FreeWonder ();
				DetectBound ();
			} else if (pursue) {//pursue > alert
				Pursue ();
			} else if (alert) {
				Escape ();
			}
		}
	}

	void OnGUI() {
		if (!pursue && ! alert) {
			GUI.Label (new Rect (15, 60, 200, 20), "Wolf: Free wondering");
		}else if (caught) {
			GUI.Label (new Rect (15, 60, 200, 20), "Wolf: Caught");
		} else if (prey) {
			GUI.Label (new Rect (15, 60, 200, 20), "Wolf: Caught Red");
		} else if (pursue) {
			GUI.Label (new Rect (15, 60, 200, 20), "Wolf: Pursue Red");
		} else if (alert) {
			GUI.Label (new Rect (15, 60, 200, 20), "Wolf: Escaping");
		} 
	}

	void FreeWonder() {
		duration += Time.deltaTime;

		if (duration > 1) {
			float r = Random.Range (-45, 45);
			transform.Rotate (0, 0, r);
			orientation += r;

			duration = 0;
		}

		movement ();

	}

	void movement() {
		float angle = orientation * (Mathf.PI / 180);
		
		if ( (speedx > max_speed * Mathf.Cos (angle)) && speedy < max_speed*Mathf.Sin(angle) ) {
			speedx -= 0.005f;
			speedy += 0.005f;
		}
		if ( (speedy > max_speed * Mathf.Sin (angle)) && speedx < max_speed*Mathf.Cos(angle) ) {
			speedy -= 0.005f;
			speedx += 0.005f;
		}
		
		if ( (speedy > max_speed * Mathf.Sin (angle)) && speedx > max_speed*Mathf.Cos(angle) ) {
			speedy -= 0.005f;
			speedx -= 0.005f;
		}
		
		if ( (speedy < max_speed * Mathf.Sin (angle)) && speedx < max_speed*Mathf.Cos(angle) ) {
			speedy += 0.005f;
			speedx += 0.005f;
		}
		
		Vector3 newpos = new Vector3 (transform.position.x + speedx, transform.position.y + speedy, 0);
		transform.position = newpos;
	}

	//Detect if hunter is in range
	void DetectHunter(GameObject hunter){
		Vector3 pos = hunter.transform.position;
		hunter_pos = pos;
		float x = pos.x;
		float y = pos.y;
		float distance = Mathf.Sqrt ((transform.position.x - x) * (transform.position.x - x) + (transform.position.y - y) * (transform.position.y - y));
		
		if (distance < 20) {
			max_speed = 0.2f;
			if(distance < 5){
				max_speed = 0.05f;
			}
			if (distance < 3 && !breaking) {
				caught = true;
			}
			if (distance > 5){
				breaking = false;
			}
			alert = true;
		} else {
			max_speed = 0.15f;
			alert = false;
		}
	}

	//Detect if red is in range
	void DetectRed(GameObject red){
		Vector3 pos = red.transform.position;
		red_pos = pos;
		float x = pos.x;
		float y = pos.y;
		float distance = Mathf.Sqrt ((transform.position.x - x) * (transform.position.x - x) + (transform.position.y - y) * (transform.position.y - y));
		
		if (distance < 20) {
			max_speed = 0.2f;
			if(distance < 3){
				max_speed -= 0.01f;
				//max_speed = 0.05f;
			}
			if (distance < 1) {
				prey = true;
			}
			pursue = true;
		} else {
			max_speed = 0.2f;
			pursue = false;
		}
	}

	//detect the bound of map and turn around
	bool DetectBound(){

		float x = transform.position.x;
		float y = transform.position.y;
		float angle = orientation * (Mathf.PI / 180);
		if (x + 5*Mathf.Cos (angle) > 88f || x + 5*Mathf.Cos (angle) < -18f) {
			transform.Rotate(0,0,180-2*orientation);
			orientation = 180 - orientation;
			return true;
		}
		if (y + 5*Mathf.Sin (angle) > 80f || y + 5*Mathf.Sin (angle) < -24f) {
			transform.Rotate(0,0,-2*orientation);
			orientation = -orientation;
			return true;
		}
		return false;
	}

	void Pursue() {
		float x = red_pos.x;
		float y = red_pos.y;
		
		//prediction of future position
		float angle = red.transform.rotation.eulerAngles.z / 180 * Mathf.PI;
		
		
		float px = x + (Mathf.Cos (angle) * 15 * 0.18f);
		float py = y + (Mathf.Sin (angle) * 15 * 0.18f);
		
		Vector3 start = new Vector3 (transform.position.x, transform.position.y, 0);
		Vector3 end = new Vector3 (px, py, 0);
		Debug.DrawLine (start, end, Color.yellow, 1/24);
		
		float gap = orientation;
		/*if (x == transform.position.x) {
			if (y > transform.position.y) {
				orientation = 90;
				
			}
			if (y <= transform.position.y) {
				orientation = -90;
			}
		}
		else {*/
			float chase_angle = Mathf.Atan ((py - transform.position.y) / (px - transform.position.x));
			if(px >= transform.position.x){
				orientation = chase_angle * 180 / Mathf.PI;
			}
			if(px < transform.position.x) {
				orientation = (chase_angle * 180 / Mathf.PI) + 180;
			}
		Debug.Log (orientation);
		//}
		gap = orientation - gap;
		
		transform.Rotate (0, 0, gap);
		
		movement ();
	}

	void Escape() {
		float x = hunter_pos.x;
		float y = hunter_pos.y;

		duration += Time.deltaTime;

		if (duration > 1.5f) {
			rotate = Random.Range (-30, 30);
			duration = 0;
		}

		float gap = orientation;
		if (x == transform.position.x) {
			if (y > transform.position.y) {
				orientation = -90;
			}
			if (y <= transform.position.y) {
				orientation = 90;
			}
		}
		else {
			float chase_angle = Mathf.Atan ((y - transform.position.y) / (x - transform.position.x));
			if(x > transform.position.x){
				orientation = (chase_angle * 180 / Mathf.PI) + 180;
			}
			else {
				orientation = (chase_angle * 180 / Mathf.PI);
			}
		}
		orientation += rotate;
		gap = orientation - gap;
		
		transform.Rotate (0, 0, gap);

		if (true) {//escape corner
			float tx = transform.position.x;
			float ty = transform.position.y;
			float angle = orientation * (Mathf.PI / 180);
			if (tx + 5*Mathf.Cos (angle) > 88f || tx + 5*Mathf.Cos (angle) < -18f) {
				transform.Rotate(0,0,180-2*orientation);
				orientation = 180 - orientation;
			}
			if (ty + 5*Mathf.Sin (angle) > 80f || ty + 5*Mathf.Sin (angle) < -24f) {
				transform.Rotate(0,0,-2*orientation);
				orientation = -orientation;
			}
		}

		movement ();
	}

}
