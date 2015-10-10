using UnityEngine;
using System.Collections;

public class FadeScript : MonoBehaviour {

    public float duration;

    IEnumerator animate()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(animate());
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f) * Time.deltaTime;
	}
}
