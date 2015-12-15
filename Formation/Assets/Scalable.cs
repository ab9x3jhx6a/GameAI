using UnityEngine;
using System.Collections;

public class Scalable: MonoBehaviour {
	//path to follow
	private int passed;
	private float lookatdistance;
	
	public float max_speed;
	
	private float x;
	private float y;

	private int num_child;
	public Transform target;

	
	// Use this for initialization
	void Start () {

		//optimize var for pathfinding
		passed = 0;
		lookatdistance = 10;
		num_child = 0;
		
		max_speed = 0.1f;
	}
	
	// Update is called once per frame
	void Update () {

		num_child = transform.childCount;
		
		//update current location
		x = transform.position.x;
		y = transform.position.y;

		move ();
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

	Vector3 FollowTarget() {
		//calculate distance to each segiments
		//and pick the min
		float lookat_angle_degrees = get_angle (target.position.x, target.position.y, x, y);
		float lookat_angle = lookat_angle_degrees / 180 * Mathf.PI;
		Debug.DrawLine (transform.position, target.position, Color.green);

		foreach (Transform child in transform) {
			child.Rotate (0, 0, lookat_angle_degrees - child.rotation.eulerAngles.z);
		}

		float dx = target.position.x - x;
		float dy = target.position.y - y;
		float distance = Mathf.Sqrt (dx * dx + dy * dy);
		distance /= 800;
		
		Vector3 str = new Vector3 (0, 0, 0);
		Debug.DrawLine (transform.position, target.position, Color.green);

		str.x = (max_speed + 0) * Mathf.Cos (lookat_angle);
		str.y = (max_speed + 0) * Mathf.Sin (lookat_angle);
		
		return str;
	}

	void move() {
		Vector3 str1 = FollowTarget ();

		Vector3 final = str1; //final modified velocity
		float t_speed = Mathf.Sqrt (Mathf.Pow (final.x, 2) + Mathf.Pow (final.y, 2));
		
		Vector3 pos = new Vector3 (transform.position.x + final.x, transform.position.y + final.y, 0); //storing speed in z value
		
		//float final_rotate = get_angle (pos.x, pos.y, transform.position.x, transform.position.y) - transform.rotation.eulerAngles.z;
		//transform.Rotate (0, 0, final_rotate-360);

		transform.position = pos;
	}
}
