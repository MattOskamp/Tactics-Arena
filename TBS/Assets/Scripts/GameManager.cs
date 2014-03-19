using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public int boardWidth = 10;
	public int boardHeight = 10;
	public GameObject tilePrefab;
	public GameObject unitPrefab;

	public int numberOfPlayers = 2;
	private List<PlayerController> players;

	public Color player1Colour;
	public Color player2Colour;

	public enum GameState {
		Moving,
		Attacking,
		Directing,
		Waiting
	}

	public PlayerController currentTurn;
	private int turnIndex;

	public GameState state;

	private TileController[,] board;
	private UnitController selectedUnit;

	public UnitController SelectedUnit
	{
		get { return this.currentTurn.selectedUnit; }
		set { this.currentTurn.selectedUnit = value; }
	}

	private static GameManager instance;
	public static GameManager Instance 
	{
		get 
		{
			return instance;
		}
	}

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		BuildBoard();
		state = GameState.Moving;
		turnIndex = 0;
		players = new List<PlayerController>();
		for (int i = 0; i < numberOfPlayers; i++)
		{
			PlayerController p = new PlayerController("Player " + (i + 1));
			players.Add(p);
		}

		currentTurn = players[turnIndex];

		// set up the units on the board
		for (int i = 0; i < boardWidth; i++)
		{
			GameObject go = GameObject.Instantiate(unitPrefab, board[1,i].transform.position, Quaternion.identity) as GameObject;
			UnitController uc = go.GetComponent<UnitController>();
			uc.SetCurrentTile(board[1,i]);
			uc.controller = players[0];
			uc.renderer.material.color = player1Colour;
			players[0].unitList.Add(uc);

			GameObject go1 = GameObject.Instantiate(unitPrefab, board[boardWidth - 2, i].transform.position, Quaternion.identity) as GameObject;
			UnitController uc1 = go1.GetComponent<UnitController>();
			uc1.SetCurrentTile(board[boardWidth-2, i]);
			uc1.controller = players[1];
			uc1.renderer.material.color = player2Colour;
			players[1].unitList.Add(uc1);
		}
	}
	
	// Update is called once per frame
	void Update () {
		// check to see where the user is clicking
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			LayerMask mask = 1 << LayerMask.NameToLayer("Tile");
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
			{
				TileController tile = hit.collider.GetComponent<TileController>();
				tile.OnTileMouseDown();
			}
		}
	}

	private void BuildBoard()
	{
		this.board = new TileController[boardHeight,boardWidth];
		GameObject boardGO = new GameObject();
		boardGO.name = "Board";
		// (1,1) (row, col) of the board should be top left
		for (int i = 0; i < this.boardHeight; i++)
		{
			for (int j = 0; j < this.boardWidth; j++)
			{
				GameObject go = GameObject.Instantiate(tilePrefab, 
				                                       new Vector3(j * this.tilePrefab.transform.localScale.x,
				            0,
				            i * this.tilePrefab.transform.localScale.z),
				                                       Quaternion.Euler(new Vector3(90, 0, 0))) as GameObject;
				go.name = "Tile " + i + "," + j;
				go.transform.parent = boardGO.transform;
				TileController tc = go.GetComponent<TileController>();
				tc.Row = i;
				tc.Col = j;
				board[i,j] = tc;
			}
		}
	}

	public void ShowMovementRange(TileController tile, int range)
	{
		List<TileController> searched = new List<TileController>();
		this.ClearTiles();

		// path find from tile to each tile in open list
		PathFind(tile, tile.UnitOnTile, range, searched);

		foreach (TileController tc in searched)
		{
			tc.SetMovable();
		}

		tile.SetInactive();
	}

	public void PathFind(TileController currentTile, UnitController unit, int steps, List<TileController> tiles)
	{
		if (currentTile.HasUnit() && currentTile.UnitOnTile != unit)
			return;

		tiles.Add(currentTile);

		if (steps == 0)
			return;

		steps--; // take a step

		// North
		int r = currentTile.Row + 1;
		int c = currentTile.Col;
		if (r >= 0 && r < boardHeight)
			if (c >= 0 && c < boardWidth)
				PathFind(board[r, c], unit, steps, tiles);

		// East
		r = currentTile.Row;
		c = currentTile.Col + 1;
		if (r >= 0 && r < boardHeight)
			if (c >= 0 && c < boardWidth)
				PathFind(board[r, c], unit, steps, tiles);

		// South
		r = currentTile.Row - 1;
		c = currentTile.Col; 
		if (r >= 0 && r < boardHeight)
			if (c >= 0 && c < boardWidth)
				PathFind(board[r, c], unit, steps, tiles);

		// West
		r = currentTile.Row;
		c = currentTile.Col - 1; 
		if (r >= 0 && r < boardHeight)
			if (c >= 0 && c < boardWidth)
				PathFind(board[r, c], unit, steps, tiles);
	}

	public void ShowAttackRange(TileController tile, int range)
	{
		this.ClearTiles();
		// May be different shapes than just moving
		for (int j = 0; j <= range; j++)
		{
			for (int i = 0; i <= j; i++)
			{
				int r = tile.Row + i;
				int c = tile.Col + j - i;
				if (r >= 0 && r < boardHeight)
					if (c >= 0 && c < boardWidth)
						board[r , c].SetAttackable();
				
				r = tile.Row + j - i;
				c = tile.Col - i;
				if (r >= 0 && r < boardHeight)
					if (c >= 0 && c < boardWidth)
						board[r, c].SetAttackable();
				
				r = tile.Row - i;
				c = tile.Col - j + i; 
				if (r >= 0 && r < boardHeight)
					if (c >= 0 && c < boardWidth)
						board[r,c].SetAttackable();
				
				r = tile.Row - (j-i);
				c = tile.Col + i; 
				if (r >= 0 && r < boardHeight)
					if (c >= 0 && c < boardWidth)
						board[r, c].SetAttackable();
			}
		}
		tile.SetInactive();
	}

	public void ClearTiles()
	{
		foreach (TileController tc in board)
		{
			tc.SetInactive();
		}
	}

	public void EndTurn()
	{
		if (turnIndex == 0) // player 1
		{
			turnIndex++;
			players[0].EndTurn();
		}
		else // only 2 players for now
		{
			turnIndex = 0;
			players[1].EndTurn();
		}
		this.currentTurn = players[turnIndex];
		this.currentTurn.BeginTurn();
		this.state = GameState.Moving;
		this.ClearTiles();
	}

	public void Battle(UnitController attacker, UnitController defender)
	{
		// see if the attacked is attacking from the front, side, or rear
		// see if the attack was blocked
		attacker.hasAttacked = true;
		currentTurn.hasAttacked = true;

		if (attacker.facing == defender.facing)
		{
			// from behind
			if (Random.value < defender.blockingChanceRear)
			{
				// miss!
				print ("miss");
			}
			else
			{
				// hit!
				defender.TakeDamage(attacker.attackDamage);
			}
		}
		else if ((attacker.facing == UnitController.UnitOrientation.North && 
		         defender.facing == UnitController.UnitOrientation.South) ||
		         (attacker.facing == UnitController.UnitOrientation.South &&
		 		defender.facing == UnitController.UnitOrientation.North) ||
		         (attacker.facing == UnitController.UnitOrientation.East && 
		 		defender.facing == UnitController.UnitOrientation.West) ||
		         (attacker.facing == UnitController.UnitOrientation.West && 
		 		defender.facing == UnitController.UnitOrientation.East))
		{
			// from head on
			if (Random.value < defender.blockingChanceFront)
			{
				// miss!
				print("miss!");
			}
			else
			{
				defender.TakeDamage(attacker.attackDamage);
			}
		}
		else
		{
			// from the side
			if (Random.value < defender.blockingChanceSide)
			{
				// miss!
				print ("miss!");
			}
			else
			{
				defender.TakeDamage(attacker.attackDamage);
			}
		}

		this.ClearTiles();

	}

}
