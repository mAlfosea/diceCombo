using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardZone : MonoBehaviour {

	public GameObject zone;

	public delegate void SendTarget(GameObject newTarget);
	public static event SendTarget TargetSelected;
	public delegate void RemoveTarget();
	public static event RemoveTarget TargetDeselected;

	public bool isCharacterZone;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnEnable()
	{
		CardPanel.CardDragged += this.ShowZone;
		CardPanel.CardDropped += this.HideZone;
	}


	void OnDisable()
	{
		CardPanel.CardDragged -= this.ShowZone;
		CardPanel.CardDropped -= this.HideZone;
	}

	public void ShowZone (Card newCard) 
	{
		if (this.isCharacterZone) {
			CharacterInfosPanel characterInfosPanelScript = this.GetComponent<CharacterInfosPanel> ();
			if (characterInfosPanelScript.characterGameobject != null) {
				//print ("la zone de " + this.characterScript.summonerName + " est activee");
				if (characterInfosPanelScript.characterScript.characterType == 3 && newCard.action.targetType == 2) {
					this.zone.SetActive (true);
				}
			}
		} else {
			if (newCard.action.targetType == 1 || newCard.action.targetType == 3) {
				this.zone.SetActive (true);
			}		
		}
	}

	public void HideZone () 
	{
		this.zone.GetComponent<Outline>().effectColor = new Color32 (118, 118, 118, 255);
		this.zone.SetActive (false);
	}

	public void CardIsOn () {
		//print ("La card passe au dessus de " + this.characterGameobject.name);
		this.zone.GetComponent<Outline>().effectColor = new Color32 (248, 240, 13, 233);
		if (TargetSelected != null) {
			if (this.isCharacterZone) {
				TargetSelected (this.GetComponent<CharacterInfosPanel> ().characterGameobject);
			} else {
				TargetSelected (this.zone);			
			}
		}
	}
	public void CardIsOut () {
		//print ("La card est sortie de " + this.characterGameobject.name);
		this.zone.GetComponent<Outline>().effectColor = new Color32 (118, 118, 118, 255);
		if (TargetDeselected != null) {
			TargetDeselected ();
		}
	}
}
