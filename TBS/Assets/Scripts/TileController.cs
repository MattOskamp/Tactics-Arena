using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour {

	public int Row, Col;
	public Color moveColour;
	public Color attackColour;
	private UnitController unitOnTile;

	public enum TileState {
		Moveable,
		Attackable,
		Inactive
	}
	public TileState state;

	public UnitController UnitOnTile
	{
		get { return unitOnTile; }
		set { unitOnTile = value; }
	}

	public bool HasUnit()
	{
		if (unitOnTile == null)
			return false;
		else
			return true;
	}

	// Use this for initialization
	void Start () {
		state = TileState.Inactive;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetUnit(UnitController unit)
	{
		this.unitOnTile = unit;
		unit.CurrentTile = this;
	}

	void OnMouseDown()
	{
		// switched to OnTileMouseDown()
	}

	public void OnTileMouseDown()
	{
		switch (state)
		{
		case TileState.Moveable:
			// move the selected unit to this tile;
			switch (GameManager.Instance.state)
			{
			case GameManager.GameState.Moving:
				if (this.HasUnit())
					this.unitOnTile.ShowMovement();
				else
					GameManager.Instance.SelectedUnit.MoveToTile(this);
				break;
			case GameManager.GameState.Attacking:
				if (this.HasUnit())
					this.unitOnTile.ShowAttack();
				break;
			}

			break;
		case TileState.Attackable:
			switch (GameManager.Instance.state)
			{
			case GameManager.GameState.Moving:
				if (this.HasUnit())
					this.unitOnTile.ShowMovement();
				break;
			case GameManager.GameState.Attacking:
				if (this.HasUnit())
					GameManager.Instance.Battle(GameManager.Instance.SelectedUnit, this.UnitOnTile);
				break;
			}
			break;
		case TileState.Inactive:
			switch (GameManager.Instance.state)
			{
			case GameManager.GameState.Moving:
				if (this.HasUnit())
					this.unitOnTile.ShowMovement();
				break;
			case GameManager.GameState.Attacking:
				if (this.HasUnit())
					this.unitOnTile.ShowAttack();
				break;
			}
			break;
		}
	}

	void OnMouseOver()
	{
		switch (state)
		{
		case TileState.Inactive:
			this.renderer.material.color = Color.gray;
			break;
		}
	}

	void OnMouseExit()
	{
		switch (state)
		{
		case TileState.Inactive:
			this.renderer.material.color = Color.white;
			break;
		}
	}

	public void SetMovable()
	{
		state = TileState.Moveable;
		// change the colour
		this.renderer.material.color = moveColour;
	}

	public void SetAttackable()
	{
		state = TileState.Attackable;
		// change the colour
		this.renderer.material.color = attackColour;
	}

	public void SetInactive()
	{
		state = TileState.Inactive;
		this.renderer.material.color = Color.white;
	}
}
