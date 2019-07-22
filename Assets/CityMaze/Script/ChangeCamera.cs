using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamera : MonoBehaviour {

	public struct CameraGroup {
		public Camera c1;
		public Camera c2;
	}; 

	string[,] strCamera = new string[,] {
		{ "CarDFS/C1", "CarDFS/C2" },
		{ "CarBFS/C1", "CarBFS/C2" },
		{ "CarAStar/C1", "CarAStar/C2" }
	};

	const int num = 3;
	CameraGroup[] cg = new CameraGroup[num];

	int v = 0;
	int car = 1;

	public void setCamera() {
		for (int i = 0; i < strCamera.GetLength (0); i++) {
			cg [i].c1 = GameObject.Find (strCamera [i, 0]).GetComponent<Camera>();
			cg [i].c2 = GameObject.Find (strCamera [i, 1]).GetComponent<Camera>();
		}
	}

	void closeCamera(int k) {
		k--;
		for (int i = 0; i < num; i++) {
			if (i == k) {
				if (v == 0) {
					cg [i].c1.enabled = true;
					cg [i].c2.enabled = false;
				} else {
					cg [i].c1.enabled = false;
					cg [i].c2.enabled = true;
				}
			} else {
				cg [i].c1.enabled = false;
				cg [i].c2.enabled = false;
			}
		}
	}

	void changeCamera(ref CameraGroup c) {
		v = v == 0 ? 1 : 0;
		if (c.c1.enabled) {
			c.c1.enabled = false;
			c.c2.enabled = true;
		} else {
			c.c1.enabled = true;
			c.c2.enabled = false;
		}
	}

	public Camera a;
	public Camera b;
	public Camera c;
	public Camera d;
	public Camera e;
	public Camera f;

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			car = 1;
			closeCamera (1);
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			car = 2;
			closeCamera (2);
		}
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			car = 3;
			closeCamera (3);
		}
		if (Input.GetKeyDown (KeyCode.V)) {
			changeCamera (ref cg [car - 1]);
		}
		a = cg [0].c1;
		b = cg [0].c2;
		c = cg [1].c1;
		d = cg [1].c2;
		e = cg [2].c1;
		f = cg [2].c2;
	}
}
