using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCube : MonoBehaviour {

	public struct RubikCube {
		public GameObject cube;
		public string location;
		public int state;
	};

	public string superCube  = "Rubik";

	public const int cornerCount = 8;
	public const int edgeCount  = 12;

	public string[] s_corner = new string[] { "UFR", "UFL", "UBL", "UBR", "DFR", "DFL", "DBL", "DBR" };
	public string[] s_edge   = new string[] { "UF", "UL", "UB", "UR", "DF", "DL", "DB", "DR", "FR", "FL", "BL", "BR" };

	public RubikCube[] corner = new RubikCube[cornerCount];
	public RubikCube[] edge = new RubikCube[edgeCount];

	private int[] upCorner = new int[4] { 1, 2, 3, 4 };
	private int[] upEdge = new int[4] { 1, 2, 3, 4 };
	private int[] downCorner = new int[4] { 5, 6, 7, 8 };
	private	int[] downEdge = new int[4]{ 5, 6, 7, 8 };
	private	int[] frontCorner = new int[4] { 1, 2, 6, 5 };
	private int[] frontEdge = new int[4] { 1, 10, 5, 9 };
	private int[] backCorner = new int[4] { 4, 3, 7, 8 };
	private int[] backEdge = new int[4] { 3, 11, 7, 12 };
	private int[] rightCorner = new int[4] { 1, 4, 8, 5 };
	private int[] rightEdge = new int[4] { 4, 12, 8, 9 };
	private int[] leftCorner = new int[4] { 2, 3, 7, 6 };
	private int[] leftEdge = new int[4] { 2, 11, 6, 10 };

	public const float rotateAngle = 90.0f;
	public float rotateRate = 15.0f;
	public float upAngle = 0.0f;
	public float downAngle = 0.0f;
	public float frontAngle = 0.0f;
	public float backAngle = 0.0f;
	public float rightAngle = 0.0f;
	public float leftAngle = 0.0f;

	public bool single = false;

	void Start () {
		// Set Corner Property
		for(int k = 0; k < cornerCount; k++) {
			corner [k].cube = GameObject.Find (superCube + "/" + s_corner [k]);
			corner [k].location = s_corner [k];
			corner [k].state = 0;
		}
		// Set Edge Property
		for(int k = 0; k < edgeCount; k++) {
			edge [k].cube = GameObject.Find (superCube + "/" + s_edge [k]);
			edge [k].location = s_edge [k];
			edge [k].state = 0;
		}
	}

	void Swap<T>(ref T a, ref T b) { T t = a; a = b; b = t; }
		
	IEnumerator RandomOrder() {
		const int cnt = 20;
		string s = "UuDdFfBbLlRr";
		string result = "";
		System.Random random = new System.Random ();
		for (int i = 0; i < cnt; i++)
			result += s [random.Next (s.Length)];
		Debug.Log (result + "(" + result.Length + ")");
		yield return StartCoroutine (StartRotate (result));
		//yield return StartCoroutine (BottomCross (0));
	}

	IEnumerator StartRotate (string magicRot) {
		this.rotateRate = 15.0f;
		int step = 0;
		while (step < magicRot.Length) {
			switch (magicRot [step++]) {
			case 'U':
				yield return StartCoroutine (MainControl ("Up", upCorner, upEdge, upAngle, 'y', true));
				upAngle += rotateAngle;
				break;
			case 'u':
				yield return StartCoroutine (MainControl ("Up", upCorner, upEdge, upAngle, 'y', false));
				upAngle -= rotateAngle;
				break;
			case 'D':
				yield return StartCoroutine (MainControl ("Down", downCorner, downEdge, downAngle, 'y', false));
				downAngle -= rotateAngle;
				break;
			case 'd':
				yield return StartCoroutine (MainControl ("Down", downCorner, downEdge, downAngle, 'y', true));
				downAngle += rotateAngle;
				break;
			case 'F':
				yield return StartCoroutine (MainControl ("Front", frontCorner, frontEdge, frontAngle, 'z', false));
				frontAngle -= rotateAngle;
				break;
			case 'f':
				yield return StartCoroutine (MainControl ("Front", frontCorner, frontEdge, frontAngle, 'z', true));
				frontAngle += rotateAngle;
				break;
			case 'B':
				yield return StartCoroutine (MainControl ("Back", backCorner, backEdge, backAngle, 'z', true));
				backAngle += rotateAngle;
				break;
			case 'b':
				yield return StartCoroutine (MainControl ("Back", backCorner, backEdge, backAngle, 'z', false));
				backAngle -= rotateAngle;
				break;
			case 'L':
				yield return StartCoroutine (MainControl ("Left", leftCorner, leftEdge, leftAngle, 'x', false));
				leftAngle -= rotateAngle;
				break;
			case 'l':
				yield return StartCoroutine (MainControl ("Left", leftCorner, leftEdge, leftAngle, 'x', true));
				leftAngle += rotateAngle;
				break;
			case 'R':
				yield return StartCoroutine (MainControl ("Right", rightCorner, rightEdge, rightAngle, 'x', true));
				rightAngle += rotateAngle;
				break;
			case 'r':
				yield return StartCoroutine (MainControl ("Right", rightCorner, rightEdge, rightAngle, 'x', false));
				rightAngle -= rotateAngle;
				break;
			}
		}
		yield return null;
	}

	IEnumerator MainControl(string face, int[] cornerNum, int[] edgeNum, float start, char shaft, bool isClockWise) {
		GameObject father = GameObject.Find (superCube + "/" + face);
		// Get GameObject(Corner)
		for (int k = 0; k < cornerNum.Length; k++) {
			corner [cornerNum [k] - 1].cube.transform.parent = father.transform;
		}
		// Get GameObject(Edge)
		for (int k = 0; k < edgeNum.Length; k++) {
			edge [edgeNum [k] - 1].cube.transform.parent = father.transform;
		}
		// Location Change
		yield return StartCoroutine (UpdateState (cornerNum, edgeNum, face [0]));
		// Array Change
		yield return StartCoroutine (UpdateLocation (cornerNum, edgeNum, isClockWise));
		// Start Animation
		if (isClockWise) {
			yield return StartCoroutine (RotateAnimation (father, start, start + rotateAngle, rotateRate, shaft, isClockWise));
			start += rotateAngle;
		} else {
			yield return StartCoroutine (RotateAnimation (father, start, start - rotateAngle, rotateRate, shaft, isClockWise));
			start -= rotateAngle;
		}
		yield return null;
	}

	IEnumerator UpdateState (int []cornerArr, int[] edgeArr, char face) {
		// Change Corner Location State
		for (int k = 0; k < cornerArr.Length; k++) {
			int mid_i = corner [cornerArr [k] - 1].state;
			int left_i = (mid_i - 1 + 3) % 3;
			int right_i = (mid_i + 1 + 3) % 3;
			char mid_c = s_corner [cornerArr[k] - 1][mid_i];
			char left_c = s_corner [cornerArr [k] - 1][left_i];
			char right_c = s_corner [cornerArr [k] - 1][right_i];
			int changeValue = mid_c == face ? 0 : left_c == face ? 1 : -1;
			int result = (corner [cornerArr [k] - 1].state + changeValue + 3) % 3;
			corner [cornerArr [k] - 1].state = result;
		}
		// Change Edge Location State
		for (int k = 0; k < edgeArr.Length; k++) {
			if (edgeArr [k] > 8) {
				// Mid
				int state = edge [edgeArr [k] - 1].state;
				if (face == 'F' || face == 'B') {
					state = state == 0 ? 1 : 0;
					edge [edgeArr [k] - 1].state = state;
				}
			} else {
				// High And Low
				int state = edge [edgeArr [k] - 1].state;
				int temp = edgeArr [k];
				if (temp == 1 || temp == 3 || temp == 5 || temp == 7) {
					if (face != 'U' && face != 'D') {
						state = state == 0 ? 1 : 0;
						edge [edgeArr [k] - 1].state = state;
					}
				}
			}
		}
		yield return null;
	}

	IEnumerator UpdateLocation(int[] cornerArr, int[] edgeArr, bool isClockWise) {

		if (isClockWise) {
			for (int k = cornerArr.Length - 1; k > 0; k--)
				Swap (ref corner [cornerArr [k] - 1], ref corner [cornerArr [k - 1] - 1]);
			for (int k = edgeArr.Length - 1; k > 0; k--)
				Swap (ref edge [edgeArr [k] - 1], ref edge [edgeArr [k - 1] - 1]);
		} else {
			for (int k = 0; k < cornerArr.Length - 1; k++)
				Swap (ref corner [cornerArr [k] - 1], ref corner [cornerArr [k + 1] - 1]);
			for (int k = 0; k < edgeArr.Length - 1; k++)
				Swap (ref edge [edgeArr [k] - 1], ref edge [edgeArr [k + 1] - 1]);
		}
		yield return null;
	}

	IEnumerator RotateAnimation(GameObject father, float start, float end, float rate, char shaft, bool isClockWise) {
		while (start != end) {
			start += isClockWise ?  rate : -rate; 
			if (shaft == 'x')
				father.transform.rotation = Quaternion.Euler (start, 0, 0);
			else if (shaft == 'y')
				father.transform.rotation = Quaternion.Euler (0, start, 0);
			else if (shaft == 'z')
				father.transform.rotation = Quaternion.Euler (0, 0, start);
			yield return null;
		}
	}

	IEnumerator BottomCross (int t) {
		// Solve the white cross on the bottom
		string[] pieces = new string[] { "DF", "DR", "DB", "DL" };
		string[] lowToHigh = new string[] { "FF", "LL", "BB", "RR" };
		string[] midToHigh = new string[] { "RUr", "FUf", "LUl", "BUb" };
		string[] highToZero = new string[] { "", "u", "uu", "U" };
		string[] zeroToFinal = new string[] { "FFd", "urFRd"};
		if (t >= 4) {
			if(!single)
				yield return StartCoroutine (BottomCorner (0));
			yield break;
		}
		int k = 0;
		for (k = 0; k < edge.Length; k++)
			if (edge [k].location == pieces [t])
				break;
		if (k == 0 || k == 1 || k == 2 || k == 3) {
			yield return StartCoroutine (StartRotate (highToZero [k]));
			yield return StartCoroutine (StartRotate (zeroToFinal [edge [0].state]));
			yield return StartCoroutine (BottomCross (t + 1));
		} else if (k == 4 || k == 5 || k == 6 || k == 7) {
			yield return StartCoroutine (StartRotate (lowToHigh [k - 4]));
			yield return StartCoroutine (BottomCross (t));
		} else if (k == 8 || k == 9 || k == 10 || k == 11) {
			yield return StartCoroutine (StartRotate (midToHigh [k - 8]));
			yield return StartCoroutine (BottomCross (t));
		}
		yield return null;
	}

	IEnumerator BottomCorner(int t) {
		// Solve four corner pieces on the bottom
		string[] pieces = new string[] {"DFR", "DBR", "DBL", "DFL"};
		string[] lowToHigh = new string[] { "RUr", "FUf", "LUl", "BUb" };
		string[] highToZero = new string[] { "", "u", "uu", "U"};
		string[] zeroToFinal = new string[] { "RUUruRUrd", "URurd", "RUrd"};
		if (t == 4) {
			if(!single)
				yield return StartCoroutine (MiddleEdge (0));
			yield break;
		}
		int k = 0;
		for (k = 0; k < corner.Length; k++)
			if (corner [k].location == pieces [t])
				break;
		if (k == 0 || k == 1 || k == 2 || k == 3) {
			yield return StartCoroutine (StartRotate (highToZero [k]));
			yield return StartCoroutine (StartRotate (zeroToFinal [corner [0].state]));
			yield return StartCoroutine (BottomCorner (t + 1));
		} else if (k == 4 || k == 5 || k == 6 || k == 7) {
			yield return StartCoroutine (StartRotate (lowToHigh [k - 4]));
			yield return StartCoroutine (BottomCorner (t));
		}
		yield return null;
	}
		
	IEnumerator MiddleEdge(int t) {
		// Solve four edge pieces in the middle layer
		string[] pieces = new string[] { "FR", "FL", "BL", "BR" };
		string[] midToHigh = new string[] { "URurufUF", "UFufulUL", "ULulubUB", "UBuburUR" };
		string[,] highToZero = new string[4, 4] {
			{ "", "u", "uu", "U" },
			{ "U", "", "u", "uu" },
			{ "uu", "U", "", "u" },
			{ "u", "uu", "U", "" }
		};

		string[,] zeroToFinal = new string[4, 2] { 
			{ "uufUFURur", "URurufUF" },
			{ "UFufulUL", "uulULUFuf" },
			{ "uubUBULul", "ULulubUB" },
			{ "UBuburUR", "uurURUBub" }
		};

		if (t >= 4) {
			if(!single)
				yield return StartCoroutine (TopCross ());
			yield break;
		}
		int k = 0;
		for (k = 0; k < edge.Length; k++) {
			if (edge [k].location == pieces [t]) {
				break;
			}
		}
		if (k == 0 || k == 1 || k == 2 || k == 3) {
			yield return StartCoroutine (StartRotate (highToZero [t, k]));
			yield return StartCoroutine (StartRotate (zeroToFinal [t, edge[t].state]));
			yield return StartCoroutine (MiddleEdge (t + 1));
		} else if (k == 8 || k == 9 || k == 10 || k == 11) {
			yield return StartCoroutine (StartRotate (midToHigh [k - 8]));
			yield return StartCoroutine (MiddleEdge (t));
		}
		yield return null;
	}

	IEnumerator TopCross() {
		// Solve the yellow edges on the top facet
		string[] topCross = new string[] { "FRUruf", "BULulb" };
		int cnt = 0;
		for (int k = 0; k < 4; k++)
			if (edge [k].state == 0)
				cnt++;
		if (cnt == 4) {
			if(!single)
				yield return StartCoroutine (TopFace ());
			yield break;
		} else if (cnt == 2) {
			if (edge [1].state == 0 && edge [3].state == 0) {
				yield return StartCoroutine (StartRotate (topCross [0]));
			} else if (edge [0].state == 0 && edge [3].state == 0) {
				yield return StartCoroutine (StartRotate (topCross [1]));
			} else {
				yield return StartCoroutine (StartRotate ("u"));
			}
		} else if (cnt == 0) {
			yield return StartCoroutine (StartRotate (topCross [0]));
		}
		yield return StartCoroutine (TopCross ());
	}
		
	IEnumerator TopFace() {
		// Solve the yellow corners on the facet
		string[] topFace = new string[] { "rUURUrUR", "RuuruRur", "LFrflFRf", "fLFrflFR", "RRdRuurDRuuR", "RUUruRUruRur", "RuurruRRurrUUR" };
		int cnt = 0;
		for (int k = 0; k < 4; k++)
			if (corner [k].state == 0)
				cnt++;
		if (cnt == 4) {
			if(!single)
				yield return StartCoroutine (TopCorner ());
			yield break;
		}
		if (corner [0].state == 0 && corner [1].state == 2 && corner [2].state == 1 && corner [3].state == 2) {
			yield return StartCoroutine (StartRotate (topFace [0]));
		} else if (corner [0].state == 2 && corner [1].state == 1 && corner [2].state == 2 && corner [3].state == 0) {
			yield return StartCoroutine (StartRotate (topFace [1]));
		} else if (corner [0].state == 0 && corner [1].state == 1 && corner [2].state == 1 && corner [3].state == 0) {
			yield return StartCoroutine (StartRotate (topFace [2]));
		} else if (corner [0].state == 1 && corner [1].state == 0 && corner [2].state == 2 && corner [3].state == 0) {
			yield return StartCoroutine (StartRotate (topFace [3]));
		} else if (corner [0].state == 0 && corner [1].state == 0 && corner [2].state == 1 && corner [3].state == 1) {
			yield return StartCoroutine (StartRotate (topFace [4]));
		} else if (corner [0].state == 1 && corner [1].state == 1 && corner [2].state == 1 && corner [3].state == 1) {
			yield return StartCoroutine (StartRotate (topFace [5]));
		} else if (corner [0].state == 1 && corner [1].state == 2 && corner [2].state == 2 && corner [3].state == 1) {
			yield return StartCoroutine (StartRotate (topFace [6]));
		} else {
			yield return StartCoroutine (StartRotate ("u"));
		}
		yield return StartCoroutine (TopFace ());
	}

	IEnumerator TopCorner() {
		string[] pieces = new string[] { "UFR", "UFL", "UBL", "UBR" };
		string[] topCorner = new string[] {
			"",
			"u rrffrbRffrBr",
			"rrffrbRffrBr u",
			"UU rrffrbRffrBr UU",
			"U rrffrbRffrBr U",
			"RRUruBUbuBUbuBUbRuRR u"
		};
		while (corner [0].location != pieces [0])
			yield return StartCoroutine (StartRotate ("u"));
		int[] temp = new int[4];
		for (int ci = 0; ci < 4; ci++) {
			string s = corner [ci].location;
			for (int pi = 0; pi < 4; pi++) {
				if (s == pieces [pi]) {
					temp [ci] = pi;
					break;
				}
			}
		}
		int t = -1;
		if (temp [1] == 1 && temp [2] == 2 && temp [3] == 3) {
			t = 0;
		} else if (temp [1] == 1 && temp [2] == 3 && temp [3] == 2) {
			t = 1;
		} else if (temp [1] == 2 && temp [2] == 1 && temp [3] == 3) {
			t = 2;
		} else if (temp [1] == 2 && temp [2] == 3 && temp [3] == 1) {
			t = 3;
		} else if (temp [1] == 3 && temp [2] == 1 && temp [3] == 2) {
			t = 4;
		} else if (temp [1] == 3 && temp [2] == 2 && temp [3] == 1) {
			t = 5;
		}
		//yield return new WaitForSeconds (10.0f);
		yield return StartCoroutine (StartRotate (topCorner [t]));
		if(!single)
			yield return StartCoroutine (TopEdge ());
	}
		
	IEnumerator TopEdge() {
		// Orient the yellow edges on the top layer
		string[] pieces = new string[] { "UF", "UL", "UB", "UR" };
		string[] topEdge = new string[] { "RuRURURuruRR" };
		string[] elseToBack = new string[] { "UU", "U", "", "u" };
		string[] backToElse = new string[] { "uu", "u", "", "U" };
		int[] temp = new int[4];
		for (int ei = 0; ei < 4; ei++) {
			string s = edge [ei].location;
			for (int pi = 0; pi < 4; pi++) {
				if (s == pieces [pi]) {
					temp [ei] = pi;
					break;
				}
			}
		}
		int k = -1, cnt = 0;
		for (int i = 0; i < 4; i++) {
			if (edge [i].location == pieces [i]) {
				k = i;
				cnt++;
			}
		}
		if (cnt == 4) {
			yield break;
		}
		if (k != -1) {
			yield return StartCoroutine (StartRotate (elseToBack [k] + topEdge [0] + backToElse [k]));
		} else {
			yield return StartCoroutine (StartRotate (topEdge [0]));
		}
		yield return StartCoroutine (TopEdge ());
	}
		
	// debug
	public GameObject[] cornerObj = new GameObject[cornerCount];
	public string[] cornerLocation = new string[cornerCount];
	public int[] cornerState = new int[cornerCount];
	public GameObject[] edgeObj = new GameObject[edgeCount];
	public string[] edgeLoaction = new string[edgeCount];
	public int[] edgeState = new int[edgeCount];

	public string debugRotStr;

	void Update () {
		if (Input.GetKeyDown (KeyCode.K))
			single = single ? false : true;
		if (Input.GetKeyDown (KeyCode.A))
			StartCoroutine (RandomOrder());
		if (Input.GetKeyDown (KeyCode.Return))
			StartCoroutine (StartRotate(debugRotStr));
		if (Input.GetKeyDown (KeyCode.Alpha1))
			StartCoroutine (BottomCross (0));
		if (Input.GetKeyDown (KeyCode.Alpha2))
			StartCoroutine (BottomCorner (0));
		if (Input.GetKeyDown (KeyCode.Alpha3))
			StartCoroutine (MiddleEdge (0));
		if (Input.GetKeyDown (KeyCode.Alpha4)) 
			StartCoroutine (TopCross ());
		if (Input.GetKeyDown (KeyCode.Alpha5))
			StartCoroutine (TopFace ());
		if (Input.GetKeyDown (KeyCode.Alpha6))
			StartCoroutine (TopCorner ());
		if (Input.GetKeyDown (KeyCode.Alpha7))
			StartCoroutine (TopEdge ());
		//debug
		for(int k = 0; k < cornerCount; k++) {
			cornerObj [k] = corner [k].cube;
			cornerLocation [k] = corner [k].location;
			cornerState [k] = corner [k].state;
		}
		for (int k = 0; k < edgeCount; k++) {
			edgeObj [k] = edge [k].cube;
			edgeLoaction [k] = edge [k].location;
			edgeState [k] = edge [k].state;
		}
	}
}
