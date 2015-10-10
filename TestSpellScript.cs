using UnityEngine;
using System.Collections;

public class TestSpellScript : MonoBehaviour {
    Rigidbody rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();

        rb.velocity = transform.forward * 40;
        StartCoroutine(ohno());
    }
	
    IEnumerator ohno()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

	// Update is called once per frame
	void Update () {
	}
}
