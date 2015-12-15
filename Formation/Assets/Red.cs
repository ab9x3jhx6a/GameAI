using UnityEngine;
using System.Collections;

public class Red : MonoBehaviour {
	//path to follow
	public Transform[] path = new Transform[20];
	private Vector3[] waypoints = new Vector3[20];
	private int passed;
	private float lookatdistance;

	public float max_speed;

	private float x;
	private float y;

	//raycast
	public Transform leftStart, leftEnd;
	public Transform rightStart, rightEnd;
	public bool left = false;
	public bool right = false;

	// Use this for initialization
	void Start () {
		for (int i=0; i<waypoints.Length; i++) {
			waypoints[i] = path[i].position;
		}
		//optimize var for pathfinding
		passed = 0;
		lookatdistance = 10;

		max_speed = 0.1f;
	}
	
	// Update is called once per frame
	void Update () {
		DrawPath ();

		//update current location
		x = transform.position.x;
		y = transform.position.y;

		Raycasting ();

		move ();
	}

	void Raycasting() {
		Debug.DrawLine (leftStart.position, leftEnd.position, Color.green);
		Debug.DrawLine (rightStart.position, rightEnd.position, Color.green);

		left = Physics2D.Linecast (leftStart.position, leftEnd.position);
		right = Physics2D.Linecast (rightStart.position, rightEnd.position);
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

	Vector3 Avoid_Obstacles() {
		//Do ray cast and collision detection


		Vector3 str = new Vector3 (0, 0, 0);
		float angle = get_angle (x, y, leftEnd.position.x, leftEnd.position.y);//right dodge angle = left dodge angle - 60 degrees
		angle = (angle + 30) / 180 * Mathf.PI;

		//modify the speed
		if (left) {
			Debug.Log (leftEnd.position);
			str.x = max_speed * Mathf.Cos (angle);
			str.y = max_speed * Mathf.Sin (angle);
		}
		if (right) {
			Debug.Log (rightEnd.position);
			str.x = max_speed * Mathf.Cos (angle - Mathf.PI/2);
			str.y = max_speed * Mathf.Sin (angle - Mathf.PI/2);
		}

		Vector3 end = transform.position + str;
		Debug.DrawLine (transform.position, end, Color.red);

		return str/2;
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
			if(distance_to_thispoint < 1.2f){
				passed = i;
			}
		}

		if (passed > 12) {
			passed = 0;
		}

		float lookat_angle_degrees = get_angle (lookat.x, lookat.y, x, y);
		float lookat_angle = lookat_angle_degrees / 180 * Mathf.PI;
		Debug.DrawLine (transform.position, lookat, Color.green);

		transform.Rotate (0, 0, lookat_angle_degrees - transform.rotation.eulerAngles.z);

		Vector3 str = new Vector3 (0, 0, 0);

		str.x = max_speed * Mathf.Cos (lookat_angle);
		str.y = max_speed * Mathf.Sin (lookat_angle);

		return str;
	}

	
	void move() {
		Vector3 str1 = FollowPath ();
		Vector3 str2 = Avoid_Obstacles ();

		Vector3 final = str1 + str2; //final modified velocity
		float t_speed = Mathf.Sqrt (Mathf.Pow (final.x, 2) + Mathf.Pow (final.y, 2));
		
		Vector3 pos = new Vector3 (transform.position.x + final.x, transform.position.y + final.y, 0); //storing speed in z value
		
		//float final_rotate = get_angle (pos.x, pos.y, transform.position.x, transform.position.y) - transform.rotation.eulerAngles.z;
		//transform.Rotate (0, 0, final_rotate-360);
		
		transform.position = pos;
	}
}
