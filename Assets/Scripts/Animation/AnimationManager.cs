using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager: MonoBehaviour {

	private GameObject launcherGO;
	private GameObject targetGO;
	//private Summoner launcherScript;

	private GameObject actionObject;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GenerateAnimation (GameObject newLauncher, GameObject newTarget, string newAnimId) {
		this.launcherGO = newLauncher;
		this.targetGO = newTarget;
		//this.launcherScript = this.launcherGO.GetComponent<Summoner> ();

		this.actionObject = Instantiate (Resources.Load ("anim/anim_" + newAnimId) as GameObject, this.transform.position, this.transform.rotation, this.transform) as GameObject;

		this.transform.position = this.targetGO.transform.position;
		StartCoroutine ("Anim_" + newAnimId);
	}

	public IEnumerator Anim_1 () {

		this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1f, this.transform.position.z);

		this.launcherGO.GetComponent<ActionManager>().animationIsPlaying = false;

		yield return new WaitForSeconds (0.1f);

		iTween.ShakePosition (GameObject.Find("Main Camera"), new Vector3(0.10f, 0.10f, 0f), 0.15f);

		yield return new WaitForSeconds (0.2f);

		Destroy (this.gameObject);
		yield return null;
	}

	public IEnumerator Anim_2 () {
		
		this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1f, this.transform.position.z);

		iTween.FadeTo (this.actionObject, iTween.Hash(
			"alpha", 1f, 
			"time", 0.2f, 
			"easetype", iTween.EaseType.easeOutBack));

		this.launcherGO.GetComponent<ActionManager>().animationIsPlaying = false;

		iTween.MoveTo (this.actionObject, iTween.Hash(
			"position", new Vector3 (this.transform.position.x, this.transform.position.y - 0.5f, this.transform.position.z), 
				"time", 0.8f, 
			"easetype", iTween.EaseType.easeOutBack));
		
		yield return new WaitForSeconds (0.8f);
		
		iTween.MoveTo (this.actionObject, iTween.Hash(
			"position", new Vector3 (this.transform.position.x, this.transform.position.y - 0.2f, this.transform.position.z), 
			"time", 0.3f, 
			"easetype", iTween.EaseType.easeOutBack));

		iTween.FadeTo (this.actionObject, iTween.Hash(
			"alpha", 0f, 
			"time", 0.3f, 
			"easetype", iTween.EaseType.easeOutBack));

		yield return new WaitForSeconds (0.3f);

		Destroy (this.gameObject);
		yield return null;
	}

	public IEnumerator Anim_3 () {

		this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.3f, this.transform.position.z);

		iTween.FadeTo (this.actionObject, iTween.Hash(
			"alpha", 1f, 
			"time", 0.01f, 
			"easetype", iTween.EaseType.easeOutBack));

		this.launcherGO.GetComponent<ActionManager>().animationIsPlaying = false;
		
		iTween.ShakePosition (GameObject.Find("Main Camera"), new Vector3(0.10f, 0.10f, 0f), 0.15f);
		
		yield return new WaitForSeconds (0.1f);

		iTween.FadeTo (this.actionObject, iTween.Hash(
			"alpha", 0f, 
			"time", 0.7f, 
			"easetype", iTween.EaseType.easeOutBack));

		yield return new WaitForSeconds (0.1f);

		yield return new WaitForSeconds (0.6f);

		Destroy (this.gameObject);
		yield return null;
	}

	public IEnumerator Anim_4 () {

		this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.3f, this.transform.position.z);

		iTween.FadeTo (this.actionObject, iTween.Hash(
			"alpha", 1f, 
			"time", 0.01f, 
			"easetype", iTween.EaseType.easeOutBack));

		this.launcherGO.GetComponent<ActionManager>().animationIsPlaying = false;

		iTween.ShakePosition (GameObject.Find("Main Camera"), new Vector3(0.10f, 0.10f, 0f), 0.15f);

		yield return new WaitForSeconds (0.1f);

		iTween.FadeTo (this.actionObject, iTween.Hash(
			"alpha", 0f, 
			"time", 0.7f, 
			"easetype", iTween.EaseType.easeOutBack));

		yield return new WaitForSeconds (0.6f);

		Destroy (this.gameObject);
		yield return null;
	}
}
