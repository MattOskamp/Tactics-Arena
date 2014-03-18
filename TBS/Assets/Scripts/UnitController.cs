using UnityEngine;
using System.Collections;

public class UnitController: MonoBehaviour {

	public float hitPoints = 100;
	public float attackDamage = 10;
	public int moveRange = 3;
	public int attackRange = 3;
	public float armour = 0.1f;
	public float blockingChanceFront = 0.6f;
	public float blockingChanceSide = 0.25f;
	public float blockingChanceRear = 0;
	public int cost = 10;
	public int restTurnsMovement = 0;
	public int restTurnsAttacking = 1;
	public int turnsToRest;
	public int turnsRested;

	public float speed = 5;

	private bool hasMoved;
	public bool hasAttacked;

	public GameManager.Player controller;

	public enum UnitOrientation
	{
		North,
		South,
		East,
		West
	}
	public UnitOrientation facing;

	public enum UnitState {
		Moving,
		Attacking,
		Resting,
		Idle
	}
	public UnitState state;

	private TileController currentTile;
	private TileController targetTile;

	public TileController CurrentTile
	{
		set { currentTile = value; }
	}

	// Use this for initialization
	void Start () {
		state = UnitState.Idle;
		hasMoved = false;
		hasAttacked = false;
		facing = UnitOrientation.North;
		turnsRested = 0;
		turnsToRest = 0;
	}
	
	// Update is called once per frame
	void Update () {
		switch (state)
		{
		case UnitState.Moving:
			Vector3 direction = targetTile.transform.position - this.transform.position;
			direction.Normalize();
			this.transform.position += direction * Time.deltaTime * speed;

			if (Vector3.Distance(this.transform.position, targetTile.transform.position) < 0.05f)
			{
				this.transform.position = targetTile.transform.position;
				state = UnitState.Idle;
				this.hasMoved = true;
				this.currentTile.UnitOnTile = null;
				this.targetTile.SetUnit(this);
				this.currentTile = this.targetTile;
				this.targetTile = null;
				GameManager.Instance.ClearTiles();
			}
			break;
		}
	}

	public void SetCurrentTile(TileController tile)
	{
		this.currentTile = tile;
		tile.UnitOnTile = this;
	}

	public void MoveToTile(TileController tile)
	{
		switch (state)
		{
		case UnitState.Idle:
			if (this.controller == GameManager.Instance.currentTurn && !hasMoved)
			{
				this.targetTile = tile;
				GameManager.Instance.ClearTiles();
				this.targetTile.SetMovable();
				state = UnitState.Moving;
			}
			break;
		}
	}

	public void ShowMovement()
	{
		switch(state)
		{
		case UnitState.Idle:
			if (controller == GameManager.Instance.currentTurn && !hasMoved)
				GameManager.Instance.ShowMovementRange(this.currentTile, this.moveRange);
			break;
		}
	}

	public void ShowAttack()
	{
		switch (state)
		{
		case UnitState.Idle:
			if (controller == GameManager.Instance.currentTurn && !hasAttacked)
				GameManager.Instance.ShowAttackRange(this.currentTile, this.attackRange);
			break;
		}
	}

	void OnMouseDown()
	{
		// replaced by tile
	}

	public void TakeDamage(float damage)
	{
		// negate the damage from the armour
		damage = damage * (1-armour);
		hitPoints -= damage;
	}

	public void EndTurn()
	{
		if (hasMoved)
			turnsToRest += restTurnsMovement;
		if (hasAttacked)
			turnsToRest += restTurnsAttacking;
		if (turnsRested < turnsToRest)
			state = UnitState.Resting;
	}

	public void BeginTurn()
	{
		hasMoved = false;
		hasAttacked = false;

		if (turnsRested >= turnsToRest)
		{
			// wake up
			turnsRested = 0;
			turnsToRest = 0;
			state = UnitState.Idle;
		}

		switch (state)
		{
		case UnitState.Resting:
			turnsRested++;
			break;
		}
	}
}
