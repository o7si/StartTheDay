using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeSearchDFSAndBFS : MonoBehaviour {

	public struct Point {
		public int x;
		public int y;
		public string route;
		public Point(int x, int y, string route) {
			this.x = x;
			this.y = y;
			this.route = route;
		}
	};

	// 許して最大範囲
	const int MAXX = 50;
	const int MAXY = 50;
	char[,] mazeMap = new char[MAXX, MAXY];
	Point start, end;
	// 今の範囲
	int maxX;
	int maxY;
	// 方向
	const string dir = "RDLT";
	int[] dx = new int[] { 0, -1, 0, 1 };
	int[] dy = new int[] { -1, 0, 1, 0 };
	//地図を初期化する
	public void setChMazeMap(int x, int y) {
		// 地図を取る
		this.maxX = x;
		this.maxY = y;
		for (int i = 0; i < maxX; i++) {
			for (int j = 0; j < maxY; j++) {
				mazeMap [i, j] = transform.GetComponent<CityMazeGenerate> ().mazeMap [i, j].state;
				//Debug.Log (i + ", " + j + ", " + mazeMap[i, j]);
				if (mazeMap [i, j] == 'S') {
					start = new Point (i, j, "");
				} else if (mazeMap [i, j] == 'E') {
					end = new Point (i, j, "");
				}
			}
		}
	}
	// BFSAlgorithm
	public string BFS() {
		Queue<Point> que = new Queue<Point> ();
		que.Enqueue (start);
		mazeMap[start.x, start.y] = '#';
		while (que.Count > 0) {
			Point temp = que.Dequeue ();
			if (temp.x == end.x && temp.y == end.y)
				return temp.route;
			for (int k = 0; k < 4; k++) {
				int x = temp.x + dx [k];
				int y = temp.y + dy [k];
				if (x < 0 || x >= maxX || y < 0 || y >= maxY || mazeMap [x, y] == '#')
					continue;
				mazeMap[x, y] = '#';
				que.Enqueue (new Point (x, y, temp.route + dir [k]));
			}
		}
		return null;
	}
	// ルートを記録する
	Stack<Point> stack = new Stack<Point>();
	Stack<Point> min = new Stack<Point> ();
	int length = MAXX * MAXY;
	// DFSAlgorithm
	void _DFS(Point point) {
		mazeMap[point.x, point.y] = '#';
		stack.Push (point);
		if (point.x == end.x && point.y == end.y) {
			if (stack.Count < length) {
				// こうすればStackが倒置している
				min = new Stack<Point>(stack);
				length = stack.Count;
			}
			mazeMap [point.x, point.y] = '.';
		} else {
			for (int k = 0; k < 4; k++) {
				int x = point.x + dx [k];
				int y = point.y + dy [k];
				if (x < 0 || x >= maxX || y < 0 || y >= maxY || mazeMap [x, y] == '#')
					continue;
				_DFS (new Point(x, y, ""));
			}
		}
		stack.Pop ();
		mazeMap[point.x, point.y] = '.';
	}
	// DFSAlgorithmMain
	public string DFS() {
		_DFS (start);
		string path = "";
		Point p1 = min.Pop ();
		while (min.Count != 0) {
			Point p2 = min.Pop ();
			if (p2.x == p1.x && p2.y < p1.y) {
				path += 'R';
			} else if (p2.x == p1.x && p2.y > p1.y) {
				path += 'L';
			} else if (p2.x < p1.x && p2.y == p1.y) {
				path += 'D';
			} else if (p2.x > p1.x && p2.y == p1.y) {
				path += 'T';
			}
			p1 = p2;
		}
		return path;
	}
	// Debug
	string ToString(Point point) {
		return "(" + point.x + "," + point.y + ")";
	}
}
