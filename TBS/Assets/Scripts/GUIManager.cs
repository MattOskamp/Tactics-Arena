using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {

	GUIStyle moveStyle, attackStyle;
	public string playerText = "Player 1";
	// Use this for initialization
	void Start () {
		moveStyle = new GUIStyle();
		moveStyle.normal.textColor = Color.green;
		attackStyle = new GUIStyle();
		attackStyle.normal.textColor = Color.white;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		if (GUI.Button(new Rect( 20, 20, 80, 20), "Movement", moveStyle))
		{
			GameManager.Instance.state = GameManager.GameState.Moving;
			moveStyle.normal.textColor = Color.green;
			attackStyle.normal.textColor = Color.white;
		}

		if (GUI.Button(new Rect( 110, 20, 80, 20), "Attack", attackStyle))
		{
			GameManager.Instance.state = GameManager.GameState.Attacking;
			attackStyle.normal.textColor = Color.green;
			moveStyle.normal.textColor = Color.white;
		}

		if (GameManager.Instance.currentTurn == GameManager.Player.One)
		{
			playerText = "Player 1";
		}
		else
		{
			playerText = "Player 2";
		}

		GUI.Label(new Rect(Screen.width - 200, 20, 80, 20), playerText);
		if (GUI.Button(new Rect(Screen.width - 100, 20, 80, 20), "End Turn"))
		{
			// tell the game manager to end the turn
			GameManager.Instance.EndTurn();
			moveStyle.normal.textColor = Color.green;
			attackStyle.normal.textColor = Color.white;
		}
	}
}
