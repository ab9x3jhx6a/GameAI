using UnityEngine;
using System.Collections;

public class Hunter : MonoBehaviour {
	
	public float max_speed;
	private float current_speed; 
	private float currentx;
	private float currenty;
	private float speedx;
	private float speedy;
	private float orientation;
	private float rotation;
	//variable that tracks duration of movements for freewondering
	private float duration;
	//how far it will look
	private bool free;
	//variables for pursue
	private GameObject target;
	private Vector3 target_pos;
	private bool talk;

	
	// Use this for initialization
	void Start () {
		max_speed = 0.15f;
		current_speed = 0;
		currentx = 0;
		currenty = 0;
		speedx = 0;
		speedy = 0;
		orientation = 0;
		rotation = 0;
		
		//time trackers
		duration = 0;

		//movement variable
		free = true;
		talk = false;
	}
	
	// Update is called once per frame
	void Update () {
		current_speed = Mathf.Sqrt(Mathf.Pow (currentx,2)+Mathf.Pow (currenty,2));
		if (currentx == 0) {
			if (currenty > 0) {
				orientation = 90;
			} else if (currenty == 0) {
				orientation = 0;
			} else {
				orientation = -90;
			}
		} else {
			orientation = Mathf.Atan (currenty / currentx);
		}
		//the expected speed
		float angle = rotation * (Mathf.PI / 180);
		speedx = max_speed * Mathf.Cos (angle);
		speedy = max_speed * Mathf.Sin (angle);

		target = GameObject.FindWithTag("Wolf");

		if (talk) {
			duration += Time.deltaTime;
			if(duration > 4f){
				talk = false;
				duration = 0;
			}
		}
		//If nothing in sight, free wondering
		else if (free) {
			DetectDistance (target);
			FreeWonder ();
		}
		//If see wolf, pursue
		else if (!free){
			DetectDistance (target);
			Pursue();
		}
	}

	void OnGUI() {
		if (free) {
			GUI.Label (new Rect (5, 40, 200, 20), "Hunter: Free wondering");
		} else if (talk) {
			GUI.Label (new Rect (5, 40, 200, 20), "Hunter: Caught Wolf");
		} else {
			GUI.Label (new Rect (5, 40, 150, 20), "Hunter: In Pursue");
		}
	}
	
	void FreeWonder() {
		duration += Time.deltaTime;
		
		if (duration > 0.5) {
			float rotate = Random.Range (-45, 45);
			transform.Rotate (0, 0, rotate);
			rotation += rotate;
			
			duration = 0;
		}
		movement ();
		 
	}

	void movement() {
		DetectBound();

		if ( (currentx > speedx) && currenty < speedy ) {
			currentx -= 0.005f;
			currenty += 0.005f;
		}
		if ( (currenty > speedy) && currentx < speedx ) {
			currenty -= 0.005f;
			currentx += 0.005f;
		}
		
		if ( (currenty > speedy) && currentx > speedx ) {
			currenty -= 0.005f;
			currentx -= 0.005f;
		}
		
		if ( (currenty < speedy) && currentx < speedx ) {
			currenty += 0.005f;
			currentx += 0.005f;
		}
		
		Vector3 newpos = new Vector3 (transform.position.x + currentx, transform.position.y + currenty, 0);
		transform.position = newpos;
	}

	//Detect if Wolf is in range
	void DetectDistance(GameObject wolf){
		Vector3 pos = wolf.transform.position;
		target_pos = pos;
		float x = pos.x;
		float y = pos.y;
		float distance = Mathf.Sqrt ((transform.position.x - x) * (transform.position.x - x) + (transform.position.y - y) * (transform.position.y - y));

		if (distance < 25) {
			max_speed = 0.18f;
			if(distance < 8){
				max_speed -= 0.01f;
				//max_speed = 0.1f;
			}
			if(distance < 3){
				talk = true;
			}
			free = false;
		} else {
			max_speed = 0.15f;
			free = true;
		}
	}
	
	//detect the bound of map and turn around
	void DetectBound(){
		if (rotation > 360) {
			rotation -= 360;
		}
		if (rotation < -360) {
			rotation += 360;
		}

		float x = transform.position.x;
		float y = transform.position.y;
		float angle = rotation * (Mathf.PI / 180);
		if (x + 2*Mathf.Cos (angle) > 88f || x + 2*Mathf.Cos (angle) < -18f) {
			transform.Rotate(0,0,180-2*rotation);
			rotation = 180 - rotation;
		}
		if (y + 2*Mathf.Sin (angle) > 80f || y + 2*Mathf.Sin (angle) < -24f) {
			transform.Rotate(0,0,-2*rotation);
			rotation = -rotation;
		}
	}

	void Pursue () {
		float x = target_pos.x;
		float y = target_pos.y;
		
		//prediction of future position
		float angle = target.transform.rotation.eulerAngles.z / 180 * Mathf.PI;


		float px = x + (Mathf.Cos (angle) * 15 * 0.2f);
		float py = y + (Mathf.Sin (angle) * 15 * 0.2f);

		Vector3 start = new Vector3 (transform.position.x, transform.position.y, 0);
		Vector3 end = new Vector3 (px, py, 0);
		Debug.DrawLine (start, end, Color.yellow, 1/24);



		float gap = rotation;
		/*if (x == transform.position.x) {
			if (y > transform.position.y) {
				rotation = 90;

			}
			if (y <= transform.position.y) {
				rotation = -90;
			}
		}
		else {*/
			float chase_angle = Mathf.Atan ((py - transform.position.y) / (px - transform.position.x));
			if(px > transform.position.x){
				rotation = chase_angle * 180 / Mathf.PI;
			}
			else {
				rotation = (chase_angle * 180 / Mathf.PI) + 180;
			}
		//}
		gap = rotation - gap;

		transform.Rotate (0, 0, gap);

		movement ();
	}
}
