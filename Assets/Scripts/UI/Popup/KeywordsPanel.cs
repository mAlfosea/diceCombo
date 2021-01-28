using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeywordsPanel : MonoBehaviour {

	public GameObject cardObject;
	public bool isFollowing;

	public GameObject keyword;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (this.isFollowing && this.cardObject != null) {
			Vector3 newPosition = new Vector3 (this.cardObject.transform.position.x + 150f, this.cardObject.transform.position.y + 150f, this.transform.position.z);
			this.transform.position = newPosition;
		}		
	}

	public void Init (GameObject newCard) {
		this.cardObject = newCard;
		this.isFollowing = true;

		this.GenerateKeywords ();
	}

	public void GenerateKeywords () {
		for (int i = 0; i < this.cardObject.GetComponent<CardPanel> ().card.action.combatActions.Length; i++) {
			for (int j = 0; j < this.cardObject.GetComponent<CardPanel> ().card.action.combatActions [i].etats.Length; j++) {
				GameObject keywordTemp = Instantiate (this.keyword, this.transform.position, this.transform.rotation, this.transform) as GameObject;
				EtatsData etatTemp = DictionaryManager.GetEtat (this.cardObject.GetComponent<CardPanel> ().card.action.combatActions [i].etats [j].idEtat);
				keyword.name = etatTemp.name;
				keywordTemp.GetComponentInChildren<Text> ().text = etatTemp.name + ": " + etatTemp.description;
			}
		}
	}

	public void DestroyKeywords () {
		this.isFollowing = false;
		this.cardObject = null;

		foreach (Transform child in this.transform) {
			//print ("je destroy " + child.name);
			GameObject.Destroy(child.gameObject);
		}
		this.transform.DetachChildren ();

	}
}
