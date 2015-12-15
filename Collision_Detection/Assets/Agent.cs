using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Agent : MonoBehaviour {

	public Vector3 velocity;

	public float direction;

	private float speed = 0.35f;
	private float orientation;// orientation = atan(v.y/v.x)
	private float posx;
	private float posy;

	private float detect_distance = 15; //the zone in which the agents will consider to avoid collision
	private float align_distance = 25; //the zone in which the agents will consider align

	// Use this for initialization
	void Start () {
		float random_angle = Random.value * 360;
		transform.Rotate (0, 0, random_angle);
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.rotation.eulerAngles.z > 180) {
			transform.Rotate(0,0,-360);
		}
		if (transform.rotation.eulerAngles.z < -180) {
			transform.Rotate (0,0,360);
		}

		List<Vector3> positions = new List<Vector3>();
		List<float> rotations = new List<float>(); //in degrees
		posx = transform.position.x;
		posy = transform.position.y;

		foreach (Transform child in transform.parent)
		{
			float tx = child.transform.position.x;
			float ty = child.transform.position.y;
			//get rid of itself
			if(tx!=transform.position.x && ty!=transform.position.y){
				//float distance = Mathf.Sqrt (Mathf.Pow (tx - posx , 2) + Mathf.Pow (ty - posy , 2) );
				//if(distance < detect_distance){
				positions.Add(child.transform.position);
				float angle = child.transform.rotation.eulerAngles.z;
				rotations.Add(angle);
				//}
			}
		}

		//move this agent
		if (positions.Count != 0 && rotations.Count != 0) {
			move (positions, rotations);
		} else {
			Debug.Log (gameObject.name);
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

	Vector3 avoid_collisions(List<Vector3> positions) {//get targets locations and avoid
		Vector3 str;
		str = new Vector3 (0, 0, 0);

		for (int i=0; i<positions.Count; i++) {
			Vector3 tmp = positions[i];
			int total = positions.Count; //how many targets
			float distance = Mathf.Sqrt(Mathf.Pow (tmp.x-posx, 2) + Mathf.Pow (tmp.y - posy, 2));

			if(distance < detect_distance){
				float ratio = (detect_distance - distance) / detect_distance;
				//calculate avoid angle
				float angle = get_angle(posx, posy, tmp.x, tmp.y);
				angle = angle/180*Mathf.PI;

				float vx = speed * Mathf.Cos(angle) * ratio;
				float vy = speed * Mathf.Sin(angle) * ratio;
				//create str from this target:

				Vector3 str_tar = new Vector3(vx/total, vy/total, 0);
				str += str_tar;
			}
		}
		return str;
	}

	Vector3 match_velocity(List<Vector3> positions, List<float> rotations) {//also rotate the object
		Vector3 str;
		str = new Vector3 (0, 0, 0);
		float avg_direction = 0.0f;
		float avg_speed = 0.0f;
		int count = 0; //count how many agents in the zone

		for (int i=0; i<positions.Count; i++) {
			Vector3 tmp = positions[i];
			float distance = Mathf.Sqrt(Mathf.Pow (tmp.x-posx, 2) + Mathf.Pow (tmp.y - posy, 2));
			if( distance<align_distance ){
				count++;
				avg_direction += rotations[i];
				avg_speed += positions[i].z;
			}
		}
		avg_direction /= count;
		avg_speed /= count;


		float rotate_angle = avg_direction - transform.rotation.eulerAngles.z;
		//angle += Random.value * 5 - 2.5f;
		transform.Rotate (0, 0, rotate_angle);

		float move_angle = avg_direction / 180 * Mathf.PI;

		float vx = Mathf.Cos (move_angle) * avg_speed;
		float vy = Mathf.Sin (move_angle) * avg_speed;

		Vector3 str_tar = new Vector3 (vx, vy, 0);

		str += str_tar;

		return str;
	}

	Vector3 flock_to_center(List<Vector3> positions) {
		Vector3 str;
		str = new Vector3 (0, 0, 0);

		Vector3 avg_pos = new Vector3 (0, 0, 0);

		for (int i=0; i<positions.Count; i++) {
			avg_pos += positions[i]/positions.Count;
		}

		float angle = get_angle (avg_pos.x, avg_pos.y, posx, posy);
		angle = angle / 180 * Mathf.PI;

		float vx = speed * Mathf.Cos(angle);
		float vy = speed * Mathf.Sin(angle);
		//create str from this target:
		
		Vector3 str_tar = new Vector3(vx, vy, 0);
		str += str_tar/positions.Count * 2;

		return str;
	}

	void draw_str (Vector3 str){
		Vector3 start = new Vector3 (transform.position.x, transform.position.y, 0);
		Vector3 end = new Vector3 (transform.position.x + str.x, transform.position.y + str.y, 0);
		Debug.DrawLine (start, end, Color.yellow, 1/24);
	}

	void follow_path(){

	}

	void cone_check() {

	}

	void collision_prediction() {

	}

	void move(List<Vector3> positions, List<float> rotations) {
		Vector3 str1 = avoid_collisions ( positions );
		Vector3 str2 = match_velocity ( positions, rotations );
		Vector3 str3 = flock_to_center ( positions );

		draw_str (str1);
		draw_str (str2);
		draw_str (str3);

		Vector3 final = str1 + str2 + str3; //final modified velocity
		float t_speed = Mathf.Sqrt (Mathf.Pow (final.x, 2) + Mathf.Pow (final.y, 2));

		if (t_speed > speed) {
			final.x *= speed/t_speed;
			final.y *= speed/t_speed;
		}
		t_speed = speed;

		Vector3 pos = new Vector3 (transform.position.x + final.x, transform.position.y + final.y, t_speed); //storing speed a z value

		transform.position = pos;
	}



}
