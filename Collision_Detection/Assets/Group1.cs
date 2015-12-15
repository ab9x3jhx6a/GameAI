﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Group1 : MonoBehaviour {
	
	public Vector3 velocity;
	
	public float direction;  
	
	private float speed = 0.25f;
	private float orientation;// orientation = atan(v.y/v.x)
	private float posx;
	private float posy;
	
	private float detect_distance = 15; //the zone in which the agents will consider to avoid collision
	private float align_distance = 25; //the zone in which the agents will consider align

	private float path_angle = 0;
	private float path_dir = 1.5f; 
	  
	public GameObject other_group;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (transform.rotation.eulerAngles.z > 180) {
			transform.Rotate(0,0,-360);
		}
		if (transform.rotation.eulerAngles.z < -180) {
			transform.Rotate (0,0,360);
		}
		//follow path
		if (path_angle >= 90) {
			path_dir = -path_dir;
		}
		if (path_angle <= -90) {
			path_dir = -path_dir;
		}
		path_angle += path_dir;

		
		List<Vector3> positions = new List<Vector3>();
		List<float> rotations = new List<float>(); //in degrees
		posx = transform.position.x;
		posy = transform.position.y;

		//flocking within group
		foreach (Transform child in transform.parent)
		{
			transform.Rotate (0, 0, path_dir);
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
		//transform.Rotate (0, 0, rotate_angle);

		
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
		str += str_tar/positions.Count;
		
		return str;
	}
		
	Vector3 follow_path(){
		float angle = path_angle / 180 * Mathf.PI;
		float speedx = speed * Mathf.Cos (angle);
		float speedy = speed * Mathf.Sin (angle);

		float speedz = Mathf.Sqrt ((speedx * speedx) + (speedy * speedy));

		Vector3 str = new Vector3 (speedx, speedy, speedz);
		return str;
	}
	
	Vector3 cone_check() {
		float cone_angle = 60;
		float face_angle = transform.rotation.eulerAngles.z;
		Vector3 str = new Vector3 (0, 0, 0);
		List<Vector3> cone = new List<Vector3>();
		Vector3 final_avoid = new Vector3(0,0,0);

		foreach (Transform child in other_group.transform) {
			float direction = get_angle(child.transform.position.x, child.transform.position.y, posx, posy);
			if(direction < face_angle + cone_angle && direction > face_angle - cone_angle){
				cone.Add(child.transform.position);
			}
		}
		if (cone.Count == 0) {
			return str;
		} else {//avoid the average direction
			for(int i=0; i<cone.Count; i++){
				final_avoid.x += cone[i].x/cone.Count;
				final_avoid.y += cone[i].y/cone.Count;
			}

			float final_avoid_angle = get_angle(final_avoid.x, final_avoid.y, posx, posy);

			//avoid behavior
			float avoid_angle = cone_angle/3;

			if(final_avoid_angle > face_angle){
				final_avoid_angle = (final_avoid_angle  - avoid_angle) / 180 * Mathf.PI;
				str.x = speed * Mathf.Cos(final_avoid_angle);
				str.y = speed * Mathf.Sin(final_avoid_angle);
			}else{
				final_avoid_angle = (final_avoid_angle + avoid_angle) / 180 * Mathf.PI;
				str.x = speed * Mathf.Cos(final_avoid_angle);
				str.y = speed * Mathf.Sin(final_avoid_angle);
			}
			return str;
		}
	}

	void draw_str (Vector3 str, Color clr){
		Vector3 start = new Vector3 (transform.position.x, transform.position.y, 0);
		Vector3 end = new Vector3 (transform.position.x + str.x*10, transform.position.y + str.y*10, 0);
		Debug.DrawLine (start, end, clr, 1/24);
	}
	
	void move(List<Vector3> positions, List<float> rotations) {
		Vector3 str1 = avoid_collisions ( positions );
		Vector3 str3 = flock_to_center ( positions );
		Vector3 str4 = follow_path ();
		Vector3 str5 = cone_check ();
		
		draw_str (str1, Color.blue);
		draw_str (str3, Color.green);
		draw_str (str4, Color.yellow);
		draw_str (str5, Color.white);
		
		Vector3 final = str1 + str3 + str4 + str5; //final modified velocity
		float t_speed = Mathf.Sqrt (Mathf.Pow (final.x, 2) + Mathf.Pow (final.y, 2));
		
		if (t_speed > speed) {
			final.x *= speed/t_speed;
			final.y *= speed/t_speed;
		}
		t_speed = speed;
		
		Vector3 pos = new Vector3 (transform.position.x + final.x, transform.position.y + final.y, t_speed); //storing speed a z value

		float final_rotate = get_angle (pos.x, pos.y, transform.position.x, transform.position.y) - transform.rotation.eulerAngles.z;
		transform.Rotate (0, 0, final_rotate);
		
		transform.position = pos;
	}
	
	
	
}
