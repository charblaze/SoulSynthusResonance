using UnityEngine;
using System.Collections;

public class SparksScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(ha());
	}
	
    IEnumerator ha()
    {

        gameObject.GetComponent<ParticleSystem>().enableEmission = true;
        yield return new WaitForSeconds(0.4f);
        gameObject.GetComponent<ParticleSystem>().enableEmission = false;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

	// Update is called once per frame
	void Update () {
	
	}
}
