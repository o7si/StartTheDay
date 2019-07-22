using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeSearchAStar : MonoBehaviour {

	public struct AStartPoint {
		public int x, y;
		public int fatherX, fatherY;
		public int fScore, gScore, hScore;
		public char state;
		public bool isClose;
		public bool isOpen;
	};

	public struct Point {
		public int x;
		public int y;
		public Point(int x, int y) {
			this.x = x;
			this.y = y;
		}
	};

	public class MinHeap {
		private AStartPoint[] data;
		private int count;
		private int capacity;
		private void Swap<T> (ref T a, ref T b) { T t = a; a = b; b = t; }
		private void shiftUp(int k) {
			while (k > 1 && data [k / 2].fScore > data [k].fScore) {
				Swap (ref data[k / 2], ref data[k]);
				k /= 2;
			}
		}
		private void shiftDown(int k) {
			while(2 * k <= count) {
				int j = 2 * k;
				if(j + 1 <= count && data[j + 1].fScore < data[j].fScore)
					j ++;
				if(data[k].fScore <= data[j].fScore) 
					break;
				Swap(ref data[k], ref data[j]);
			}
		}
		public MinHeap(int capacity) {
			data = new AStartPoint[capacity + 1];
			this.count = 0;
			this.capacity = capacity;
		}
		public bool isEmpty() {
			return count == 0;
		}
		public void insert(AStartPoint elem) {
			data [count + 1] = elem;
			this.count++;
			shiftUp (count);
		}
		public AStartPoint getTop() {
			AStartPoint result = data [1];
			Swap (ref data [1], ref data [count]);
			this.count--;
			shiftDown (1);
			return result;
		}
	};

	// 許して最大範囲
	const int MAXX = 50;
	const int MAXY = 50;
	AStartPoint[,] mazeMap = new AStartPoint[MAXX, MAXY];
	AStartPoint start, end;
	// 前の範囲
	int maxX;
	int maxY;
	// 方向
	int[] dx = new int[] { 0, -1, 0, 1 };
	int[] dy = new int[] { -1, 0, 1, 0 };
	//地図を初期化する
	public void initialAStarMap(int x, int y) {
		this.maxX = x;
		this.maxY = y;
		for (int i = 0; i < maxX; i++) {
			for (int j = 0; j < maxY; j++) {
				mazeMap [i, j].x = i;
				mazeMap [i, j].y = j;
				mazeMap [i, j].gScore = 0;
				mazeMap [i, j].hScore = 0;
				mazeMap [i, j].fScore = 0 + 0;
				mazeMap [i, j].isOpen = false;
				mazeMap [i, j].isClose = false;
				mazeMap[i, j].state = transform.GetComponent<CityMazeGenerate> ().mazeMap [i, j].state;
				if (mazeMap [i, j].state == 'S')
					start = mazeMap [i, j];
				if (mazeMap [i, j].state == 'E')
					end = mazeMap [i, j];
			}
		}
		for (int i = 0; i < maxX; i++) {
			for (int j = 0; j < maxY; j++) {
				double dis = Mathf.Sqrt (Mathf.Pow (i - end.x, 2) + Mathf.Pow (j - end.y, 2));
				mazeMap [i, j].gScore = (int)(dis * 10);
				mazeMap [i, j].fScore = mazeMap [i, j].gScore + mazeMap [i, j].hScore;
			}
		}
	}
	// AStarAlgorithm
	public string AStar() {
		Stack<Point> stack = new Stack<Point>();
		MinHeap table = new MinHeap(maxX * maxY);
		mazeMap [start.x, start.y].isOpen = true;
		table.insert (mazeMap [start.x, start.y]);
		while (!table.isEmpty ()) {
			AStartPoint temp = table.getTop ();
			mazeMap [temp.x, temp.y].isClose = true;

			if (temp.x == end.x && temp.y == end.y) {
				int imaX = end.x;
				int imaY = end.y; 
				while (!(imaX == start.x && imaY == start.y)) {
					stack.Push (new Point (imaX, imaY));
					int ta = mazeMap [imaX, imaY].fatherX;
					int tb = mazeMap [imaX, imaY].fatherY;
					imaX = ta;
					imaY = tb;
				}
				break;
			}
			for (int k = 0; k < 4; k++) {
				int x = temp.x + dx [k];
				int y = temp.y + dy [k];
				if (x < 0 || x >= maxX || y < 0 || y >= maxY || mazeMap [x, y].state == '#' || mazeMap [x, y].isClose)
					continue;
				if (mazeMap [x, y].isOpen) {
					if (mazeMap [temp.x, temp.y].hScore + 10 < mazeMap[x, y].hScore) {
						mazeMap [x, y].fatherX = temp.x;
						mazeMap [x, y].fatherY = temp.y;
					}
				} else {
					mazeMap [x, y].fatherX = temp.x;
					mazeMap [x, y].fatherY = temp.y;
				}
				mazeMap [x, y].hScore = mazeMap [temp.x, temp.y].hScore + 10;
				table.insert (mazeMap [x, y]);
			}
		}
		string path = "";
		Point temp1 = new Point (start.x, start.y);
		while (stack.Count != 0) {
			Point temp2 = stack.Pop ();
			if (temp2.x == temp1.x && temp2.y < temp1.y) {
				path += 'R';
			} else if (temp2.x == temp1.x && temp2.y > temp1.y) {
				path += 'L';
			} else if (temp2.x < temp1.x && temp2.y == temp1.y) {
				path += 'D';
			} else if (temp2.x > temp1.x && temp2.y == temp1.y) {
				path += 'T';
			}
			temp1 = temp2;
		}
		return path;
	}
}
