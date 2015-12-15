using UnityEngine;
using System.Collections;

public class Pathfind : MonoBehaviour {

	Grid grid;

	void Awake() {
		grid = GetComponent<grid> ();
	}

	void FindPath(Vector3 startPos, Vector3 targetPos) {

	}
}
