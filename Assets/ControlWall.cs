using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class ControlWall : MonoBehaviour {

	public int[,,] lu = new int[7,4,6]{
		{{ 0, 2, 0, 1, 0,-1}, {-1, 0, 1, 0, 2, 0}, { 0, 1, 0,-1, 0,-2}, {-2, 0,-1, 0, 1, 0}},
		{{ 0, 1, 0,-1, 1,-1}, {-1, 0, 1, 0,-1,-1}, {-1, 1, 0, 1, 0,-1}, {-1, 0, 1, 0, 1, 1}},
		{{ 0, 1,-1,-1, 0, 1}, {-1, 1,-1, 0, 1, 0}, { 0, 1, 1, 1, 0,-1}, {-1, 0, 1, 0, 1,-1}},
		{{ 0, 1,-1, 0, 1, 0}, { 0, 1, 1, 0, 0,-1}, {-1, 0, 1, 0, 0,-1}, { 0, 1,-1, 0, 0,-1}},
		{{ 0, 1, 1, 1, 1, 0}, { 0, 1, 1, 1, 1, 0}, { 0, 1, 1, 1, 1, 0}, { 0, 1, 1, 1, 1, 0}},
		{{ 0, 1, 1, 1,-1, 0}, { 0, 1, 1, 0, 1,-1}, { 1, 0,-1,-1, 0,-1}, {-1, 1,-1, 0, 0,-1}},
		{{-1, 1, 0, 1, 1, 0}, { 1, 1, 1, 0, 0,-1}, {-1, 0, 0,-1, 1,-1}, { 0, 1,-1, 0,-1,-1}}
	};

	public float steptime = 0.5F; // seconds
	public float currentsteptime = 0.0F; // seconds
	public int playerBlockId = 0;
	public int playerOrient = 0;
	public int movespeed = 1;
	public int score = 0;
	public int[] playerBlockLocation = new int[2]{0, 0};
	public Block[,] blocks;
	public Transform blockPrefab;
	public int[] posx = new int[4];
	public int[] posy = new int[4];
	
	void Start () {
		blocks = new Block[16, 32];
		for (int i = 0; i < 16; i++) {
			for (int j = 0; j < 32; j++) {
				Transform bl = (Transform) Instantiate(blockPrefab, new Vector3(-8 + i, j, 0), Quaternion.identity);
				blocks[i,j] = bl.gameObject.GetComponent<Block>();
				blocks[i,j].SetState("empty");
			}
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
	}

	void InitiateNewPlayerBlock(){
		playerOrient = 1;
		playerBlockId = (int)Mathf.Floor(Random.Range(0, 7));
		playerBlockLocation [0] = 8;
		playerBlockLocation [1] = 30;

		int i = playerBlockLocation [0];
		int j = playerBlockLocation [1];

		int[] posx = new int[4] {i, 
			i+ lu[playerBlockId, playerOrient, 0], 
			i+ lu[playerBlockId, playerOrient, 2], 
			i+ lu[playerBlockId, playerOrient, 4]};
		
		int[] posy = new int[4]{ j, 
			j + lu[playerBlockId, playerOrient, 1], 
			j + lu[playerBlockId, playerOrient, 3], 
			j + lu[playerBlockId, playerOrient, 5]};
		
		for (int x = 0; x < 4; x++){
			blocks[posx[x], posy[x]].SetState("active");
		}
	}

	void FixBlocks(int[] posx, int[] posy){
		for (int x = 0; x < 4; x++){
			blocks[posx[x], posy[x]].SetState("filled");
		}
	}

	void MovePlayerBlocks(){
		for (int x = 0; x < 16; x++) {
			for (int y = 0; y < 32; y++) {
				if(blocks[x, y].GetState() == "active"){
					blocks[x, y].SetState("empty");
				}
			}
		}

		for (int x = 0; x < 4; x++){
			blocks[posx[x], posy[x]].SetState("active");
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
			if(blocks[posx[index] + direction, posy[index]].GetState() == "filled"){
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
				if (blocks [posx [x], posy [x]].GetState() == "filled") {
					if (posx [x] > posx [0]) {
						if (DoesItFit (i - 1, j, playerBlockId, playerOrient)) {
							moveby [0] = -1;
						}
					} else if (posx [x] < posx [0]) {
						if (DoesItFit (i + 1, j, playerBlockId, playerOrient)) {
							moveby [0] = 1;
						}
					} else if (posy [x] < posx[1]){
						if (DoesItFit (i, j+1, playerBlockId, playerOrient)) {
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
		} else if (testposx.Max () > 15) {
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

		Debug.Log ("poop");

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

		MovePlayerBlocks();
	}

	int[] CheckForCompletedRows(){
		List<int> rows = new List<int>();
	
		for (int j = 0; j < 32; j++) {
			for (int i = 0; i < 16; i++){
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
		for (int i = 0; i < 16; i++) {
			blocks[i, rownum].SetState("empty");
		}
	}

	int NumRowsGreater(int i, int[] rows){
		int index = 0;
		while (i > rows[index]) {
			index++;
		}
		return index;
	}

	void UpdateGrid(int[] rows){
		for (int row = 0; row < rows.Length; row++) {
			EmptyRow(rows[row]);
		}

		for (int j = 1; j < 32; j++) {
			for (int i = 0; i < 16; i++){
				int nrg = NumRowsGreater(i, rows);
				if(blocks[i, j].GetState() != "active"){
					blocks[i, j-nrg].SetState(blocks[i, j].GetState());
					blocks[i, j].SetState("empty");
				}
			}
		}
	}

	void Update () {

		if (Input.GetKeyDown(KeyCode.RightArrow)) {
			UpdatePlayerHorizontally(1);
		}
		
		else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			UpdatePlayerHorizontally(-1);
		}
		
		else if (Input.GetKeyDown(KeyCode.UpArrow)) {
			RotatePlayer(1);
		}
		
		else if (Input.GetKeyDown(KeyCode.DownArrow)) {
			RotatePlayer(-1);
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
