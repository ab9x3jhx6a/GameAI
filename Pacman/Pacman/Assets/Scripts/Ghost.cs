using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ghost : MonoBehaviour {
	
	public Pacman pacman;
	public float offset;//seconds to wait 
	public GameObject loader;
	public Transform origin;
	
	Grid grid;
	
	private List<Node> chase_path;
	
	void Awake() {
		grid = loader.GetComponent<Grid>();
	}

	void Start() {
		chase_path = new List<Node> ();
	}

	void Update() {
		if (dist (transform, pacman.transform) < 1.5f) {
			Application.Quit();
			Debug.Log ("Pacman is dead.");
		}

		Node next = new Node (true,Vector3.zero,0,0);
		if (!pacman.super) {
			FindPath (transform.position, pacman.transform.position);
			next = chase_path[0];
		} else {//if pacman has super pallet
			if(dist (transform,origin) > 2.0f){
				FindPath (transform.position, origin.position);
				next = chase_path[0];
			}
			else {
				int rand = Random.Range(0,4);
				if(rand == 0){//up
					Vector3 flee = new Vector3(transform.position.x, 1, transform.position.z+1);
					if(grid.NodeFromWorldPoint(flee).walkable){
						next = grid.NodeFromWorldPoint(flee);
					}
					else {
						flee = new Vector3(transform.position.x, 1, transform.position.z-1);
						next = grid.NodeFromWorldPoint(flee);
					}
				}
				if(rand == 1){//down
					Vector3 flee = new Vector3(transform.position.x, 1, transform.position.z-1);
					if(grid.NodeFromWorldPoint(flee).walkable){
						next = grid.NodeFromWorldPoint(flee);
					}
					else {
						flee = new Vector3(transform.position.x, 1, transform.position.z-1);
						next = grid.NodeFromWorldPoint(flee);
					}
				}
				if(rand == 2){//left
					Vector3 flee = new Vector3(transform.position.x-1, 1, transform.position.z);
					if(grid.NodeFromWorldPoint(flee).walkable){
						next = grid.NodeFromWorldPoint(flee);
					}
					else {
						flee = new Vector3(transform.position.x, 1, transform.position.z-1);
						next = grid.NodeFromWorldPoint(flee);
					}
				}
				if(rand == 3){//right
					Vector3 flee = new Vector3(transform.position.x+1, 1, transform.position.z);
					if(grid.NodeFromWorldPoint(flee).walkable){
						next = grid.NodeFromWorldPoint(flee);
					}
					else {
						flee = new Vector3(transform.position.x, 1, transform.position.z-1);
						next = grid.NodeFromWorldPoint(flee);
					}
				}
			}
		}
		move (next);
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

	void move(Node next) {
		Vector3 pos = next.worldPosition;

		pos.x -= 0.5f;
		pos.z -= 0.5f;
		transform.position = pos;
	}
	
	void RetracePath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		
		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse();
		
		chase_path = path;
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