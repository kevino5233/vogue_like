using UnityEngine;
using System;
using System.Collections;

public class LevelGenScript : MonoBehaviour {
	
	const int SMALL = 0;
	const int MEDIUM = 1;
	const int LARGE = 2;
	
	const int WIDTH = 0;
	const int HEIGHT = 1;
	
	const int OPEN = 0;
	const int BLOCKED = 1;
	const int L_WALL = 2;
	const int R_WALL = 3;
	const int CEILING = 4;
	const int FLOOR = 5;
	const int TORCH = 6;
	const int WINDOW = 7;
	const int START = 8;
	const int END = 9;
	
	int[][] SIZES = new int[3][];
	
	string str_seed = "";
	
	Texture2D shirt;
	
	Texture2D[] blocks;
	
	void CreateRoom(int[][] map, int x, int y, int w, int h){
		int x1 = x;
		int x2 = x + w;
		int y1 = y;
		int y2 = y + h;
		for (int j = y1; j < y2; j++){
			for (int i = x1; i < x2; i++){
				int tile_x = i % map.Length;
				int tile_y = j;
				map[tile_y][tile_x] = OPEN;
			}
		}
	}
	
	void CreateVCorridor(int[][] map, int y1, int y2, int x){
		for (int j = y1; j <= y2; j++){
			map[j][x % map[j].Length] = OPEN;
		}
	}
	
	void CreateHCorridor(int[][] map, int x1, int x2, int y){
		for (int i = 0; i <= Mathf.Abs(x1 - x2); i++){
			int tile_x = (i + x1) % map[y].Length;
			map[y][tile_x] = OPEN;
		}
	}
	
	void ConnectRooms(int[][] map, int[] room1, int[] room2, int flag){
		int room1_x_center = (room1[0] + room1[1]) / 2;
		int room1_y_center = (room1[2] + room1[3]) / 2;
		int room2_x_center = (room2[0] + room2[1]) / 2;
		int room2_y_center = (room2[2] + room2[3]) / 2;
		if (flag == 1){
			CreateHCorridor(map, room2_x_center, room1_x_center, room2_y_center);
			CreateVCorridor(map, Mathf.Min(room1_y_center, room2_y_center), Mathf.Max(room1_y_center, room2_y_center), room1_x_center);
		} else {
			CreateHCorridor(map, room1_x_center, room2_x_center, room1_y_center);
			CreateVCorridor(map, Mathf.Min(room1_y_center, room2_y_center), Mathf.Max(room1_y_center, room2_y_center), room2_x_center);
		}
	}
		
	bool intersects(int[] room1, int[] room2){
		return (room1[0] <= room2[1] && room1[1] >= room2[0] && room1[2] <= room2[3] && room2[3] >= room2[2]);
	}
	
	void generateNewLevel(int seed, int size){
		System.Random rand = new System.Random();
		if (seed != 0){
			rand = new System.Random(seed);
		}
		Debug.Log(rand.Next());
		int width = SIZES[size][WIDTH];
		int height = SIZES[size][HEIGHT];
		int area = width * height;
		int sqrt_area = (int)(Mathf.Sqrt(area));
		int num_rooms = rand.Next((int)(sqrt_area * .75), (int)(sqrt_area  * 1.25));
		int[][] map = new int[height][];
		for (int j = 0; j < height; j++){
			map[j] = new int[width];
			for (int i = 0; i < width; i++){
				map[j][i] = BLOCKED;
			}
		}
		int[][] rooms = new int[num_rooms][];
		for (int i = 0; i < num_rooms; i++){
			rooms[i] = new int[4];
		}
		//x1 = room[0]
		//x2 = room[1]
		//y1 = room[2]
		//y2 = room[3]
		for (int i = 0; i < num_rooms; i++){
			int x = rand.Next(width);
			int y = rand.Next(1, height);
			int w = rand.Next(3, 6);
			int h = rand.Next(3, 6);
			if (y + h > height - 1){
				h = height - y - 1;
			}
			CreateRoom(map, x, y, w, h);
			rooms[i][0] = x;
			rooms[i][1] = x + w;
			rooms[i][2] = y;
			rooms[i][3] = y + h;
		}
		for (int i = 0; i < num_rooms; i++){
			bool connected = false;
			for (int j = 0; j < num_rooms; j++){
				if (i != j) connected = connected || intersects(rooms[j], rooms[i]);
				if (i == 0 && j == num_rooms - 1) connected = false;
			}
			if (!connected){
				int j = rand.Next(num_rooms);
				while (j == i){
					j = rand.Next(num_rooms);
				}
				ConnectRooms(map, rooms[i], rooms[j], rand.Next(2));
			}
		}
		int startX = (rooms[0][0] + rooms[0][1]) / 2;
		int startY = rooms[0][3];
		/*while (startY < height - 2 && map[startY + 1][startX % width] == OPEN){
			Debug.Log(startY < height - 2);
			Debug.Log(map[startY + 1][startX % width] == OPEN);
			startY++; 
		}*/
		Debug.Log(startX + ", " + startY);
		int endX = (rooms[num_rooms - 1][0] + rooms[num_rooms - 1][1]) / 2;
		int endY = rooms[num_rooms - 1][3];
		/*while (endY < height - 2 && map[endY + 1][endX % width] == OPEN){
			Debug.Log(endY < height - 2);
			Debug.Log(map[endY + 1][endX % width] == OPEN);
			endY++; 
		}*/
		Debug.Log(endX + " " + endY);
		shirt = new Texture2D(width * 30, height * 30);
		string s = "";
		Color[] openPixels = blocks[OPEN].GetPixels();
		Color[] blockedPixels = blocks[BLOCKED].GetPixels();
		for (int j = 0; j < height; j++){
			for (int i = 0; i < width; i++){
				blocks[OPEN].SetPixels(openPixels);
				blocks[BLOCKED].SetPixels(blockedPixels);
				Texture2D block = blocks[map[j][i]];
				/*not going in until I figure out what the fuck is going on
				if (i == startX && j == startY){
					Debug.Log("start " + i + " " + j);
					Color[] startPixels = blocks[START].GetPixels();
					block.SetPixels(startPixels);
					block.Apply();
				} else if (i == endX && j == endY){
					Debug.Log("end " + i + " " + j);
					Color[] endPixels = blocks[END].GetPixels();
					block.SetPixels(endPixels);
					block.Apply();
				} else */if (map[j][i] == BLOCKED){
					if (i != 0 && map[j][i - 1] != BLOCKED){
						Texture2D wall = blocks[L_WALL];
						Color[] wallPixels = wall.GetPixels(0, 0, 5, 30);
						block.SetPixels(0, 0, 5, 30, wallPixels);
						block.Apply();
					}
					if (i != width - 1 && map[j][i + 1] != BLOCKED){
						Texture2D wall = blocks[R_WALL];
						Color[] wallPixels = wall.GetPixels(25, 0, 5, 30);
						block.SetPixels(25, 0, 5, 30, wallPixels);
						block.Apply();
					}
					if (j != 0 && map[j - 1][i] != BLOCKED){
						Texture2D wall = blocks[FLOOR];
						Color[] wallPixels = wall.GetPixels(0, 0, 30, 5);
						block.SetPixels(0, 0, 30, 5, wallPixels);
						block.Apply();
					}
					if (j != height - 1 && map[j + 1][i] != BLOCKED){
						Texture2D wall = blocks[CEILING];
						Color[] wallPixels = wall.GetPixels(0, 25, 30, 5);
						block.SetPixels(0, 25, 30, 5, wallPixels);
						block.Apply();
					}
				} else {
					bool spaced = true;
					for (int b = -2; b < 2; b++){
						if (j + b < 0 || j + b > height - 1) continue;
						for (int a = -2; a < 2; a++){
							if (i + a < 0 || i + a > width - 1) continue;
							if (map[j + b][i + a] == TORCH || map[j + b][i + a] == WINDOW){
								spaced = false;
								break;
							}
						}
					}
					if (spaced && rand.Next(100) < 10){
						if (rand.Next(4) < 2){
							Texture2D torch = blocks[TORCH];
							Color[] torchPixels = torch.GetPixels();
							block.SetPixels(torchPixels);
							map[j][i] = TORCH;
						} else {
							Texture2D window = blocks[WINDOW];
							Color[] windowPixels = window.GetPixels();
							block.SetPixels(windowPixels);
							map[j][i] = WINDOW;
						}
					}
				}
				Color[] blockPixels = block.GetPixels();
				shirt.SetPixels(i * 30, j * 30, 30, 30, blockPixels);
				s += map[j][i];
			}
			s += "\n";
		}
		shirt.Apply();
		this.renderer.material.mainTexture = shirt;
		//Sprite shirt_sprite = Sprite.Create(shirt, new Rect(0, 0, width * 30, height * 30), new Vector2(0, 0), 1.0f);
		//test.GetComponent<SpriteRenderer>().sprite = shirt_sprite;
		
		Debug.Log(s);
	}

	// Use this for initialization
	void Start () {
		blocks = new Texture2D[10] {Resources.Load<Texture2D>("block_0"), 
									Resources.Load<Texture2D>("block_1"),
									Resources.Load<Texture2D>("block_2"),
									Resources.Load<Texture2D>("block_3"),
									Resources.Load<Texture2D>("block_4"),
									Resources.Load<Texture2D>("block_5"),
									Resources.Load<Texture2D>("block_6"),
									Resources.Load<Texture2D>("block_7"),
									Resources.Load<Texture2D>("block_8"),
									Resources.Load<Texture2D>("block_9")};
		SIZES[SMALL] = new int[2] {33, 26};
		SIZES[MEDIUM] = new int[2] {35, 28};
		SIZES[LARGE] = new int[2] {38, 29};
		generateNewLevel(0, MEDIUM);
	}
	
	void OnGUI() {
		int s_width = Screen.width;
		int s_height = Screen.height;
		
		GUI.BeginGroup(new Rect(s_width - 150, s_height - 150, 100, 100));
		GUI.Label(new Rect(0, 0, 100, 20), "Set random");
		GUI.Label(new Rect(0, 20, 100, 20), "seed.");
		str_seed = GUI.TextField(new Rect(0, 40, 100, 20), str_seed);
		if (GUI.Button(new Rect(0, 60, 100, 20), "New outfit")){
			int seed;
			if (Int32.TryParse(str_seed, out seed)){
				generateNewLevel(seed, MEDIUM);
			} else {
				generateNewLevel(0, MEDIUM);
			}
		}
		GUI.EndGroup();
	}
}
