using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardPanel : MonoBehaviour {

	public GameObject typeGameObject;
	public Image cardBackgroundImg;
	public Image cardShadowImg;
	public Text actionName;
	public Text actionDesc;
	public Image actionImg;
	public Image summonerImg;
	public Image diceImg;
	public Card card;
	public Sprite[] diceFaces;
	public Sprite[] cardBarckgrounds;

	private int cardPanelIndex;
	private Transform objectToFollowPos;
	private GameObject hiddenCardToFollow;
	private bool isFollowObject;
	private bool isFollowMouse;
	private float yPosModificator;
	public bool isDragged;
	public bool isPlaying;
	public bool isWaiting;

	public Coroutine cardCoroutine;

	public delegate void CardSelected(Card newCard);
	public static event CardSelected CardDragged;

	public delegate void CardDeselected();
	public static event CardDeselected CardDropped;

	public delegate void CardHovered(GameObject newCardObject);
	public static event CardHovered E_CardHovered;

	public delegate void RemoveKeywords();
	public static event RemoveKeywords E_CardOut;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (this.isFollowObject) {
			Vector3 ObjectToFollowPos = new Vector3 (this.objectToFollowPos.position.x, this.objectToFollowPos.position.y + yPosModificator, this.objectToFollowPos.position.z);
			this.transform.position = Vector3.Lerp(this.transform.position, ObjectToFollowPos, 5f * Time.deltaTime);
		}
		if (this.isFollowMouse) {
			Vector3 mouseToFollow = new Vector3 (Input.mousePosition.x, Input.mousePosition.y + yPosModificator, Input.mousePosition.z);
			this.transform.position = Vector3.Lerp(this.transform.position, mouseToFollow, 5f * Time.deltaTime);
		}
	}

	public void Init (Card newCard, int newIndex, GameObject newHiddenCardToFollow) {
		this.card = newCard;
		this.cardPanelIndex = newIndex;
		this.name = this.cardPanelIndex + "_" + this.card.action.name;
		this.hiddenCardToFollow = newHiddenCardToFollow;
		this.GenerateCard ();
		this.objectToFollowPos = this.hiddenCardToFollow.transform;
		this.isFollowObject = true;
	}

	#region Gestion des Events de la carte
	public void CardOnDrag () {
		if (this.card.isLaunchable) {
			this.isDragged = true;
			this.FollowMouse ();
			if (CardDragged != null)
				CardDragged (this.card);
		}
	}

	public void CheckCardClick () {
		if (this.isDragged == false && this.card.isLaunchable) {
			if (this.card.isLaunchable) {
				this.isDragged = true;
				this.FollowMouse ();
				if (CardDragged != null)
					CardDragged (this.card);
			}
		} else if (this.isDragged) {
			this.GetComponent<EventTrigger> ().enabled = false;
			this.isDragged = false;
			this.CardOut ();
			this.isFollowObject = false;
			this.isFollowMouse = false;
			this.isWaiting = true;
			if (CardDropped != null)
				CardDropped ();
		}
	} 

	public void CardIsDropped () {
		if (this.isDragged) {
			this.GetComponent<EventTrigger> ().enabled = false;
			this.isDragged = false;
			this.CardOut ();
			this.isFollowObject = false;
			this.isFollowMouse = false;
			this.isWaiting = true;
			if (CardDropped != null)
				CardDropped ();
		}
	}

	public void CardHover () {
		if (this.isDragged) {
		} else {
			this.hiddenCardToFollow.GetComponent<LayoutElement> ().minWidth = 300f + (25f * this.transform.parent.childCount);
			this.yPosModificator = 100f;
		}
		//this.transform.SetAsLastSibling ();
	}

	public void CardOut () {
		if (this.isDragged) {
		} else {
			this.hiddenCardToFollow.GetComponent<LayoutElement> ().minWidth = 300f;
			this.yPosModificator = 0;
			//this.transform.SetSiblingIndex (this.cardPanelIndex);
		}
	}

	public void CallKeywordsGenerator () {
		if (E_CardHovered != null) {
			E_CardHovered (this.gameObject);
		}
	}

	public void CallKeywordsDestroyer () {
		if (E_CardOut != null) {
			E_CardOut ();
		}
	}
	#endregion

	#region Gestion du Drag 
	public void FollowMouse () {
		this.isFollowObject = false;
		this.yPosModificator = - 130f;
		this.isFollowMouse = true;
		iTween.ScaleTo (this.gameObject, iTween.Hash(
			"scale", new Vector3 (0.5f, 0.5f, 0.5f), 
			"time", 0.5f, 
			"easetype", iTween.EaseType.easeOutBack));	
	}
	#endregion

	#region Animation de play de carte
	public void PlayDroppedCardAnimation (bool newCardState, GameObject newPlayPosition) {
		if (newCardState) {
			this.cardCoroutine = StartCoroutine (this.GoToPlayPos (newPlayPosition));
		} else {
			this.isWaiting = false;
			this.isPlaying = false;
			this.isFollowObject = true;
			this.objectToFollowPos = this.hiddenCardToFollow.transform;
			this.yPosModificator = 0;
			iTween.ScaleTo (this.gameObject, iTween.Hash(
				"scale", new Vector3 (1f, 1f, 1f), 
				"time", 0.5f, 
				"easetype", iTween.EaseType.easeOutBack));	
			this.GetComponent<EventTrigger> ().enabled = true;
		}
	}

	public IEnumerator GoToPlayPos (GameObject newPlayPos) {
		this.isPlaying = true;
		this.isWaiting = false;
		this.objectToFollowPos = newPlayPos.transform;
		this.isFollowObject = true;
		iTween.ScaleTo (this.gameObject, iTween.Hash(
			"scale", new Vector3 (1f, 1f, 1f), 
			"time", 0.5f, 
			"easetype", iTween.EaseType.easeOutBack));
		
		yield return null;
	}

	public void ScaleDownCard () {
		this.cardCoroutine = StartCoroutine (this.ScaleDownAnimation ());
	}

	public IEnumerator ScaleDownAnimation () {
		iTween.ScaleTo (this.gameObject, iTween.Hash(
			"scale", new Vector3 (0f, 0f, 0f), 
			"time", 0.5f, 
			"easetype", iTween.EaseType.easeOutBack));	

		yield return new WaitForSeconds (0.5f);
		Destroy (this.gameObject);
	}
	#endregion

	#region Mort de la carte a la fin du tour
	public void RemoveCard (GameObject newKillPos) {
		//if (this.objectToFollowPos.gameObject.name == this.hiddenCardToFollow.name && this.isFollowObject == true) {
		if (this.isPlaying == false || this.isWaiting == false) {
			this.cardCoroutine = StartCoroutine (this.GotoKillPos (newKillPos));
		}
	}
		
	public IEnumerator GotoKillPos (GameObject newKillPos) {
		this.objectToFollowPos = newKillPos.transform;
		this.isFollowObject = true;
		yield return new WaitForSeconds (3f);
		this.StopCoroutine (this.cardCoroutine);
		Destroy (this.gameObject);
	}
	#endregion

	#region Generation de la card
	public void GenerateCard () {
		this.name = this.card.uId.ToString();
		this.actionName.text = this.card.tempAction.name;
		this.actionDesc.text = DictionaryManager.GetCardDescription(this.card);
		this.actionImg.sprite = Resources.Load<Sprite> ("skill_icons/" + this.card.tempAction.id);
		if (this.card.OwnerType == 1) {
			this.summonerImg.sprite = Resources.Load<Sprite> ("character_avatars/" + this.card.owner.GetComponent<Summoner> ().idAvatar);
			this.diceImg.sprite = this.diceFaces [this.card.owner.GetComponent<Summoner> ().L_dicesValue [this.card.diceId]];
		} else if (this.card.OwnerType == 2) {
			this.summonerImg.sprite = Resources.Load<Sprite> ("character_avatars/" + this.card.owner.GetComponent<Support> ().idAvatar);
			this.diceImg.sprite = this.diceFaces [this.card.owner.GetComponent<Support> ().L_dicesValue [this.card.diceId]];
		}
		//this.typeGameObject.GetComponent<Image> ().color = this.ActionColor ();
		this.cardBackgroundImg.sprite = this.cardBarckgrounds[this.card.tempAction.color];
		this.typeGameObject.transform.GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("action_type_" + this.card.tempAction.type);
		this.CheckLaunchable ();
	}

	/*public string GetDescription () {
		print (this.card.uId + " / " + this.card.tempAction.name);

		string actionDescription = this.card.tempAction.description;
		if (actionDescription.Contains("[dmg_all]")) {
			int damage = 0;
			for (int i = 0; i < this.card.tempAction.combatActions.Length; i++) {
				if (this.card.tempAction.combatActions [i].typeDmg == 1) {
					damage += this.card.tempAction.combatActions [i].damage;
				}
			}
			actionDescription = actionDescription.Replace ("[dmg_all]", "<b><size=20><color=#" + ColorUtility.ToHtmlStringRGBA (this.ActionColor()) + ">" + Mathf.Abs(damage).ToString() + "</color></size></b>"); 
		}

		if (actionDescription.Contains ("[dmg_id_")) {
			do {
				char indexTemp = actionDescription [actionDescription.IndexOf ("[dmg_id_") + 8];
				string stringToReplace = "[dmg_id_" + indexTemp + actionDescription [actionDescription.IndexOf ("[dmg_id_") + 9];
				int damage = this.card.tempAction.combatActions [(int)char.GetNumericValue (indexTemp)].damage;
				actionDescription = actionDescription.Replace (stringToReplace, "<b><size=20><color=#" + ColorUtility.ToHtmlStringRGBA (this.ActionColor ()) + ">" + Mathf.Abs (damage).ToString () + "</color></size></b>"); 
			} while (actionDescription.Contains ("[dmg_id_"));
		}

		if (actionDescription.Contains("[dmg_count]")) {
			int damage = this.card.tempAction.combatActions.Length;
			actionDescription = actionDescription.Replace ("[dmg_count]", "<b><size=20>" + Mathf.Abs(damage).ToString() + "</size></b>"); 
		}
			
		if (actionDescription.Contains ("[etat_id_")) {
			do {
				char indexTemp = actionDescription [actionDescription.IndexOf ("[etat_id_") + 9];
				string stringToReplace = "[etat_id_" + indexTemp + actionDescription [actionDescription.IndexOf ("[etat_id_") + 10];
				int damage = this.card.tempAction.combatActions [(int)char.GetNumericValue (indexTemp)].etats [0].value;
				print (indexTemp + " / " + stringToReplace + " / " + damage);
				actionDescription = actionDescription.Replace (stringToReplace, "<b><size=20><color=#" + ColorUtility.ToHtmlStringRGBA (this.ActionColor ()) + ">" + Mathf.Abs (damage).ToString () + "</color></size></b>"); 
			} while (actionDescription.Contains ("[etat_id_"));
		}

		if (actionDescription.Contains("[etat_count]")) {
			int nbEtat = 0;
			for (int i = 0; i < this.card.tempAction.combatActions.Length; i++) {
				nbEtat += this.card.tempAction.combatActions [i].etats.Length;
			}
			actionDescription = actionDescription.Replace ("[etat_count]", "<b><size=20>" + Mathf.Abs(nbEtat).ToString() + "</size></b>"); 
		}

		if (actionDescription.Contains("[DEFENSE]")) {
			actionDescription = actionDescription.Replace ("[DEFENSE]", "<b>" + "Armures" + "</b>"); 
		}

		if (actionDescription.Contains("[FORCE]")) {
			actionDescription = actionDescription.Replace ("[FORCE]", "<b>" + "Force" + "</b>"); 
		}

		return actionDescription;
	}

	public Color32 ActionColor () {
		Color32 newColor;
		switch (this.card.tempAction.color) 
		{
		case 1:
			newColor = new Color32 (200, 30, 30, 255);	// rouge
			break;
		case 2:
			newColor = new Color32 (28, 152, 46, 255);	// vert
			break;
		case 3:
			newColor = new Color32 (28, 105, 152, 255);	// bleu
			break;
		case 4:
			newColor = new Color32 (70, 70, 70, 255);	// noir
			break;
		default:
			newColor = new Color32 (212, 211, 211, 255);	// blanc
			break;
		}
		return newColor;
	}*/

	public void CheckLaunchable () {
		//print ("LE CARD PANEL DIT: " + this.card.isLaunchable);
		if (this.card.isLaunchable) {
			this.cardShadowImg.gameObject.SetActive (true);
		} else {
			this.cardShadowImg.gameObject.SetActive (false);			
		}
	}
	#endregion
}
