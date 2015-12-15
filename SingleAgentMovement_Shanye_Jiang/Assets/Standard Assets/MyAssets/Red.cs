using UnityEngine;
using System.Collections;

public class Red : MonoBehaviour {
	//path to follow
	public Vector3[] waypoints = new Vector3[8];
	private int passed;
	private float min_dist_overall = 10000f;//gurantee any distances would be smaller than this one
	private float lookatdistance;

	public float max_speed;
	private float orientation;
	private float rotate;
	private float current_speed; 
	private float speedx;
	private float speedy;
	
	//variable that tracks duration of movements for freewondering
	private float duration;
	//targets
	private GameObject wolf;
	private Vector3 wolf_pos;
	//movements variables
	private bool alert;
	private bool caught;
	private bool breaking;

	// Use this for initialization
	void Start () {
		//create waypoints
		waypoints [0] = new Vector3 (0f, 67f, 0f);
		waypoints [1] = new Vector3 (25f, 67f, 0f);
		waypoints [2] = new Vector3 (25f, 41f, 0f);
		waypoints [3] = new Vector3 (43f, 41f, 0f);
		waypoints [4] = new Vector3 (43f, 60f, 0f);
		waypoints [5] = new Vector3 (51f, 60f, 0f);
		waypoints [6] = new Vector3 (51f, 43f, 0f);
		waypoints [7] = new Vector3 (63f, 43f, 0f);
		//optimize var for pathfinding
		passed = 1;
		lookatdistance = 10;


		max_speed = 0.1f;
		current_speed = 0;
		speedx = 0;
		speedy = 0;
		orientation = 30;
		rotate = 0;
		
		//time trackers
		duration = 0;
		
		//movement variables
		alert = false;
		caught = false;

		//adjust the sprite
		transform.Rotate (0, 0, 30);
	}
	
	// Update is called once per frame
	void Update () {
		DrawPath ();
		current_speed = Mathf.Sqrt(Mathf.Pow (speedx,2)+Mathf.Pow (speedy,2));
		wolf = GameObject.FindWithTag ("Wolf");


		if (caught) {
			duration += Time.deltaTime;
			if(duration > 3) {
				caught = false;
				breaking = true;
				duration = 0;
			}
		}
		else{
			DetectWolf (wolf);
			//If nothing in sight, free wondering
			if (!alert) {

				if(Mathf.Sqrt ( Mathf.Pow (transform.position.x - waypoints[7].x, 2)+ Mathf.Pow (transform.position.y - waypoints[7].y ,2)) < 3){
					max_speed = 0.05f;
					FreeWonder();
				}
				else{
					FollowPath();
				}
				//FreeWonder ();
			} else if (alert) {//pursue > alert
				Escape ();
			}
		}
		DetectBound();

	}

	void OnGUI() {
		if (!alert) {
			GUI.Label (new Rect (15, 20, 200, 20), "Red: Following Path");
		} else if (caught) {
			GUI.Label (new Rect (15, 20, 100, 20), "Red: Caught");
		} else {
			GUI.Label (new Rect (15, 20, 100, 20), "Red: Escaping");
		}
	}

	void DrawPath() {
		Debug.DrawLine (waypoints [0], waypoints [1], Color.white);
		Debug.DrawLine (waypoints [1], waypoints [2], Color.white);
		Debug.DrawLine (waypoints [2], waypoints [3], Color.white);
		Debug.DrawLine (waypoints [3], waypoints [4], Color.white);
		Debug.DrawLine (waypoints [4], waypoints [5], Color.white);
		Debug.DrawLine (waypoints [5], waypoints [6], Color.white);
		Debug.DrawLine (waypoints [6], waypoints [7], Color.white);
	}

	void FollowPath() {
		//calculate distance to each segiments
		//and pick the min

		Vector3 min;
		Vector3 lookat;

		min = new Vector3(waypoints[passed-1].x, waypoints[passed-1].y, min_dist_overall);
		lookat = min;//setup

		int line_num = passed;//which segment is the nearest to Red

		for (int i=passed; i<7; i++) {//search all line segments not passed yet

			Vector3 start = waypoints[i-1];
			Vector3 end = waypoints[i];

			Vector3 min_distance_point;//x=distance, y = x axis, z = y axis;
			//initialize
			min_distance_point =  new Vector3(
				Mathf.Sqrt(Mathf.Pow( start.x - transform.position.x, 2) + Mathf.Pow (start.y - transform.position.y, 2)),
				start.x, start.y);

			/*if(start.x == end.x){
				if(start.y > end.y){
					float delta_x = Mathf.Pow( (start.x - transform.position.x), 2);
					for (int j=(int)start.y; j<(int)end.y; j++){
						float distance = Mathf.Sqrt(delta_x + Mathf.Pow (j - transform.position.y, 2));
						if(distance < min_distance_point.x){
							min_distance_point.x = distance;
							min_distance_point.z = j;//update the y coord;
						}
					}
				}
				else { 
					float delta_x = Mathf.Pow( (start.x - transform.position.x), 2);
					for (int j=(int)start.y; j>(int)end.y; j--){
						float distance = Mathf.Sqrt(delta_x + Mathf.Pow (j - transform.position.y, 2));
						if(distance < min_distance_point.x){
							min_distance_point.x = distance;
							min_distance_point.z = j;//update the y coord;
						}
					}
				}

			}
			if(start.y == end.y){
				if(start.x > end.x){
					float delta_y = Mathf.Pow( (start.y - transform.position.y), 2);
					for (int j=(int)start.x; j<(int)end.x; j++){
						float distance = Mathf.Sqrt(delta_y + Mathf.Pow (j - transform.position.x, 2));
						if(distance < min_distance_point.x){
							min_distance_point.x = distance;
							min_distance_point.y = j;//update the x coord;
						}
					}
				}
				else { 
					float delta_y = Mathf.Pow( (start.y - transform.position.y), 2);
					for (int j=(int)start.x; j<(int)end.x; j--){
						float distance = Mathf.Sqrt(delta_y + Mathf.Pow (j - transform.position.x, 2));
						if(distance < min_distance_point.x){
							min_distance_point.x = distance;
							min_distance_point.y = j;//update the x coord;
						}
					}
				}
			}*/

			if( min_distance_point.x < min.z){
				//update overall min point
				min.z = min_distance_point.x;
				min.x = min_distance_point.y;
				min.y = min_distance_point.z;
				line_num = i;
				if(i>1 && i-1>passed){
					passed = i-1;
				}
			}
		}//-----------------end of all search


		//float lookatdistance = 10;//how far she will look from the nearest point

		if (line_num == 7 || line_num == 6) {
			lookat = waypoints [7];//look at the house if it's near
			lookatdistance = 0;
		} else if (line_num < 7){
			if (min.x == waypoints [line_num].x) {
				if (min.y > waypoints [line_num].y) {
					if(min.y > transform.position.y){
						min.y = transform.position.y;
					}
					if ((min.y - lookatdistance) < waypoints [line_num].y) {
						float remain = min.y - waypoints [line_num].y;
						if (waypoints [line_num + 1].x > min.x) {
							lookat = new Vector3(waypoints[line_num].x + remain, waypoints[line_num].y, 0);
						} else {
							lookat = new Vector3(waypoints[line_num].x - remain, waypoints[line_num].y, 0);
						}
					} else {
						lookat = new Vector3 (min.x, min.y - lookatdistance, 0);
					}
				} else {
					if(min.y < transform.position.y){
						min.y = transform.position.y;
					}
					if ((min.y + lookatdistance) > waypoints [line_num].y) {
						float remain = waypoints [line_num].y - min.y;
						if (waypoints [line_num + 1].x > min.x) {
							lookat = new Vector3(waypoints[line_num].x + remain, waypoints[line_num].y, 0);
						} else {
							lookat = new Vector3(waypoints[line_num].x - remain, waypoints[line_num].y, 0);
						}
					} else {
						lookat = new Vector3 (min.x, min.y + lookatdistance, 0);
					}
				}
			}
			if (min.y == waypoints [line_num].y) {

				if (min.x > waypoints [line_num].x) {
					if(min.x > transform.position.x){
						min.x = transform.position.x;
					}
					if ((min.x - lookatdistance) < waypoints [line_num].x) {
						float remain = min.x - waypoints [line_num].x;
						if (waypoints [line_num + 1].y > min.y) {
							lookat = new Vector3(waypoints[line_num].x, waypoints[line_num].y + remain, 0);
						} else {
							lookat = new Vector3(waypoints[line_num].x, waypoints[line_num].y - remain, 0);
						}
					} else {
						lookat = new Vector3 (min.x - lookatdistance, min.y, 0);
					}
				} else {
					if(min.x < transform.position.x){
						min.x = transform.position.x;
					}
					if ((min.x + lookatdistance) > waypoints[line_num].x) {
						float remain = waypoints [line_num].x - min.x;
						if (waypoints [line_num + 1].y > min.y) {
							lookat = new Vector3(waypoints[line_num].x, waypoints[line_num].y + remain, 0);
						} else {
							lookat = new Vector3(waypoints[line_num].x , waypoints[line_num].y - remain, 0);
						}
					} else {
						lookat = new Vector3 (min.x + lookatdistance, min.y, 0);
					}
				}
			}
		}//----------finish set lookat point

		float gap = orientation;
		Debug.DrawLine (transform.position, lookat, Color.yellow);
		Debug.DrawLine (transform.position, new Vector3 (min.x, min.y, 0), Color.yellow);

		float chase_angle = Mathf.Atan ((lookat.y - transform.position.y) / (lookat.x - transform.position.x));
		if(lookat.x > transform.position.x){
			orientation = chase_angle * 180 / Mathf.PI;
		}
		else {
			orientation = (chase_angle * 180 / Mathf.PI) + 180;
		}

		gap = orientation - gap;
		
		transform.Rotate (0, 0, gap);

		movement ();
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
	
	//Detect if wolf is in range
	void DetectWolf(GameObject wolf){
		Vector3 pos = wolf.transform.position;
		wolf_pos = pos;
		float x = pos.x;
		float y = pos.y;
		float distance = Mathf.Sqrt ((transform.position.x - x) * (transform.position.x - x) + (transform.position.y - y) * (transform.position.y - y));
		
		if (distance < 15) {
			max_speed = 0.18f;
			if(distance < 3){
				max_speed = 0.05f;
			}
			if(distance < 1 && !breaking) {
				caught = true;
			}
			if (distance > 3){
				breaking = false;
			}
			alert = true;
		} else {
			max_speed = 0.1f;
			alert = false;
		}
	}

	
	//detect the bound of map and turn around
	void DetectBound(){
		if (orientation >= 180) {
			orientation -= 360;
		}
		if (orientation <= -180) {
			orientation += 360;
		}
		float x = transform.position.x;
		float y = transform.position.y;
		float angle = orientation * (Mathf.PI / 180);
		if (x + 5*Mathf.Cos (angle) > 88f || x + 5*Mathf.Cos (angle) < -18f) {
			transform.Rotate(0,0,180-2*orientation);
			orientation = 180 - orientation;
		}
		if (y + 5*Mathf.Sin (angle) > 80f || y + 5*Mathf.Sin (angle) < -24f) {
			transform.Rotate(0,0,-2*orientation);
			orientation = -orientation;
		}
	}
	
	void Escape() {
		float x = wolf_pos.x;
		float y = wolf_pos.y;

		duration += Time.deltaTime;
		if (duration > 1) {
			rotate = Random.Range (-20, 20);
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
				orientation = chase_angle * 180 / Mathf.PI + 180;
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
