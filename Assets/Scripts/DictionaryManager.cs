using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DictionaryManager {

	//public static Dictionary<int, Etat> dictionaryEtats = new Dictionary<int, Etat> ();
	public static Dictionary<int, ActionsData> actionsDictionary = new Dictionary<int, ActionsData> ();
	public static Dictionary<int, EtatsData> etatsDictionary = new Dictionary<int, EtatsData> ();

	public static bool isActionLoaded;
	public static bool isEtatsLoaded;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*public void LoadEtats ()
	{
		EtatCreator etatXML = XMLDeserializer.DeserializeEtat ();
		for (int i = 0; i < etatXML.etatsList.Count; i++) {
			dictionaryEtats.Add (etatXML.etatsList[i].ID, etatXML.etatsList[i]);
		}
	}*/

	public static void LoadActions ()
	{
		ActionsData[] actions = JsonSave.LoadActions ();
		for (int i = 0; i < actions.Length; i++) {
			actionsDictionary.Add (actions[i].id, actions[i]);
		}
		isActionLoaded = true;
	}

	public static void LoadEtats ()
	{
		EtatsData[] etats = JsonSave.LoadEtats ();
		for (int i = 0; i < etats.Length; i++) {
			etatsDictionary.Add (etats[i].id, etats[i]);
		}
		isEtatsLoaded = true;
	}

	#region Renvoyer l'action / l'etat depuis le dictionnaire
	public static ActionsData GetAction(int newActionId) {
		ActionsData action; 
		actionsDictionary.TryGetValue (newActionId, out action);
		ActionsData actionTemp = new ActionsData ();

		actionTemp.id = action.id;
		actionTemp.name = action.name;
		actionTemp.description = action.description;
		actionTemp.price = action.price;
		actionTemp.type = action.type;
		actionTemp.color = action.color;
		actionTemp.targetType = action.targetType;
		actionTemp.combatActions = new CombatActionData[action.combatActions.Length];

		actionTemp.upgradeRequirements = action.upgradeRequirements;

		for (int i = 0; i < action.combatActions.Length; i++) {
			actionTemp.combatActions [i].target = action.combatActions [i].target;
			actionTemp.combatActions [i].modulateur = action.combatActions [i].modulateur;
			actionTemp.combatActions [i].damage = action.combatActions [i].damage;
			actionTemp.combatActions [i].typeDmg = action.combatActions [i].typeDmg;
			actionTemp.combatActions [i].colorDmg = action.combatActions [i].colorDmg;
			actionTemp.combatActions [i].idAnim = action.combatActions [i].idAnim;
			actionTemp.combatActions [i].etats = new CombatEtatData[action.combatActions [i].etats.Length];

			for (int j = 0; j < action.combatActions [i].etats.Length; j++) {
				actionTemp.combatActions [i].etats [j].target = action.combatActions [i].etats [j].target;
				actionTemp.combatActions [i].etats [j].modulateur = action.combatActions [i].etats [j].modulateur;
				actionTemp.combatActions [i].etats [j].idEtat = action.combatActions [i].etats [j].idEtat;
				actionTemp.combatActions [i].etats [j].duration = action.combatActions [i].etats [j].duration;
				actionTemp.combatActions [i].etats [j].value = action.combatActions [i].etats [j].value;
			}

			actionTemp.combatActions [i].conditions = new ActionCondition[action.combatActions [i].conditions.Length];

			for (int j = 0; j < action.combatActions [i].conditions.Length; j++) {
				actionTemp.combatActions [i].conditions [j].target = action.combatActions [i].conditions [j].target;
				actionTemp.combatActions [i].conditions [j].type = action.combatActions [i].conditions [j].type;
				actionTemp.combatActions [i].conditions [j].mod = action.combatActions [i].conditions [j].mod;
				actionTemp.combatActions [i].conditions [j].value = action.combatActions [i].conditions [j].value;
			}

		} 

		return actionTemp;
	}

	public static EtatsData GetEtat(int newEtatId) {
		EtatsData etat; 
		etatsDictionary.TryGetValue (newEtatId, out etat);
		EtatsData etatTemp = new EtatsData ();

		etatTemp.id = etat.id;
		etatTemp.name = etat.name;
		etatTemp.description = etat.description;
		etatTemp.duration = etat.duration;
		etatTemp.value = etat.value;
		etatTemp.turnStateApply = etat.turnStateApply;
		etatTemp.isShowable = etat.isShowable;
		etatTemp.isRemovable = etat.isRemovable;

		return etatTemp;
	}
	#endregion

	#region Card Description
	public static string GetCardDescription (Card newCard) {
		//print (this.card.uId + " / " + this.card.tempAction.name);

		string actionDescription = newCard.tempAction.description;
		if (actionDescription.Contains("[dmg_all]")) {
			int damage = 0;
			for (int i = 0; i < newCard.tempAction.combatActions.Length; i++) {
				if (newCard.tempAction.combatActions [i].typeDmg == 1) {
					damage += newCard.tempAction.combatActions [i].damage;
				}
			}
			actionDescription = actionDescription.Replace ("[dmg_all]", "<b><size=20><color=#" + ColorUtility.ToHtmlStringRGBA (ActionColor(newCard.tempAction.color)) + ">" + Mathf.Abs(damage).ToString() + "</color></size></b>"); 
		}

		if (actionDescription.Contains ("[dmg_id_")) {
			do {
				char indexTemp = actionDescription [actionDescription.IndexOf ("[dmg_id_") + 8];
				string stringToReplace = "[dmg_id_" + indexTemp + actionDescription [actionDescription.IndexOf ("[dmg_id_") + 9];
				int damage = newCard.tempAction.combatActions [(int)char.GetNumericValue (indexTemp)].damage;
				actionDescription = actionDescription.Replace (stringToReplace, "<b><size=20><color=#" + ColorUtility.ToHtmlStringRGBA (ActionColor (newCard.tempAction.color)) + ">" + Mathf.Abs (damage).ToString () + "</color></size></b>"); 
			} while (actionDescription.Contains ("[dmg_id_"));
		}

		if (actionDescription.Contains("[dmg_count]")) {
			int damage = newCard.tempAction.combatActions.Length;
			actionDescription = actionDescription.Replace ("[dmg_count]", "<b><size=20>" + Mathf.Abs(damage).ToString() + "</size></b>"); 
		}

		if (actionDescription.Contains ("[etat_id_")) {
			do {
				char indexTemp = actionDescription [actionDescription.IndexOf ("[etat_id_") + 9];
				string stringToReplace = "[etat_id_" + indexTemp + actionDescription [actionDescription.IndexOf ("[etat_id_") + 10];
				int damage = newCard.tempAction.combatActions [(int)char.GetNumericValue (indexTemp)].etats [0].value;
				//print (indexTemp + " / " + stringToReplace + " / " + damage);
				actionDescription = actionDescription.Replace (stringToReplace, "<b><size=20><color=#" + ColorUtility.ToHtmlStringRGBA (ActionColor (newCard.tempAction.color)) + ">" + Mathf.Abs (damage).ToString () + "</color></size></b>"); 
			} while (actionDescription.Contains ("[etat_id_"));
		}

		if (actionDescription.Contains("[etat_count]")) {
			int nbEtat = 0;
			for (int i = 0; i < newCard.tempAction.combatActions.Length; i++) {
				nbEtat += newCard.tempAction.combatActions [i].etats.Length;
			}
			actionDescription = actionDescription.Replace ("[etat_count]", "<b><size=20>" + Mathf.Abs(nbEtat).ToString() + "</size></b>"); 
		}

		actionDescription = GetKeyWordDescription (actionDescription);

		return actionDescription;
	}
	#endregion

	public static string GetEtatDescription (EtatsData newEtat) {
		string etatDescription = newEtat.description;
		if (etatDescription.Contains("[value]")) {
			int value = 0;
			value = newEtat.value;
			etatDescription = etatDescription.Replace ("[value]", "<b><size=20><color=#" + ColorUtility.ToHtmlStringRGBA (ActionColor(5)) + ">" + Mathf.Abs(value).ToString() + "</color></size></b>"); 
		}

		etatDescription = GetKeyWordDescription (etatDescription);

		return etatDescription;
	}

	public static string GetKeyWordDescription (string newString) {
		string description = newString;

		if (description.Contains("[DEFENSE]")) {
			description = description.Replace ("[DEFENSE]", "<b>" + "Armures" + "</b>"); 
		}

		if (description.Contains("[FORCE]")) {
			description = description.Replace ("[FORCE]", "<b>" + "Force" + "</b>"); 
		}

		if (description.Contains("[ROBUSTESSE]")) {
			description = description.Replace ("[ROBUSTESSE]", "<b>" + "Robustesse" + "</b>"); 
		}

		if (description.Contains("[FAIBLESSE]")) {
			description = description.Replace ("[FAIBLESSE]", "<b>" + "Faiblesse" + "</b>"); 
		}

		if (description.Contains("[ENTRAVE]")) {
			description = description.Replace ("[ENTRAVE]", "<b>" + "Entrave" + "</b>"); 
		}

		if (description.Contains("[ROUILLE]")) {
			description = description.Replace ("[ROUILLE]", "<b>" + "Rouillé" + "</b>"); 
		}

		return description; 
	}

	public static List<string> GetKeywords (ActionsData newAction) {
		List<string> keywords = new List<string> ();
		if (newAction.description.Contains("[DEFENSE]")) {
			string tempString;
			tempString = "Défense: permet de réduire les dégats reçus";
			keywords.Add (tempString);
		}

		if (newAction.description.Contains("[FORCE]")) {
			string tempString;
			tempString = GetEtat (10).description;
			keywords.Add (tempString);
		}

		return keywords;
	}

	public static Color32 ActionColor (int newInt) {
		Color32 newColor;
		switch (newInt) 
		{
		case 0:
			newColor = new Color32 (255, 255, 255, 255);	// blanc
			break;
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
		case 5:
			newColor = new Color32 (154, 154, 154, 255);	// gris
			break;
		default:
			newColor = new Color32 (212, 211, 211, 255);	// blanc
			break;
		}
		return newColor;
	}
}