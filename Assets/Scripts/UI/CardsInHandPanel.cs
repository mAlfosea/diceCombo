using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsInHandPanel : MonoBehaviour {

	public GameObject cardPrefab;
	public GameObject cardInvisiblePrefab;

	public GameObject cardHiddenContainer;
	public GameObject cardVisibleContainer;
	public GameObject instantiateCardPos;
	public GameObject playCardPos;
	public GameObject killCardPos;

	public static List<GameObject> L_Cards_visible = new List<GameObject>();
	public static List<GameObject> L_Cards_hidden = new List<GameObject>();

	private int cardIndex;

	void OnEnable()
	{
		Summoner.CardGenerated += this.GenerateCard;
		Summoner.E_CardUpdatedWithEtat += this.UpdateCardsWithEtat;
		Summoner.CardPlayed += this.CardPlayed;
		Summoner.E_CardBanned += this.BanSpecificCard;
		Summoner.CheckLaunchableCard += this.UpdateIsLaunchable;
		Summoner.PlayCardDroppedAnimation += this.CardDroppedAnimation;
		CombatManager.E_ChangeTurn += this.ChangeTurn;
	}


	void OnDisable()
	{
		Summoner.CardGenerated -= this.GenerateCard;
		Summoner.E_CardUpdatedWithEtat -= this.UpdateCardsWithEtat;
		Summoner.CardPlayed -= this.CardPlayed;
		Summoner.E_CardBanned -= this.BanSpecificCard;
		Summoner.CheckLaunchableCard -= this.UpdateIsLaunchable;
		Summoner.PlayCardDroppedAnimation -= this.CardDroppedAnimation;
		CombatManager.E_ChangeTurn -= this.ChangeTurn;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	#region Generation de la carte et Mort de la carte
	public void GenerateCard (Card newCard) {
		GameObject hiddenCard = Instantiate (this.cardInvisiblePrefab, this.cardHiddenContainer.transform.position, this.transform.rotation, this.cardHiddenContainer.transform) as GameObject;
		L_Cards_hidden.Add (hiddenCard);
		this.UpdateSpacing ();

		GameObject card = Instantiate (this.cardPrefab, this.instantiateCardPos.transform.position, this.transform.rotation, this.cardVisibleContainer.transform) as GameObject;
		CardPanel cardScript = card.GetComponent<CardPanel> ();
		cardScript.Init (newCard, L_Cards_hidden.Count - 1, hiddenCard);
		L_Cards_visible.Add (card);
	}
	public void EndCombat () {
		for (int i = 0; i < L_Cards_visible.Count; i++) {
			Destroy (L_Cards_visible [i]);
		}
		L_Cards_visible.Clear ();

		for (int i = 0; i < L_Cards_hidden.Count; i++) {
			Destroy (L_Cards_hidden [i]);
		}
		L_Cards_hidden.Clear ();
	}


	public void ChangeTurn (int newTurnState) {
		if (newTurnState == 1 || newTurnState == 3) {
			for (int i = 0; i < L_Cards_visible.Count; i++) {
				CardPanel cardScript = L_Cards_visible [i].GetComponent<CardPanel> ();
				cardScript.RemoveCard (killCardPos);
			}
			L_Cards_visible.Clear ();

			for (int i = 0; i < L_Cards_hidden.Count; i++) {
				Destroy (L_Cards_hidden [i]);
			}
			L_Cards_hidden.Clear ();
		}
	}
	#endregion

	public void BanSpecificCard (Card newCard) {
		for (int i = 0; i < L_Cards_visible.Count; i++) {
			if (L_Cards_visible [i].GetComponent<CardPanel> ().card.uId == newCard.uId) {
				CardPanel cardScript = L_Cards_visible [i].GetComponent<CardPanel> ();
				cardScript.RemoveCard (killCardPos);
				this.UpdateCardList (newCard);
				return;
			}
		}
	}

	public void CardPlayed (Card newCard) {
		this.UpdateCardList (newCard);
	}

	public void CardDroppedAnimation (Card newCard, bool newCardState) {
		//print ("je dois alors dropped anim la carte: " + newCard.uId);
		for (int i = 0; i < L_Cards_visible.Count; i++) {
			if (L_Cards_visible [i].GetComponent<CardPanel> ().card.uId == newCard.uId) {
				L_Cards_visible [i].GetComponent<CardPanel> ().PlayDroppedCardAnimation (newCardState, this.playCardPos);
				//print ("jai bien trouve la carte a animer");
			}
		}
	}

	#region Update de la liste de carte et des spacing
	public void UpdateCardList (Card newCard) {
		for (int i = 0; i < L_Cards_visible.Count; i++) {
			if (L_Cards_visible[i].GetComponent<CardPanel>().card.uId == newCard.uId) {

				//Destroy (cardVisibleContainer.transform.GetChild(i).gameObject);
				Destroy (cardHiddenContainer.transform.GetChild(i).gameObject);
				L_Cards_visible[i].GetComponent<CardPanel>().ScaleDownCard ();

				L_Cards_visible.RemoveAt (i);
				L_Cards_hidden.RemoveAt (i);

				this.UpdateSpacing ();

				return;
			}
		}
	}

	public void UpdateCardsWithEtat () {
		for (int i = 0; i < L_Cards_visible.Count; i++) {
			L_Cards_visible [i].GetComponent<CardPanel> ().GenerateCard ();
		}
	}

	public void UpdateIsLaunchable () {
		for (int i = 0; i < L_Cards_visible.Count; i++) {
			L_Cards_visible [i].GetComponent<CardPanel> ().CheckLaunchable ();
		}
	}

	public void UpdateSpacing () {
		this.cardHiddenContainer.GetComponent<HorizontalLayoutGroup> ().spacing = - (13f * L_Cards_hidden.Count);
	}
	#endregion
}
