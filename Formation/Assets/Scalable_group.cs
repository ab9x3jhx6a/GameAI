using UnityEngine;
using System.Collections;

public class Scalable_group: MonoBehaviour {
	
	public float max_speed;
	public Vector3 strength;
	
	private float x;
	private float y;

	//raycast
	public Transform leftStart, leftEnd;
	public Transform rightStart, rightEnd;
	public bool left = false;
	public bool right = false;
	
	public Transform target;
	
	// Use this for initialization
	void Start () {
		max_speed = 0.1f;
		strength = new Vector3 (0, 0, 0);
		
		move ();
	}
	
	// Update is called once per frame
	void Update () {
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
		
		transform.Rotate (0, 0, lookat_angle_degrees - transform.rotation.eulerAngles.z);
		
		float dx = target.position.x - x;
		float dy = target.position.y - y;
		float distance = Mathf.Sqrt (dx * dx + dy * dy);
		distance /= 800;
		
		Vector3 str = new Vector3 (0, 0, 0);
		Debug.DrawLine (transform.position, target.position, Color.green);
		
		str.x = (max_speed + distance) * Mathf.Cos (lookat_angle);
		str.y = (max_speed + distance) * Mathf.Sin (lookat_angle);
		
		return str;
	}
	
	Vector3 Avoid_Obstacles() {
		//Do ray cast and collision detection
		
		
		Vector3 str = new Vector3 (0, 0, 0);
		float angle = get_angle (x, y, leftEnd.position.x, leftEnd.position.y);//right dodge angle = left dodge angle - 60 degrees
		angle = (angle + 30) / 180 * Mathf.PI;
		
		//modify the speed
		if (left) {
			str.x = max_speed * Mathf.Cos (angle);
			str.y = max_speed * Mathf.Sin (angle);
		}
		if (right) {
			str.x = max_speed * Mathf.Cos (angle - Mathf.PI/2);
			str.y = max_speed * Mathf.Sin (angle - Mathf.PI/2);
		}
		
		Vector3 end = transform.position + str;
		Debug.DrawLine (transform.position, end, Color.red);
		
		strength = str;
		
		return str*2;
	}
	
	void move() {
		Vector3 str1 = Avoid_Obstacles ();
		Vector3 str3 = FollowTarget ();
		
		Vector3 zero = new Vector3 (0, 0, 0);

		
		Vector3 final = str1 + str3; //final modified velocity
		float t_speed = Mathf.Sqrt (Mathf.Pow (final.x, 2) + Mathf.Pow (final.y, 2));
		
		Vector3 pos = new Vector3 (transform.position.x + final.x, transform.position.y + final.y, 0); //storing speed in z value
		
		//float final_rotate = get_angle (pos.x, pos.y, transform.position.x, transform.position.y) - transform.rotation.eulerAngles.z;
		//transform.Rotate (0, 0, final_rotate-360);
		
		transform.position = pos;
	}
	
}
