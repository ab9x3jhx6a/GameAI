using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;

public class loader : MonoBehaviour {
	
	public string fileNameToLoad;
	
	public int mapWidth;
	public int mapHeight;

	public GameObject Wall;

	public bool toggle;
	
	private int[,] _file;
	private int[,] _rep;

	
	void Awake () {
		_file = Load (Application.dataPath + "\\Maps\\" + fileNameToLoad);
		_rep = GenerateRep (_file);

		if (toggle) {
			BuildMap (_file);
		}
		if (!toggle) {
			BuildMap (_rep);
		}
	}
	
	void BuildMap (int[,] map) {
		Debug.Log("Building Map...");
		for(int i = 0; i < map.GetLength(0); i++) {
			for(int j = 0; j < map.GetLength(1); j++) {
				if(map[i,j] == 1) {
					GameObject TilePrefab = Instantiate(Wall, new Vector3(i - Mathf.RoundToInt(map.GetLength(0)/2), 1, j - Mathf.RoundToInt(map.GetLength(1)/2)), Quaternion.identity) as GameObject;
					TilePrefab.transform.parent = GameObject.FindGameObjectWithTag("Map").transform;
				}
			}
		}
		Debug.Log("Building Completed!");
	}
	
	private int[,] Load(string filePath) {
		Debug.Log("Loading File...");
		using (StreamReader sr = new StreamReader(filePath)) {
			string input = sr.ReadToEnd ();
			string[] lines = input.Split (new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
			int[,] tiles = new int[lines.Length, mapWidth];
			Debug.Log ("Parsing...");
			for (int i = 0; i < lines.Length; i++) {
				string st = lines [i];
				for (int j = 0; j <  mapWidth; j++) {
					if (st [j] == 'M')
						tiles [i, j] = 1;
					if (st [j] == 'T')
						tiles [i, j] = 1;
					if (st [j] == '.')
						tiles [i, j] = 0;
				}
			}
			Debug.Log ("Parsing Completed!");
			return tiles;
		}
	}

	private int[,] GenerateRep(int[,] tiles){
		int replength = tiles.GetLength (0) / 2;
		int repwidth = tiles.GetLength (1) / 2;

		int[,] rep = new int[replength, repwidth];
		Debug.Log ("Generating...");
		for(int i=0; i<replength; i++){
			for(int j=0; j<repwidth; j++) {
				int value = tiles[i*2,j*2] + tiles[i*2+1, j*2] + tiles[i*2,j*2] + tiles[i*2+1, j*2+1];
				if(value <= 1) {
					rep[i,j] = 0;
				}
				else
					rep[i,j] = 1; //non-passable
			}
		}
		return rep;
	}








































}