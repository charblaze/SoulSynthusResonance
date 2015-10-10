using UnityEngine;
using System.Collections;

public class HatScript : MonoBehaviour {
    public string name;
    public float weight;
    public Vector3 pos;
    public Quaternion rot;

    GameObject player;
    Player p;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        p = player.GetComponent<Player>();

        p.EQUIPLOAD_hatbonus = weight;
        p.RecalculateStats();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
