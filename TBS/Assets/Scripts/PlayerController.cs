using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController {
	public UnitController selectedUnit;
	public string name;
	public bool hasMoved;
	public bool hasAttacked;
	public List<UnitController> unitList;

	public PlayerController(string name)
	{
		this.name = name;
		unitList = new List<UnitController>();
	}

	public void BeginTurn()
	{
		hasMoved = false;
		hasAttacked = false;

		foreach (UnitController uc in unitList)
		{
			uc.BeginTurn();
		}
	}

	public void EndTurn()
	{
		foreach (UnitController uc in unitList)
		{
			uc.EndTurn();
		}
	}
}
