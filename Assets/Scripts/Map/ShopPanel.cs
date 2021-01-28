using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour {

	public GameObject cardContainer;
	public GameObject itemContainer;

	public GameObject[] cards = new GameObject[5];
	public GameObject[] items = new GameObject[1];

	public int actionSelected;
	public int itemSelected;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Init () {
		this.GenerateCards ();
		this.GenerateItems ();
	}

	public void GenerateCards () {
		for (int i = 0; i < CampaignManager.cardAvailable.Length; i++) {
			//print (CampaignManager.cardAvailable [i].price.ToString ());
			this.cards [i].transform.GetChild (1).gameObject.SetActive (true);
			this.cards [i].transform.GetChild (2).gameObject.SetActive (true);
			this.cards [i].transform.GetChild (1).GetChild (1).GetComponent<Text> ().text = CampaignManager.cardAvailable [i].price.ToString ();
			this.cards [i].transform.GetChild (2).GetComponent<PopupGenerator> ().GenerateCard (CampaignManager.cardAvailable [i]);

			if (CampaignManager.gold < CampaignManager.cardAvailable [i].price) {
				this.cards [i].GetComponent<Button> ().enabled = false;
				this.cards [i].transform.GetChild (1).GetChild (1).GetComponent<Text> ().color = DictionaryManager.ActionColor (1);
			} else {
				this.cards [i].GetComponent<Button> ().enabled = true;
				this.cards [i].transform.GetChild (1).GetChild (1).GetComponent<Text> ().color = DictionaryManager.ActionColor (2);
			}
		}
	}

	public void GenerateItems () {
		for (int i = 0; i < this.items.Length; i++) {

			if (CampaignManager.gold < this.items[i].GetComponent<PopupElement>().characterId) {
				this.items [i].GetComponent<Button> ().enabled = false;
				this.items [i].transform.GetChild (0).GetChild (1).GetComponent<Text> ().color = DictionaryManager.ActionColor (1);
			} else {
				this.items [i].GetComponent<Button> ().enabled = true;
				this.items [i].transform.GetChild (0).GetChild (1).GetComponent<Text> ().color = DictionaryManager.ActionColor (2);
			}
		}
	}

	public void CardSelected (int newCardId) {
		this.actionSelected = newCardId;
		CampaignManager.itemToBuy = newCardId;
		CampaignManager.itemPrice = CampaignManager.cardAvailable [newCardId].price;
	}
	public void ItemSelected (int newCardId) {
		this.itemSelected = newCardId;
		CampaignManager.itemToBuy = newCardId;
		CampaignManager.itemPrice = 50;
	}

	public void CardBought (PopupElement newPopupElement) {
		this.cards [this.actionSelected].transform.GetChild (1).gameObject.SetActive (false);
		this.cards [this.actionSelected].transform.GetChild (2).gameObject.SetActive (false);
		this.actionSelected = 0;
	}

	public void ItemBought (PopupElement newPopupElement) {
		this.itemSelected = 0;
	}
}
