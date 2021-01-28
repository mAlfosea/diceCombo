using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementInfos : MonoBehaviour {

	public Card card;
	public int diceId;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void InitDice (int newDiceId, Card newCard) {
		this.card = newCard;
		this.diceId = newDiceId;
		this.UpdateDiceInfos ();
	}

	public void UpdateDiceInfos () {
		GameObject dice = this.transform.GetChild (1).gameObject;
		dice.GetComponent<Image> ().color = DictionaryManager.ActionColor (this.card.tempAction.color);
		dice.transform.GetChild(0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("action_type_" + this.card.tempAction.type);
		if (this.card.tempAction.type == 1) {
			dice.transform.GetChild (1).GetComponent<Text> ().text = this.card.tempAction.combatActions [0].damage.ToString ();
			dice.transform.GetChild (1).gameObject.SetActive (true);
			if (this.card.tempAction.combatActions.Length > 1) {
				dice.transform.GetChild (2).GetComponent<Text> ().text = "x" + this.card.tempAction.combatActions.Length;
				dice.transform.GetChild (2).gameObject.SetActive (true);
			} else {
				dice.transform.GetChild (2).gameObject.SetActive (false);			
			}
		} else {
			dice.transform.GetChild (1).gameObject.SetActive (false);
			dice.transform.GetChild (2).gameObject.SetActive (false);
		}
	}
}
