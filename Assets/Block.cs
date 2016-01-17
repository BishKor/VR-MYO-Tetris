using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {

	public string state; // possible states are "player", "filled", "empty"

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void SetState(string argState){
		state = argState;
		switch (argState)
		{
		case "empty":
			this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.clear);
			break;
		case "filled":
			this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
			break;
		case "active":
			this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
			break;
		}
		Debug.Log (state);
	}
	
	public string GetState(){
		return state;
	}
}
