using UnityEngine;
using System.Collections;

public class Hemoglobin : MonoBehaviour {

    GameObject p;

    public float deltaTime, deleteTime;
    ParticleSystem[] ps = new ParticleSystem[2];

	// Use this for initialization
	void Start () {
        p = GameObject.FindGameObjectWithTag("Player") as GameObject;
        ps[0] = GetComponent<ParticleSystem>();
        ps[1] = transform.GetChild(0).GetComponent<ParticleSystem>();
        p.GetComponent<Player>().GainHealth(50);
        transform.position = p.transform.position;
        transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        gameObject.transform.SetParent(p.transform.FindChild("Buffs"));
	}
	


	// Update is called once per frame
	void Update () {
        deltaTime -= Time.deltaTime;
        deleteTime -= Time.deltaTime;
        if(deltaTime < 0)
        {
            ps[0].enableEmission = false;
            ps[1].enableEmission = false;
        }
        if(deleteTime < 0)
        {
            Destroy(gameObject);
        }
	}
}
