using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfosPanel : MonoBehaviour {

	public GameObject characterGameobject;
	public Summoner characterScript;
	public Support supportScript;
	private int characterType; // sert a savoir si j'affiche l'UI d'un support ou d'un summoner

	public Text characterName;
	public GameObject characterHealthBar;
	public GameObject dicesContainer;
	public GameObject etatsContainer;
	public GameObject defenseGameobject;

	public GameObject etatPrefab;

	public GameObject dicePrefab;
	public Sprite[] diceFaces;

	private bool diceGenerated;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	#region Attribution du Player & de la position de l'UI
	public void Init (GameObject newCharacter, int newCharacterType) {
		this.characterGameobject = newCharacter;
		this.characterType = newCharacterType;

		if (this.characterType == 1 || this.characterType == 3) { 
			this.characterScript = this.characterGameobject.GetComponent<Summoner> ();
			this.defenseGameobject.SetActive (false);
		} else if (this.characterType == 2) {
			this.supportScript = this.characterGameobject.GetComponent<Support> ();
			this.characterHealthBar.SetActive (false);
			this.characterName.gameObject.SetActive(false);
		}

		this.SetParent ();
		this.SetPlayerInfosPosition ();
		this.GenerateDices ();
		this.GenerateEtats ();
		this.UpdateInfos ();
	}

	private void SetParent () {
		this.transform.SetParent (GameObject.Find ("_CHARACTERS_INFOS").transform);
	}

	private void SetPlayerInfosPosition () //sert a position la vie et l'action au dessus du Gameobject de player
	{
		RectTransform canvasRect = GameObject.Find ("Canvas").GetComponent<RectTransform> ();

		Vector2 viewPortPosition = Camera.main.WorldToViewportPoint (this.characterGameobject.transform.position);
		Vector2 playerGameObjectToCanva = new Vector2 (
			((viewPortPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
			((viewPortPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
		this.GetComponent<RectTransform> ().anchoredPosition = playerGameObjectToCanva;
	}
	#endregion

	public void UpdateInfos () {
		if (this.characterType == 1 || this.characterType == 3) {
			this.characterName.text = this.characterScript.summonerName;
			this.characterHealthBar.GetComponentInChildren<Text> ().text = this.characterScript.pv.ToString () + " / " + this.characterScript.pvMax.ToString ();
			this.characterHealthBar.GetComponent<Slider> ().value = (float)this.characterScript.pv / (float)this.characterScript.pvMax;
			//this.UpdateEtats ();
			if (characterScript.defense > 0) {
				this.defenseGameobject.GetComponentInChildren<Text> ().text = this.characterScript.defense.ToString ();
				this.defenseGameobject.SetActive (true);
				this.BumpDefense ();
			} else {
				this.defenseGameobject.SetActive (false);
			}
		} else if (this.characterType == 2) {
			//this.characterName.text = this.supportScript.supportName;	
		}
	}

	public void GenerateEtats () {
		if (this.characterType == 1 || this.characterType == 3) {
			for (int i = 0; i < this.characterScript.L_etats.Count; i++) {
				if (this.characterScript.L_etats [i].isShowable) {
					this.AddEtat (this.characterScript.L_etats [i]);
				}
			}
		} else if (this.characterType == 2) {
			for (int i = 0; i < this.supportScript.L_etats.Count; i++) {
				if (this.supportScript.L_etats [i].isShowable) {
					this.AddEtat (this.supportScript.L_etats [i]);
				}
			}
		}
	}

	public void AddEtat (EtatsData newEtat) {
		if (this.characterType == 1 || this.characterType == 3) {
			GameObject etatPrefab = Instantiate (this.etatPrefab, this.transform.GetChild (1).transform.position, this.transform.GetChild (1).transform.rotation, this.etatsContainer.transform) as GameObject;
			etatPrefab.name = "etat_" + newEtat.id;
			etatPrefab.transform.GetChild (0).GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("etat_icons/" + newEtat.id);
			etatPrefab.transform.GetChild (1).GetComponentInChildren<Text> ().text = newEtat.duration.ToString ();
			etatPrefab.transform.GetChild (2).GetComponentInChildren<Text> ().text = newEtat.value.ToString ();

			if (newEtat.value == 0) {
				etatPrefab.transform.GetChild (2).gameObject.SetActive (false);
			} else {
				etatPrefab.transform.GetChild (2).gameObject.SetActive (true);
			}
			if (newEtat.duration < 0) {
				etatPrefab.transform.GetChild (1).gameObject.SetActive (false);
			} else {
				etatPrefab.transform.GetChild (1).gameObject.SetActive (true);
			}

			etatPrefab.GetComponent<PopupElement> ().etat = newEtat;
			this.EndBumpEtat (this.etatsContainer.transform.childCount - 1);
		} else if (this.characterType == 2) {
			GameObject etatPrefab = Instantiate (this.etatPrefab, this.transform.GetChild (1).transform.position, this.transform.GetChild (1).transform.rotation, this.etatsContainer.transform) as GameObject;
			etatPrefab.name = "etat_" + newEtat.id;
			etatPrefab.transform.GetChild (0).GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("etat_icons/" + newEtat.id);
			etatPrefab.transform.GetChild (1).GetComponentInChildren<Text> ().text = newEtat.duration.ToString ();
			etatPrefab.transform.GetChild (2).GetComponentInChildren<Text> ().text = newEtat.value.ToString ();

			if (newEtat.value == 0) {
				etatPrefab.transform.GetChild (2).gameObject.SetActive (false);
			} else {
				etatPrefab.transform.GetChild (2).gameObject.SetActive (true);
			}
			if (newEtat.duration <= 0) {
				etatPrefab.transform.GetChild (1).gameObject.SetActive (false);
			} else {
				etatPrefab.transform.GetChild (1).gameObject.SetActive (true);
			}

			etatPrefab.GetComponent<PopupElement> ().etat = newEtat;
			this.EndBumpEtat (this.etatsContainer.transform.childCount - 1);
		}
	}

	public void UpdateEtat (EtatsData newEtat) {
		if (this.characterType == 1 || this.characterType == 3) {
			for (int i = 0; i < this.etatsContainer.transform.childCount; i++) {
				if (this.etatsContainer.transform.GetChild (i).GetComponent<PopupElement> ().etat.id == newEtat.id) {
					this.etatsContainer.transform.GetChild (i).transform.GetChild (1).GetComponentInChildren<Text> ().text = newEtat.duration.ToString ();
					this.etatsContainer.transform.GetChild (i).transform.GetChild (2).GetComponentInChildren<Text> ().text = newEtat.value.ToString ();
					this.etatsContainer.transform.GetChild (i).GetComponent<PopupElement> ().etat = newEtat;

					if (newEtat.value == 0) {
						this.etatsContainer.transform.GetChild (i).transform.GetChild (2).gameObject.SetActive (false);
					} else {
						this.etatsContainer.transform.GetChild (i).transform.GetChild (2).gameObject.SetActive (true);
					}
					if (newEtat.duration < 0) {
						this.etatsContainer.transform.GetChild (i).transform.GetChild (1).gameObject.SetActive (false);
					} else {
						this.etatsContainer.transform.GetChild (i).transform.GetChild (1).gameObject.SetActive (true);
					}

					this.EndBumpEtat (i);
				}
			}
		} else if (this.characterType == 2) {
			for (int i = 0; i < this.etatsContainer.transform.childCount; i++) {
				if (this.etatsContainer.transform.GetChild (i).GetComponent<PopupElement> ().etat.id == newEtat.id) {
					this.etatsContainer.transform.GetChild (i).transform.GetChild (1).GetComponentInChildren<Text> ().text = newEtat.duration.ToString ();
					this.etatsContainer.transform.GetChild (i).transform.GetChild (2).GetComponentInChildren<Text> ().text = newEtat.value.ToString ();
					this.etatsContainer.transform.GetChild (i).GetComponent<PopupElement> ().etat = newEtat;

					if (newEtat.value == 0) {
						this.etatsContainer.transform.GetChild (i).transform.GetChild (2).gameObject.SetActive (false);
					} else {
						this.etatsContainer.transform.GetChild (i).transform.GetChild (2).gameObject.SetActive (true);
					}
					if (newEtat.duration < 0) {
						this.etatsContainer.transform.GetChild (i).transform.GetChild (1).gameObject.SetActive (false);
					} else {
						this.etatsContainer.transform.GetChild (i).transform.GetChild (1).gameObject.SetActive (true);
					}

					this.EndBumpEtat (i);
				}
			}
		}
	}

	public void RemoveEtat (EtatsData newEtat) {
		for (int i = 0; i < this.etatsContainer.transform.childCount; i++) {
			if (this.etatsContainer.transform.GetChild (i).GetComponent<PopupElement> ().etat.id == newEtat.id) {
				Destroy (this.etatsContainer.transform.GetChild (i).gameObject);
			}
		}
	}

	public void ShowDices () {
		for (int i = 0; i < this.dicesContainer.transform.childCount; i++) {
			this.dicesContainer.transform.GetChild (i).GetChild (1).gameObject.SetActive (false);
			this.dicesContainer.transform.GetChild (i).GetChild (2).gameObject.SetActive (false);
			this.dicesContainer.transform.GetChild (i).gameObject.SetActive (true);
		}
	}
	public void ShowBlockedDice (int newDiceId) {
		this.dicesContainer.transform.GetChild (newDiceId).GetChild (2).gameObject.SetActive (true);
	}
	public void HideBlockedDice (int newDiceId) {
		this.dicesContainer.transform.GetChild (newDiceId).GetChild (2).gameObject.SetActive (false);
	}

	public void HideDice (int newDiceId) {
		this.dicesContainer.transform.GetChild (newDiceId).gameObject.SetActive (false);		
	}

	public void UpdateDice (int newDiceId, Card newCard) {
		this.dicesContainer.transform.GetChild (newDiceId).GetComponent<ElementInfos> ().InitDice (newDiceId, newCard);
	}
	public void ShowActionDice (int newDiceId) {
		this.dicesContainer.transform.GetChild (newDiceId).GetChild (1).gameObject.SetActive (true);
	}

	public void ChangeDiceFace (int newDiceId, int newDiceValue) {
		this.dicesContainer.transform.GetChild(newDiceId).GetChild(0).GetComponent<Image> ().sprite = this.diceFaces [newDiceValue];
	}
	public void EndBumpDice (int newDiceId) {
		iTween.ScaleFrom (this.dicesContainer.transform.GetChild(newDiceId).gameObject, iTween.Hash(
			"scale", new Vector3 (1.5f, 1.5f, 1.5f), 
			"time", 0.5f, 
			"easetype", iTween.EaseType.easeOutBack));		
	}

	public void EndBumpEtat (int newIndex) {
		iTween.ScaleFrom (this.etatsContainer.transform.GetChild(newIndex).gameObject, iTween.Hash(
			"scale", new Vector3 (1.5f, 1.5f, 1.5f), 
			"time", 0.5f, 
			"easetype", iTween.EaseType.easeOutBack));		
	}

	public void BumpDefense () {
		iTween.ScaleFrom (this.defenseGameobject, iTween.Hash(
			"scale", new Vector3 (1.5f, 1.5f, 1.5f), 
			"time", 0.5f, 
			"easetype", iTween.EaseType.easeOutBack));	
	}

	public void GenerateDices () {
		if (this.characterType == 1 || this.characterType == 3) {
			for (int i = 0; i < this.characterScript.nbDice; i++) {
				GameObject dicePrefab = Instantiate (this.dicePrefab, this.transform.GetChild (1).transform.position, this.transform.GetChild (1).transform.rotation, this.dicesContainer.transform) as GameObject;

				if (this.characterType == 3) {
					dicePrefab.GetComponent<RectTransform> ().sizeDelta = new Vector2 (60f, 60f);
				}

				dicePrefab.name = "dice_" + i;
				dicePrefab.transform.GetChild (1).gameObject.SetActive (false);
				dicePrefab.SetActive (false);
			}
		} else if (this.characterType == 2) {
			for (int i = 0; i < this.supportScript.nbDice; i++) {
				GameObject dicePrefab = Instantiate (this.dicePrefab, this.transform.GetChild (1).transform.position, this.transform.GetChild (1).transform.rotation, this.dicesContainer.transform) as GameObject;
				dicePrefab.name = "dice_" + i;
				dicePrefab.transform.GetChild (1).gameObject.SetActive (false);
				dicePrefab.SetActive (false);
			}
		}
	}
}
