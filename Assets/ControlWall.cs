using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class ControlWall : MonoBehaviour {

	public int[,,] lu = new int[7,4,6]{
		{{ 0, 2, 0, 1, 0,-1}, {-1, 0, 1, 0, 2, 0}, { 0, 1, 0,-1, 0,-2}, {-2, 0,-1, 0, 1, 0}},
		{{ 0, 1, 0,-1, 1,-1}, {-1, 0, 1, 0,-1,-1}, {-1, 1, 0, 1, 0,-1}, {-1, 0, 1, 0, 1, 1}},
		{{ 0, 1,-1,-1, 0,-1}, {-1, 1,-1, 0, 1, 0}, { 0, 1, 1, 1, 0,-1}, {-1, 0, 1, 0, 1,-1}},
		{{ 0, 1,-1, 0, 1, 0}, { 0, 1, 1, 0, 0,-1}, {-1, 0, 1, 0, 0,-1}, { 0, 1,-1, 0, 0,-1}},
		{{ 0, 1, 1, 1, 1, 0}, { 0, 1, 1, 1, 1, 0}, { 0, 1, 1, 1, 1, 0}, { 0, 1, 1, 1, 1, 0}},
		{{ 0, 1, 1, 1,-1, 0}, { 0, 1, 1, 0, 1,-1}, { 1, 0,-1,-1, 0,-1}, {-1, 1,-1, 0, 0,-1}},
		{{-1, 1, 0, 1, 1, 0}, { 1, 1, 1, 0, 0,-1}, {-1, 0, 0,-1, 1,-1}, { 0, 1,-1, 0,-1,-1}}
	};

	public static int COLUMNS = 64;
	public static int ROWS = 24;
	public float steptime = 0.3F; // seconds
	public float currentsteptime = 0.0F; // seconds
	public int playerBlockId = 0;
	public int colorId = 0;
	public int playerOrient = 0;
	public int movespeed = 1;
	public int score = 0;
	public int[] playerBlockLocation = new int[2]{0, 0};
	public Block[,] blocks;
	public Transform blockPrefab;
	public int[] posx = new int[4];
	public int[] posy = new int[4];
	private Color[] colors = new Color[4]{
		new Color(75.0F/255.0F, 240.0F/255.0F, 252.0F/255.0F, 1.0F), 
		new Color(254.0F/255.0F, 229.0F/255.0F, 49.0F/255.0F, 1.0F),
		new Color(8.0F/255.0F, 79.0F/255.0F, 0.0F, 1.0F),
		new Color(255.0F/255.0F, 66.0F/255.0F, 80.0F/255.0F, 1.0F)
	};
	
	void Start () {
		float r = ((32.0F /Mathf.PI));
		float theta = 0.0F;
		float dT = (2*Mathf.PI)/64;
		blocks = new Block[COLUMNS, ROWS];
		for (int i = 0; i < COLUMNS; i++) {
			for (int j = 0; j < ROWS; j++) {
				float x = r * Mathf.Cos (theta);
				float y = r * Mathf.Sin (theta);
				Transform bl = (Transform) Instantiate(blockPrefab, new Vector3(x, j, y), Quaternion.identity);
				bl.eulerAngles = new Vector3(0, -theta * (180 / Mathf.PI), 0);
				blocks[i,j] = bl.gameObject.GetComponent<Block>();
				blocks[i,j].SetState("empty");
			}
			theta += dT;
		}

		InitiateNewPlayerBlock ();

		posx [0] = playerBlockLocation [0]; 
		posx [1] = playerBlockLocation [0] + lu [playerBlockId, playerOrient, 0];
		posx [2] = playerBlockLocation [0] + lu [playerBlockId, playerOrient, 2];
		posx [3] = playerBlockLocation [0] + lu [playerBlockId, playerOrient, 4]; 

		posy [0] = playerBlockLocation [1]; 
		posy [1] = playerBlockLocation [1] + lu [playerBlockId, playerOrient, 1];
		posy [2] = playerBlockLocation [1] + lu [playerBlockId, playerOrient, 3];
		posy [3] = playerBlockLocation [1] + lu [playerBlockId, playerOrient, 5];
	}

	public void ChangePlayerBlocks(int dx, int dy){
		for (int index = 0; index < 4; index ++) {
			posx [index] += dx;
			posy [index] += dy;
		}

		playerBlockLocation [0] += dx;
		playerBlockLocation [1] += dy;
	}

	void MovePlayerBlocks(){
		int x_t = 0;
		int y_t = 0;
		for (int x = 0; x < COLUMNS; x++) {
			for (int y = 0; y < ROWS; y++) {
				if(blocks[x, y].GetState() == "active"){
					blocks[x, y].SetState("empty");
					x_t = x;
					y_t = y;
				}
			}
		}

		for (int x = 0; x < 4; x++){
			blocks [posx [x], posy [x]].SetColor (blocks [x_t, y_t].GetColor ());
			blocks[posx[x], posy[x]].SetState("active");
		}
	}

	void InitiateNewPlayerBlock(){
		playerOrient = 1;
		playerBlockId = (int)Mathf.Floor(Random.Range(0, 7));
//		colorId = (int)Mathf.Floor(Random.Range(0, 3));
		colorId = 3;
		playerBlockLocation [0] = 8;
		playerBlockLocation [1] = 22;

		posx [0] = playerBlockLocation [0]; 
		posx [1] = playerBlockLocation [0] + lu [playerBlockId, playerOrient, 0];
		posx [2] = playerBlockLocation [0] + lu [playerBlockId, playerOrient, 2];
		posx [3] = playerBlockLocation [0] + lu [playerBlockId, playerOrient, 4]; 
		
		posy [0] = playerBlockLocation [1];
		posy [1] = playerBlockLocation [1] + lu [playerBlockId, playerOrient, 1];
		posy [2] = playerBlockLocation [1] + lu [playerBlockId, playerOrient, 3];
		posy [3] = playerBlockLocation [1] + lu [playerBlockId, playerOrient, 5];
		
		for (int x = 0; x < 4; x++){
			blocks[posx[x], posy[x]].SetColor(colors[colorId]);
			blocks[posx[x], posy[x]].SetState("active");
		}
	}

	void FixBlocks(int[] posx, int[] posy){
		for (int x = 0; x < 4; x++){
			blocks[posx[x], posy[x]].SetColor(colors[colorId]);
			blocks[posx[x], posy[x]].SetState("filled");
		}
	}
		
	void UpdatePlayerVertically(){
		for (int index = 0; index < 4; index++){
			if (posy[index] - 1 < 0){
				FixBlocks(posx, posy);
				InitiateNewPlayerBlock();
				return;
			} else if(blocks[posx[index], posy[index] - 1].GetState() == "filled") {
				FixBlocks(posx, posy);
				InitiateNewPlayerBlock();
				return;
			} 
		}
		ChangePlayerBlocks (0, -1);
		MovePlayerBlocks();
	}

	void UpdatePlayerHorizontally(int direction){
		for (int index = 0; index < 4; index++){
			if (posx[index] + direction > 15 || posx[index] + direction < 0) {
				return;
			}
			if(blocks[posx[index] + direction, posy[index]].GetState() == "filled") {
				return;
			}
		}
		ChangePlayerBlocks (direction, 0);
		MovePlayerBlocks();
	}

	int[] NeedsToChangeBy(int i, int j, int blockid, int orient){
		int[] moveby = new int[]{0, 0};

		if (posx.Min() < 0) {
			if(DoesItFit(i + 1, j, playerBlockId, playerOrient)) {
				moveby[0] = -posx.Min();
				return moveby;
			}
		} else if (posx.Max () > 15) {
			if(DoesItFit(i - (15 - posx.Max()), j, playerBlockId, playerOrient)) {
				moveby[0] = 15 - posx.Max();
				return moveby;
			}
		}

		if (posy.Min() < 0) {
			if(DoesItFit(i - (15 - posx.Max()), j, playerBlockId, playerOrient)) {
				moveby[1] = -posx.Min();
				return moveby;
			}
		}

		if (playerBlockId != 0) {
			for (int x = 1; x < 4; x++) {
				if (blocks [posx [x], posy [x]].GetState () == "filled") {
					if (posx [x] > posx [0]) {
						if (DoesItFit (i - 1, j, playerBlockId, playerOrient)) {
							moveby [0] = -1;
						}
					} else if (posx [x] < posx [0]) {
						if (DoesItFit (i + 1, j, playerBlockId, playerOrient)) {
							moveby [0] = 1;
						}
					} else if (posy [x] < posx [1]) {
						if (DoesItFit (i, j + 1, playerBlockId, playerOrient)) {
							moveby [1] = 1;
						}
					}
				}
			}
		} else {
			for (int x = 0; x < 4; x++) {
				if (blocks [posx [x], posy [x]].GetState () == "filled") {
					if (posx [x] > posx [0]) {
						if (DoesItFit (i - 1, j, playerBlockId, playerOrient)) {
							moveby [0] = -1;
						}
					} else if (posx [x] < posx [0]) {
						if (DoesItFit (i + 1, j, playerBlockId, playerOrient)) {
							moveby [0] = 1;
						}
					} else if (posy [x] < posx [1]) {
						if (DoesItFit (i, j + 1, playerBlockId, playerOrient)) {
							moveby [1] = 1;
						}
					}
				}
			}
		}
		return moveby;
	}

	bool DoesItFit(int i, int j, int blockid, int orient){
		int[] testposx = new int[4] {i, 
			i + lu[playerBlockId, orient, 0], 
			i + lu[playerBlockId, orient, 2], 
			i + lu[playerBlockId, orient, 4]};
		
		int[] testposy = new int[4]{ j,
			j + lu[playerBlockId, orient, 1],
			j + lu[playerBlockId, orient, 3],
			j + lu[playerBlockId, orient, 5]};
		
		if (testposx.Min () < 0) {
			return false;
		} else if (testposx.Max () > COLUMNS-1) {
			return false;
		} else {
			for (int index = 0; index < 4; index++){
				if(blocks[testposx[index], testposy[index]].GetState() == "filled"){
					return false;
				} 
			}
		}

		return true;
	}
	
	public void RotatePlayer(int direction){

		int testOrient = playerOrient + direction;
		if (testOrient > 3) {
			testOrient = 0;
		} else if (testOrient < 0) {
			testOrient = 3;
		}

		if (DoesItFit(playerBlockLocation [0], playerBlockLocation [1], playerBlockId, testOrient)) {
			playerOrient = testOrient;
		} else {
			int[] moveby = NeedsToChangeBy(playerBlockLocation [0], playerBlockLocation [1], playerBlockId, testOrient);
			if (moveby[0] == 0 && moveby[0] == 0){
				return;
			} else {
				playerBlockLocation[0] += moveby[0];
				playerBlockLocation[1] += moveby[1];
				playerOrient = testOrient;
			}
		}

		posx [0] = playerBlockLocation [0]; 
		posx [1] = playerBlockLocation [0] + lu [playerBlockId, playerOrient, 0];
		posx [2] = playerBlockLocation [0] + lu [playerBlockId, playerOrient, 2];
		posx [3] = playerBlockLocation [0] + lu [playerBlockId, playerOrient, 4]; 
		
		posy [0] = playerBlockLocation [1]; 
		posy [1] = playerBlockLocation [1] + lu [playerBlockId, playerOrient, 1];
		posy [2] = playerBlockLocation [1] + lu [playerBlockId, playerOrient, 3];
		posy [3] = playerBlockLocation [1] + lu [playerBlockId, playerOrient, 5];
		
		MovePlayerBlocks();
	}

	int[] CheckForCompletedRows(){
		List<int> rows = new List<int>();
	
		for (int j = 0; j < ROWS; j++) {
			for (int i = 0; i < COLUMNS; i++){
				if (blocks[i,j].GetState() != "filled"){
					break;
				} else if (i == 15) {
					rows.Add(j);
				}
			}
		}

		int[] comprows = rows.ToArray();
		return comprows;
	}

	void UpdateScore(int numrows){
		for (int i = 0; i < numrows; i++) {
			score += 100 * i;
		}
	}

	void EmptyRow(int rownum){
		for (int i = 0; i < COLUMNS; i++) {
			blocks[i, rownum].SetState("empty");
		}
	}

	int NumRowsGreater(int i, int[] rows){
		int nrows = 0;
		for (int index = 0; index < rows.Length; index++) {
			if(i > rows[index]){
				nrows++;
			}
		}
		return nrows;
	}

	void UpdateGrid(int[] rows){
		Debug.Log (rows);
		for (int row = 0; row < rows.Length; row++) {
			EmptyRow(rows[row]);
		}
	
		for (int j = 1; j < COLUMNS; j++) {
			if (!rows.Contains(j)){
				for (int i = 0; i < ROWS; i++){
					int nrg = NumRowsGreater(j, rows);
					if(blocks[i, j].GetState() != "active"){
						blocks[i, j-nrg].SetColor(blocks[i, j].GetColor());
						blocks[i, j-nrg].SetState(blocks[i, j].GetState());
						blocks[i, j].SetState("empty");
					}
				}
			}
		}
	}

	public void Slam(){
		bool slammed = false;
		while (!slammed) {
			if(DoesItFit(playerBlockLocation [0], playerBlockLocation [1] - 1, playerBlockId, playerOrient)) {
					ChangePlayerBlocks(0, -1);
			} else {
				FixBlocks(posx, posy);
				InitiateNewPlayerBlock();
				slammed = true;
			}
		}
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			Slam();
		} else {
			if (Input.GetKeyDown(KeyCode.RightArrow)) {
				UpdatePlayerHorizontally(1);
			}

			else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
				UpdatePlayerHorizontally(-1);
			}

			if (Input.GetKeyDown(KeyCode.UpArrow)) {
				RotatePlayer(1);
			}

			else if (Input.GetKeyDown(KeyCode.DownArrow)) {
				RotatePlayer(-1);
			}
		}
		
		currentsteptime += Time.deltaTime;

		if (currentsteptime >= steptime) {
			currentsteptime = 0.0F;
			UpdatePlayerVertically();
		}

		int[] comprows = CheckForCompletedRows();
		if (comprows.Length > 0) {
			UpdateScore(comprows.Length);
			UpdateGrid(comprows);
		}
	}
}
