using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupGenerator : MonoBehaviour {

	public GameObject actionElementPrefab;
	public GameObject talentElementPrefab;
	public GameObject characterElementPrefab;
	private ActionsData actionCard; // sert pour la generation du card popup
	private EtatsData etatInfos; // sert pour la generation de l'etat popup
	public Sprite[] cardBackgrounds;
	public List<GameObject> actionsGoList = new List<GameObject>();
	public List<GameObject> etatsGoList = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	#region Actions Popup
	public void GenerateActionsPopup () {
		for (int i = 0; i < DictionaryManager.actionsDictionary.Count; i++) {
			GameObject actionElementTemp = Instantiate (this.actionElementPrefab, this.transform.GetChild (2).position, this.transform.rotation, this.transform.GetChild (2).GetChild (0)) as GameObject;
			ActionsData actionTemp = DictionaryManager.actionsDictionary [i];

			actionElementTemp.name = actionTemp.name;
			actionElementTemp.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite> ("skill_icons/" + actionTemp.id);
			actionElementTemp.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite> ("action_type_" + actionTemp.type);
			actionElementTemp.transform.GetChild (2).GetComponent<Text> ().text = actionTemp.name;
			actionElementTemp.GetComponent<Image> ().color = DictionaryManager.ActionColor (actionTemp.color);
			actionElementTemp.GetComponent<PopupElement> ().action = actionTemp;

			this.actionsGoList.Add (actionElementTemp);
		}
		this.actionsGoList.Sort (SortByName);
		for (int i = 0; i < this.actionsGoList.Count; i++) {
			this.actionsGoList [i].transform.SetSiblingIndex (i);
		}
	}
	#endregion

	#region Actions Popup
	public void GenerateTalentsPopup () {
		for (int i = 0; i < DictionaryManager.etatsDictionary.Count; i++) {
			GameObject etatElementTemp = Instantiate (this.talentElementPrefab, this.transform.GetChild (2).position, this.transform.rotation, this.transform.GetChild (2).GetChild (0)) as GameObject;
			EtatsData etatTemp = DictionaryManager.etatsDictionary [i+1];

			etatElementTemp.name = etatTemp.name;
			etatElementTemp.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite> ("etat_icons/" + etatTemp.id);
			etatElementTemp.transform.GetChild (1).gameObject.SetActive (false);
			etatElementTemp.transform.GetChild (2).GetComponent<Text> ().text = etatTemp.name;
			//etatElementTemp.GetComponent<Image> ().color = DictionaryManager.ActionColor (etatTemp.color);
			etatElementTemp.GetComponent<PopupElement> ().etat = etatTemp;

			this.etatsGoList.Add (etatElementTemp);
		}
		this.etatsGoList.Sort (SortByName);
		for (int i = 0; i < this.etatsGoList.Count; i++) {
			this.etatsGoList [i].transform.SetSiblingIndex (i);
		}
	}
	#endregion

	private static int SortByName(GameObject o1, GameObject o2) {
		return o1.name.CompareTo(o2.name);
	}

	public void GenerateCharactersPopup () {

		SummonerData[] summoners = JsonSave.LoadSummoners ();
		SummonerData[] enemies = JsonSave.LoadEnemies ();
		SupportData[] supports = JsonSave.LoadSupports ();

		for (int i = 0; i < summoners.Length; i++) {
			GameObject summonerElementTemp = Instantiate (this.characterElementPrefab, this.transform.GetChild (2).position, this.transform.rotation, this.transform.GetChild (2).GetChild (0)) as GameObject;
			PopupElement summonerElementScript = summonerElementTemp.GetComponent<PopupElement> ();

			summonerElementTemp.transform.GetChild (0).GetChild (0).GetComponent<Image>().sprite = Resources.Load<Sprite> ("character_avatars/" + summoners [i].idAvatar);
			summonerElementTemp.transform.GetChild (1).GetComponent<Text> ().text = summoners [i].summonerName;

			summonerElementTemp.name = summoners [i].summonerName;
			summonerElementScript.summoner = summoners [i];
			summonerElementScript.characterType = 1;
			summonerElementScript.characterId = summoners [i].id;
		}
		for (int i = 0; i < enemies.Length; i++) {
			GameObject summonerElementTemp = Instantiate (this.characterElementPrefab, this.transform.GetChild (2).position, this.transform.rotation, this.transform.GetChild (2).GetChild (0)) as GameObject;
			PopupElement summonerElementScript = summonerElementTemp.GetComponent<PopupElement> ();

			summonerElementTemp.transform.GetChild (0).GetChild (0).GetComponent<Image>().sprite = Resources.Load<Sprite> ("character_avatars/" + enemies [i].idAvatar);
			summonerElementTemp.transform.GetChild (1).GetComponent<Text> ().text = enemies [i].summonerName;

			summonerElementTemp.name = enemies [i].summonerName;
			summonerElementScript.summoner = enemies [i];
			summonerElementScript.characterType = 3;
			summonerElementScript.characterId = enemies [i].id;
		}
		for (int i = 0; i < supports.Length; i++) {
			GameObject summonerElementTemp = Instantiate (this.characterElementPrefab, this.transform.GetChild (2).position, this.transform.rotation, this.transform.GetChild (2).GetChild (0)) as GameObject;
			PopupElement summonerElementScript = summonerElementTemp.GetComponent<PopupElement> ();

			summonerElementTemp.transform.GetChild (0).GetChild (0).GetComponent<Image>().sprite = Resources.Load<Sprite> ("character_avatars/" + supports [i].idAvatar);
			summonerElementTemp.transform.GetChild (1).GetComponent<Text> ().text = supports [i].supportName;

			summonerElementTemp.name = supports [i].supportName;
			summonerElementScript.support = supports [i];
			summonerElementScript.characterType = 2;
			summonerElementScript.characterId = supports [i].id;
		}
	}

	public void FilterCharacters (int newCharacterType) {
		for (int i = 0; i < this.transform.GetChild (2).GetChild (0).childCount; i++) {
			if (this.transform.GetChild (2).GetChild (0).GetChild (i).GetComponent<PopupElement> ().characterType == newCharacterType) {
				this.transform.GetChild (2).GetChild (0).GetChild (i).gameObject.SetActive (true);
			} else {
				this.transform.GetChild (2).GetChild (0).GetChild (i).gameObject.SetActive (false);
			}
		}
	}

	#region Generation de la carte popup
	public void GenerateCard (ActionsData newAction) {
		this.actionCard = newAction;
		this.GetComponent<CardPanel>().card = new Card (1, this.gameObject, 1, 1, newAction);
		this.name = this.actionCard.id.ToString();
		this.transform.GetChild(2).GetComponentInChildren<Text>().text = this.actionCard.name;
		this.transform.GetChild(3).GetComponentInChildren<Text>().text = DictionaryManager.GetCardDescription(this.GetComponent<CardPanel>().card);
		this.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite> ("skill_icons/" + this.actionCard.id);
		this.transform.GetChild(0).GetComponent<Image>().sprite = this.cardBackgrounds[this.actionCard.color];
		this.transform.GetChild(4).GetChild(0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("action_type_" + this.actionCard.type);
	}
	#endregion

	#region Generation de l'etat popup
	public void GenerateEtat (EtatsData newEtat) {
		this.etatInfos = newEtat;
		this.name = this.etatInfos.id.ToString();
		this.transform.GetChild(0).GetComponentInChildren<Text>().text = this.etatInfos.name;
		this.transform.GetChild(1).GetComponentInChildren<Text>().text = DictionaryManager.GetEtatDescription (this.etatInfos);
	}
	#endregion
}
