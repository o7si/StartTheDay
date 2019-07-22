using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityMazeGenerate : MonoBehaviour {

	public struct MazePoint {
		public int x;
		public int y;
		public int k;
		public bool sign;
		public char state;
		public string route;
	};

	public struct Point {
		public int x;
		public int y;
		public Point(int x, int y) {
			this.x = x;
			this.y = y;
		}
	};
	// Main
	public GameObject father;
	// 道路
	public GameObject roadEdge;
	public GameObject road0;
	public GameObject road1;
	public GameObject road2a;
	public GameObject road2b;
	public GameObject road3;
	public GameObject road4;
	// End記号
	public GameObject end;
	// プリセット車
	public GameObject carPrecubDFS;
	public GameObject carPrecubBFS;
	public GameObject carPrecubAStar;
	// 車
	GameObject carDFS;
	GameObject carBFS;
	GameObject carAStar;
	// AからFまでの建物
	public GameObject A;
	public GameObject B;
	public GameObject C;
	public GameObject D;
	public GameObject E;
	public GameObject F;
	List<GameObject> building = new List<GameObject> ();
	// 運行ルート
	public string pathDFS;
	public string pathBFS;
	public string pathAStar; 
	// 運行属性
	public float rotRate = 5.0f;
	public float goRate = 5.0f;
	public float DFSAngle = 0.0f;
	public float BFSAngle = 0.0f;
	public float AStarAngle = 0.0f;
	// else属性（更改できない）
	public const float distance = 90.0f;
	public const float edgeDis = 63.0f;
	public const float buildingDis = 90.0f;
	public const float height = 0.0f;
	public const float carHeight = 1.0f;
	public const float edgeHeight = 0.48f;
	public const float buildingHeight = 0.0f;
	// 地図サイズ
	const int MAXX = 50;
	const int MAXY = 50;
	public int maxX = 5;
	public int maxY = 5;
	public MazePoint[,] mazeMap = new MazePoint[MAXX, MAXY];
	// MapMessageを記録する
	List<Point> mazeRoad = new List<Point> ();
	List<Point> noMazeRoad = new List<Point> ();
	// 方向
	string dir = "RDLT";
	int[] dx = new int[] { 0, -1, 0, 1 };
	int[] dy = new int[] { -1, 0, 1, 0 };
	// Rotate基準
	string[,] rotRoad = new string[,] {
		{ "R", "D", "L", "T" },
		{ "RL", "DT", "LR", "TD" },
		{ "RD", "DL", "LT", "RT" },
		{ "RDL", "DLT", "RLT", "RDT" },
		{ "RDLT", "RDLT", "RDLT", "RDLT" }
	};
	// rename
	string toString(int x, int y) {
		return "[" + x + ", " + y + "]";
	}
	//地図を初期化する
	void InitialMap() {
		// 地図点の初期化
		for(int i = 0; i < maxX; i++) {
			for (int j = 0; j < maxY; j++) {
				mazeMap [i, j].x = i;
				mazeMap [i, j].y = j;
				mazeMap [i, j].k = 0;
				mazeMap [i, j].sign = false;
				mazeMap [i, j].state = '#';
				mazeMap [i, j].route = "";
			}
		}
		// 建物Tableの初期化
		building.Add (A);
		building.Add (B);
		building.Add (C);
		building.Add (D);
		building.Add (E);
		building.Add (F);
	}
	// Primアルゴリズムに通じて迷宮を生成するのを進行する
	void PrimMazeGenerate() {
		System.Random rd = new System.Random ();
		int randomX = rd.Next (maxX);
		int randomY = rd.Next (maxY);
		List<Point> list = new List<Point> ();
		list.Add (new Point (randomX, randomY));
		mazeMap [randomX, randomY].sign = true;
		while (list.Count > 0) {
			int random = rd.Next (list.Count);
			Point point = list [random];
			list.RemoveAt (random);
			if (mazeMap [point.x, point.y].k > 1)
				continue;
			mazeMap[point.x, point.y].state = '.';
			for (int k = 0; k < 4; k++) {
				int x = point.x + dx [k];
				int y = point.y + dy [k];
				if (x < 0 || x >= maxX || y < 0 || y >= maxY)
					continue;
				mazeMap [x, y].k++;
				if (mazeMap [x, y].state == '.' || mazeMap [x, y].sign)
					continue;
				list.Add (new Point (x, y));
				mazeMap [x, y].sign = true;
			}
		}
	}
	// ルートアップデート
	void MazeRouteUpdate() {
		for(int i = 0; i < maxX; i++) {
			for(int j = 0; j < maxY; j++) {
				if (mazeMap [i, j].state == '#') {
					noMazeRoad.Add (new Point (i, j));
					continue;
				}
				mazeRoad.Add (new Point (i, j));
				for (int k = 0; k < 4; k++) {
					int x = i + dx [k];
					int y = j + dy [k];
					if (x < 0 || x >= maxX || y < 0 || y >= maxY)
						continue;
					if (mazeMap [x, y].state == '#')
						continue;
					mazeMap [i, j].route += dir [k];
				}
			}
		}
	}
	// シティールートモデル化:CityRoadMode化
	void CityRoadGenerate() {
		// 普通のルート
		for (int k = 0; k < mazeRoad.Count; k++) {
			int x = mazeRoad [k].x;
			int y = mazeRoad [k].y;
			int tempA = -1, tempB = -1;
			for (int m = 0; m < rotRoad.GetLength (0); m++) {
				for (int n = 0; n < rotRoad.GetLength (1); n++) {
					if (mazeMap [x, y].route == rotRoad [m, n]) {
						tempA = m;
						tempB = n;
					}
				}
			}
			GameObject gameobj = null;
			switch (tempA) {
			case 0:
				gameobj = Instantiate (road1, new Vector3 (x * distance, height, y * distance), Quaternion.Euler (0, 90.0f * tempB, 0));
				break;
			case 1:
				gameobj = Instantiate (road2a, new Vector3 (x * distance, height, y * distance), Quaternion.Euler (0, 90.0f * tempB, 0));
				break;
			case 2:
				gameobj = Instantiate (road2b, new Vector3 (x * distance, height, y * distance), Quaternion.Euler (0, 90.0f * tempB, 0));
				break;
			case 3:
				gameobj = Instantiate (road3, new Vector3 (x * distance, height, y * distance), Quaternion.Euler (0, 90.0f * tempB, 0));
				break;
			case 4:
				gameobj = Instantiate (road4, new Vector3 (x * distance, height, y * distance), Quaternion.Euler (0, 90.0f * tempB, 0));
				break;
			}
			gameobj.transform.parent = father.transform;
			gameobj.transform.name = toString (x, y);
		}
		// 混みのルート
		for(int k = 0; k < noMazeRoad.Count; k++) {
			int x = noMazeRoad [k].x;
			int y = noMazeRoad [k].y;
			GameObject gameobj = null;
			gameobj = Instantiate (road0, new Vector3 (x * distance, height, y * distance), Quaternion.identity);
			gameobj.transform.parent = father.transform;
			gameobj.transform.name = toString (x, y);
		}
		// エッジのルート
		for (int j = 0; j < maxY; j++) {
			GameObject gameobj1 = Instantiate (roadEdge, new Vector3 (-edgeDis, edgeHeight, j * distance), Quaternion.Euler (0, -90.0f, 0));
			gameobj1.transform.parent = father.transform;
			GameObject gameobj2 = Instantiate (roadEdge, new Vector3 (edgeDis + (maxY - 1) * distance, edgeHeight, j * distance), Quaternion.Euler (0, 90.0f, 0));
			gameobj2.transform.parent = father.transform;
		}
		for (int i = 0; i < maxX; i++) {
			GameObject gameobj1 = Instantiate (roadEdge, new Vector3 (i * distance, edgeHeight, -edgeDis), Quaternion.Euler (0, -180.0f, 0));
			gameobj1.transform.parent = father.transform;
			GameObject gameobj2 = Instantiate (roadEdge, new Vector3 (i * distance, edgeHeight, edgeDis + (maxX - 1) * distance), Quaternion.Euler (0, 0.0f, 0));
			gameobj2.transform.parent = father.transform;
		}
		// 建物を生成する
		System.Random rd = new System.Random ();
		for(int i = -1; i < maxX; i++) {
			for (int j = -1; j < maxY; j++) {
				int random = rd.Next (building.Count);
				int rotK = rd.Next (4);
				GameObject gameobj = Instantiate (building [random], new Vector3 (i * buildingDis + 45.0f, buildingHeight, j * buildingDis + 45.0f), Quaternion.Euler (0, rotK * 90.0f, 0));
				gameobj.transform.parent = father.transform;
			}
		}
		// 初と末
		mazeMap[mazeRoad[0].x, mazeRoad[0].y].state = 'S';
		mazeMap [mazeRoad [mazeRoad.Count - 1].x, mazeRoad [mazeRoad.Count - 1].y].state = 'E';
		// 車
		carDFS = Instantiate (carPrecubDFS, new Vector3 (mazeRoad [0].x * distance, carHeight, mazeRoad [0].y * distance), Quaternion.identity);
		carDFS.transform.name = "CarDFS";
		carBFS = Instantiate (carPrecubBFS, new Vector3 (mazeRoad [0].x * distance, carHeight, mazeRoad [0].y * distance), Quaternion.identity);
		carBFS.transform.name = "CarBFS";
		carAStar = Instantiate (carPrecubAStar, new Vector3 (mazeRoad [0].x * distance, carHeight, mazeRoad [0].y * distance), Quaternion.identity);
		carAStar.transform.name = "CarAStar";
		// EndSign
		float t = 5.0f;
		Instantiate (end, new Vector3 (mazeRoad [mazeRoad.Count - 1].x * distance - t, height, mazeRoad [mazeRoad.Count - 1].y * distance - t), Quaternion.identity);
		Instantiate (end, new Vector3 (mazeRoad [mazeRoad.Count - 1].x * distance - t, height, mazeRoad [mazeRoad.Count - 1].y * distance + t), Quaternion.identity);
		Instantiate (end, new Vector3 (mazeRoad [mazeRoad.Count - 1].x * distance + t, height, mazeRoad [mazeRoad.Count - 1].y * distance - t), Quaternion.identity);
		Instantiate (end, new Vector3 (mazeRoad [mazeRoad.Count - 1].x * distance + t, height, mazeRoad [mazeRoad.Count - 1].y * distance + t), Quaternion.identity);
	}
	// 車の旋転
	IEnumerator carRotate(GameObject car, float carAngle, float angle) {
		bool isClockWise = angle > 0 ? true : false;
		float end = carAngle + angle;
		while (carAngle != end) {
			carAngle += isClockWise ? rotRate : -rotRate;
			if (isClockWise)
				car.transform.Rotate (new Vector3 (0, rotRate, 0), Space.World);
			else
				car.transform.Rotate (new Vector3 (0, -rotRate, 0), Space.World);
			yield return null;
		}
		yield return null;
	}
	// 車の進み
	IEnumerator carGoForward(GameObject car, char ch) {
		float temp = distance;
		while (temp > 0.0f) {
			if (ch == 'R') {
				car.transform.Translate (new Vector3 (0, 0, - goRate), Space.World); 
			} else if (ch == 'D') {
				car.transform.Translate (new Vector3 (- goRate, 0, 0), Space.World); 
			} else if (ch == 'L') {
				car.transform.Translate (new Vector3 (0, 0, + goRate), Space.World);
			} else if (ch == 'T') {
				car.transform.Translate (new Vector3 (+ goRate, 0, 0), Space.World);
			}
			temp -= goRate;
			yield return null;
		}
	}
	// 車の運転
	IEnumerator driveCar(GameObject car, float carAngle, string dPath) {
		for(int k = 0; k < dPath.Length; k++) {
			int t;
			if (k == 0) {
				t = dir.IndexOf (dPath [k]) - dir.IndexOf ('R');
			} else {
				t = dir.IndexOf (dPath [k]) - dir.IndexOf (dPath [k - 1]);
			}
			if (t == 3)
				t = -1;
			if (t == -3)
				t = 1;
			yield return StartCoroutine (carRotate (car, carAngle, 90.0f * t));
			yield return StartCoroutine(carGoForward(car, dPath[k]));
		}
		//Destroy (car, 3.0f);
		yield return null;
	}
	// Use this for initialization
	void Start () {
		InitialMap ();
		PrimMazeGenerate ();
		MazeRouteUpdate ();
		CityRoadGenerate ();
		transform.GetComponent<MazeSearchDFSAndBFS> ().setChMazeMap (maxX, maxY);
		pathDFS = transform.GetComponent<MazeSearchDFSAndBFS> ().DFS ();
		Debug.Log (pathDFS + "---" + pathDFS.Length);
		transform.GetComponent<MazeSearchDFSAndBFS> ().setChMazeMap (maxX, maxY);
		pathBFS = transform.GetComponent<MazeSearchDFSAndBFS> ().BFS ();
		Debug.Log (pathBFS + "---" + pathBFS.Length);
		transform.GetComponent<MazeSearchAStar>().initialAStarMap(maxX, maxY);
		pathAStar = transform.GetComponent<MazeSearchAStar> ().AStar ();
		Debug.Log (pathAStar + "---" + pathAStar.Length);
		transform.GetComponent<ChangeCamera> ().setCamera ();
	}
	bool DKey = true;
	bool BKey = true;
	bool AKey = true;
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.D) && DKey) {
			DKey = false;
			StartCoroutine (driveCar (carDFS, DFSAngle, pathDFS));
		}
		if (Input.GetKeyDown (KeyCode.B) && BKey) {
			BKey = false;
			StartCoroutine (driveCar (carBFS, BFSAngle, pathBFS));
		}
		if (Input.GetKeyDown (KeyCode.A) && AKey) {
			AKey = false;
			StartCoroutine (driveCar (carAStar, AStarAngle, pathAStar));
		}
	}
}
