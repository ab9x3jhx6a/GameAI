using UnityEngine;
using System.Collections;

public class target: MonoBehaviour {
	//path to follow
	public Transform[] path = new Transform[20];
	private Vector3[] waypoints = new Vector3[20];
	private int passed;

	public float max_speed;
	
	private float x;
	private float y;

	
	// Use this for initialization
	void Start () {
		for (int i=0; i<waypoints.Length; i++) {
			waypoints[i] = path[i].position;
		}
		//optimize var for pathfinding
		passed = 0;
		
		max_speed = 0.12f;
		transform.position = path [1].position;
	}
	
	// Update is called once per frame
	void Update () {
		DrawPath ();

		//update current location
		x = transform.position.x;
		y = transform.position.y;
		
		move_target ();
	}
	
	void DrawPath() {
		for (int i = 0; i < waypoints.Length - 2; i++) {
			Debug.DrawLine(waypoints[i], waypoints[i+1], Color.red);
		}
	}
	
	float get_angle(float x, float y, float tx, float ty){//direction point from targetx to x, same for y. Return value in degrees
		float angle = 0.0f;
		if (tx == x) {
			if (ty > y) {
				angle = -90;
			}
			if (ty <= y) {
				angle = 90;
			}
		}
		else {
			float atan = Mathf.Atan ((ty - y) / (tx - x));
			if(tx > x){
				angle = (atan * 180 / Mathf.PI) + 180;
			}
			else {
				angle = (atan * 180 / Mathf.PI);
			}
		}
		return angle;
	}
	
	Vector3 FollowPath() {
		//calculate distance to each segiments
		//and pick the min
		
		Vector3 previous;
		Vector3 next;
		Vector3 lookat;
		
		previous = waypoints[passed];//setup
		next = waypoints [passed + 1];
		lookat = next;
		
		for (int i=passed; i < waypoints.Length - 1; i++) {//search all line segments not passed yet
			Vector3 thispoint = waypoints [i];
			
			float distance_to_thispoint = Mathf.Sqrt (Mathf.Pow (thispoint.x - transform.position.x, 2) + Mathf.Pow (thispoint.y - transform.position.y, 2));
			if(distance_to_thispoint <= 0.1f){
				passed = i;
			}
		}

		if (passed >= 12) {
			passed = 0;
		}
		
		float lookat_angle_degrees = get_angle (lookat.x, lookat.y, x, y);
		float lookat_angle = lookat_angle_degrees / 180 * Mathf.PI;
		Vector3 str = new Vector3 (0, 0, 0);
		
		str.x = max_speed * Mathf.Cos (lookat_angle);
		str.y = max_speed * Mathf.Sin (lookat_angle);
		
		return str;
	}

	
	void move_target() {
		Vector3 target_str = FollowPath ();
		Vector3 pos = new Vector3 (transform.position.x + target_str.x, transform.position.y + target_str.y, 0);
		transform.position = pos;
	}

}
