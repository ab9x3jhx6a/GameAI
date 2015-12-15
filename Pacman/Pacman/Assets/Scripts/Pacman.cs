using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pacman : MonoBehaviour {
	
	public Transform super1, super2, super3, super4;
	public Transform ghost1, ghost2, ghost3, ghost4;
	public GameObject loader;

	public float speed = 5.0f; //pacman move speed

	public bool super; //whether super pallet is active
	private float duration = 5.0f; //super pallet last 5 seconds
	private float counter = 0.0f;
	//states
	private bool runaway; //when a ghost is close and no super pallet active
	private bool chase; //when a ghost is close and with super pallet active
	public Transform target_ghost; //the ghost pacman is chasing
	private bool wander; //socre most points and get super pallet if possible

	private bool eat1,eat2,eat3,eat4;
	private int num_eaten;

	private List<Node> pac_path;

	Grid grid;
	
	void Awake() {
		grid = loader.GetComponent<Grid>();
	}

	void Start() {
		eat1 = false;
		eat2 = false;
		eat3 = false;
		eat4 = false;
		num_eaten = 0;

		pac_path = new List<Node> ();
		wander = true;
		chase = false;
		runaway = false;
	}

	void OnGUI() {
		GUI.Label (new Rect (15, 20, 100, 20), "Score: " + (grid.traveled.Count + 4*num_eaten));
	}
	
	void Update() {
		Node next = new Node (true,Vector3.zero,0,0);
		if (wander) {
			if (!eat1) {
				FindPath (transform.position, super1.position);
			} else if (!eat2) {
				FindPath (transform.position, super2.position);
			} else if (!eat3) {
				FindPath (transform.position, super3.position);
			} else if (!eat4) {
				FindPath (transform.position, super4.position);
			}
			next = pac_path[0];

		} else if (runaway) {
			next = Runaway();
		} else if (chase) {
			FindPath(transform.position, target_ghost.position);
			next = pac_path[0];
		}

		Decide ();

		Node current = grid.NodeFromWorldPoint (transform.position);
		if (!grid.traveled.Contains (current)) {
			grid.traveled.Add (current);
		}

		Pallet ();
		move (next);
	}

	Node Runaway(){
		Node next = new Node (true,Vector3.zero,0,0);
		Vector3 avg_ghost = target_ghost.position;
		avg_ghost.x = transform.position.x + (transform.position.x - target_ghost.position.x);
		avg_ghost.z = transform.position.z + (transform.position.z - target_ghost.position.z);
		pac_path = new List<Node> ();
		FindPath (transform.position, avg_ghost);
		if (grid.NodeFromWorldPoint(avg_ghost).walkable) {
			next = pac_path [0];
		} else {
			int rand = Random.Range(0,4);
			if(rand == 0){//up
				Vector3 flee = new Vector3(transform.position.x, 1, transform.position.z+1);
				if(grid.NodeFromWorldPoint(flee).walkable){
					next = grid.NodeFromWorldPoint(flee);
				}
				else {
					flee = new Vector3(transform.position.x, 1, transform.position.z-1);
					if(grid.NodeFromWorldPoint(flee).walkable){
						next = grid.NodeFromWorldPoint(flee);
					}
					else
						next = grid.NodeFromWorldPoint(transform.position);
				}
			}
			if(rand == 1){//down
				Vector3 flee = new Vector3(transform.position.x, 1, transform.position.z-1);
				if(grid.NodeFromWorldPoint(flee).walkable){
					next = grid.NodeFromWorldPoint(flee);
				}
				else {
					flee = new Vector3(transform.position.x, 1, transform.position.z-1);
					if(grid.NodeFromWorldPoint(flee).walkable){
						next = grid.NodeFromWorldPoint(flee);
					}
					else
						next = grid.NodeFromWorldPoint(transform.position);
				}
			}
			if(rand == 2){//left
				Vector3 flee = new Vector3(transform.position.x-1, 1, transform.position.z);
				if(grid.NodeFromWorldPoint(flee).walkable){
					next = grid.NodeFromWorldPoint(flee);
				}
				else {
					flee = new Vector3(transform.position.x, 1, transform.position.z-1);
					if(grid.NodeFromWorldPoint(flee).walkable){
						next = grid.NodeFromWorldPoint(flee);
					}
					else
						next = grid.NodeFromWorldPoint(transform.position);
				}
			}
			if(rand == 3){//right
				Vector3 flee = new Vector3(transform.position.x+1, 1, transform.position.z);
				if(grid.NodeFromWorldPoint(flee).walkable){
					next = grid.NodeFromWorldPoint(flee);
				}
				else {
					flee = new Vector3(transform.position.x, 1, transform.position.z-1);
					if(grid.NodeFromWorldPoint(flee).walkable){
						next = grid.NodeFromWorldPoint(flee);
					}
					else
						next = grid.NodeFromWorldPoint(transform.position);
				}
			}
		}

		return next;
	}

	void Decide() {//decide which state it's in
		bool close1 = false;
		bool close2 = false;
		bool close3 = false;
		bool close4 = false;
		if (dist (transform, ghost1) < 15.0f) {
			close1 = true;
		}
		if (dist (transform, ghost2) < 15.0f) {
			close2 = true;
		}
		if (dist (transform, ghost3) < 15.0f) {
			close3 = true;
		}
		if (dist (transform, ghost4) < 15.0f) {
			close4 = true;
		}

		if (dist (transform, ghost1) >= 15.0f) {
			close1 = false;
		}
		if (dist (transform, ghost1) >= 15.0f) {
			close2 = false;
		}
		if (dist (transform, ghost1) >= 15.0f) {
			close3 = false;
		}
		if (dist (transform, ghost1) >= 15.0f) {
			close4 = false;
		}

		float dist1 = dist (transform, ghost1);
		float dist2 = dist (transform, ghost2);
		float dist3 = dist (transform, ghost3);
		float dist4 = dist (transform, ghost4);

		float min = Mathf.Min (dist1, dist2, dist3, dist4);

		if (min == dist1) {
			target_ghost = ghost1;
		}
		if (min == dist2) {
			target_ghost = ghost2;
		}
		if (min == dist3) {
			target_ghost = ghost3;
		}
		if (min == dist4) {
			target_ghost = ghost4;
		}


		if (super) {
			counter += Time.deltaTime;
			if( counter >= duration ){
				super = false;
				counter = 0;
			}
		}

		bool close = close1 || close2 || close3 || close4;

		if (super && close) {
			chase = true;
			runaway = false;
			wander = false;
		}

		if (super && !close) {
			chase = false;
			runaway = false;
			wander = true;
		}

		if (!super && close) {
			chase = false;
			runaway = true;
			wander = false;
		}
	}

	void Pallet() {
		Vector3 down = new Vector3(0,-10,0);
		if (dist (transform, super1) < 2.0f) {
			super1.position = (super1.position + down);
			eat1 = true;
			num_eaten++;
			super = true;
		}
		if (dist (transform, super2) < 2.0f) {
			super2.position = (super2.position + down);
			eat2 = true;
			num_eaten++;
			super = true;
		}
		if (dist (transform, super3) < 2.0f) {
			super4.position = (super3.position + down);
			eat3 = true;
			num_eaten++;
			super = true;
		}
		if (dist (transform, super4) < 2.0f) {
			super4.position = (super4.position + down);
			eat4 = true;
			num_eaten++;
			super = true;
		}
	}
	
	void move(Node next) { //called every frame
		Vector3 pos = next.worldPosition;
		/*
		float dx = pos.x - transform.position.x;
		float dy = pos.z - transform.position.z;

		float speedx = dx*speed;
		float speedy = dy*speed;*/


		//Vector3 new_pos = new Vector3 (transform.position.x + speedx, transform.position.y, transform.position.z + speedy);
		pos.x -= 0.5f;
		pos.z -= 0.5f;
		transform.position = pos;

		//if not traveled, collect the pallet and mark it as travelled
	}


	void FindPath(Vector3 startPos, Vector3 targetPos) {
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);
		
		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);
		
		while (openSet.Count > 0) {
			Node currentNode = openSet[0];
			for (int i = 1; i < openSet.Count; i ++) {
				if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) {
					currentNode = openSet[i];
				}
			}
			
			openSet.Remove(currentNode);
			closedSet.Add(currentNode);
			
			if (currentNode == targetNode) {
				RetracePath(startNode,targetNode);
				return;
			}
			
			foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
				if (!neighbour.walkable || closedSet.Contains(neighbour)) {
					continue;
				}
				
				int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
				if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = currentNode;
					
					if (!openSet.Contains(neighbour)){
						openSet.Add(neighbour);
					}
					
				}
			}
		}
	}
	
	void RetracePath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		
		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse();
		
		//grid.path = path;

		pac_path =  path;
	}
	
	int GetDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
		
		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}


	//helper funciton
	float dist(Transform start, Transform target) {
		float sx = start.position.x;
		float sy = start.position.z;
		float tx = target.position.x;
		float ty = target.position.z;
		
		float dx = tx - sx;
		float dy = ty - sy;
		
		float dist = Mathf.Sqrt (dx * dx + dy * dy);
		
		return dist;
	}
}