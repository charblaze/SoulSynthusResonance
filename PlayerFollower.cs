using UnityEngine;
using System.Collections;

public class PlayerFollower : MonoBehaviour {
    public GameObject Player;
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.Lerp(transform.position, Player.transform.position, Time.deltaTime * 3f);
	}
}
