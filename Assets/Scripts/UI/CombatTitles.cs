using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatTitles : MonoBehaviour {

	public GameObject turnStateTitle;
	public GameObject startPos;
	public GameObject endPos;
	public GameObject intermediatePos;

	void OnEnable()
	{
		CombatManager.E_ChangeTurn += this.ChangeTurn;
		CombatManager.E_ShowCombatText += this.ShowCombatText;
	}


	void OnDisable()
	{
		CombatManager.E_ChangeTurn -= this.ChangeTurn;
		CombatManager.E_ShowCombatText -= this.ShowCombatText;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void ChangeTurn (int newTurnState) {
		if (newTurnState == 0) {
			this.turnStateTitle.GetComponentInChildren<Text> ().text = "Votre Tour";
			this.MoveToIntermediatePos ();
		} else if (newTurnState == 1) {
			this.turnStateTitle.GetComponentInChildren<Text> ().text = "Tour Ennemi";
			this.MoveToIntermediatePos ();
		}
	}

	private void ShowCombatText (int newCombatTextId) {
		switch (newCombatTextId) 
		{
		case 1:
			this.turnStateTitle.GetComponentInChildren<Text> ().text = "Début du combat";
			break;
		case 2:
			this.turnStateTitle.GetComponentInChildren<Text> ().text = "Victoire";
			break;
		case 3:
			this.turnStateTitle.GetComponentInChildren<Text> ().text = "Défaite";
			break;
		default:
			this.turnStateTitle.GetComponentInChildren<Text> ().text = "Text par defaut";			
			break;
		}
		this.MoveToIntermediatePos ();
	}

	private void MoveToIntermediatePos () {
		this.turnStateTitle.transform.position = this.startPos.transform.position;
		
		iTween.MoveTo (this.turnStateTitle, iTween.Hash(
			"position", intermediatePos.transform.position, 
			"time", 1.5f, 
			"easetype", iTween.EaseType.easeInOutQuart,
			"oncompletetarget", this.gameObject,
			"oncomplete", "MoveToEndPos"));
	}
	private void MoveToEndPos () {
		iTween.MoveTo (this.turnStateTitle, iTween.Hash(
			"position", this.endPos.transform.position, 
			"time", 1.5f, 
			"easetype", iTween.EaseType.easeInOutQuart));
	}
}
