﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {
	public bool OnlyDisplayPath;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	Node[,] grid;
	
	float nodeDiameter;
	int gridSizeX, gridSizeY;
	
	void Start() {
		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
		CreateGrid();
	}

	void CreateGrid() {
		grid = new Node[gridSizeX,gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;
		
		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius,unwalkableMask));
				grid[x,y] = new Node(walkable,worldPoint, x,y);
			}
		}
	}
	
	public List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();
		
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;
				
				int checkX = node.gridX + x;
				int checkY = node.gridY + y;
				
				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}
		return neighbours;
	}
	
	
	public Node NodeFromWorldPoint(Vector3 worldPosition) {
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);
		
		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		return grid[x,y];
	}
	
	public List<Node> path;
	public List<Node> ghost = new List<Node>();
	public List<Node> traveled = new List<Node>();

	void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));

		if (OnlyDisplayPath) {
			if (path != null) {
				foreach (Node n in path) {
					Gizmos.color = Color.black;
					Gizmos.DrawCube (n.worldPosition, Vector3.one * (nodeDiameter - .1f));
				}
			}
		} else {

			if (grid != null) {
				foreach (Node n in grid) {
					Gizmos.color = (n.walkable) ? Color.yellow : Color.blue;
					if (path != null)
						if (path.Contains (n))
							Gizmos.color = Color.black;
					if (traveled != null){
						if (traveled.Contains(n))
							Gizmos.color = Color.white;
					}
					/*if (ghost != null)
						if (ghost.Contains (n))
							Gizmos.color = Color.black;*/ //really laggy if turned on
					Gizmos.DrawCube (n.worldPosition, Vector3.one * (nodeDiameter - .1f));
				}
			}
		}
	}
}

public class Node {
	
	public bool walkable;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;
	
	public int gCost;
	public int hCost;
	public Node parent;
	
	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) {
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}
	
	public int fCost {
		get {
			return gCost + hCost;
		}
	}
}