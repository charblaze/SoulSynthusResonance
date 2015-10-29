using UnityEngine;
using System.Collections;

public class Footsteps : MonoBehaviour {
    public AudioClip[] fsconcrete;
    public AudioClip[] fsextra;
    public float footSize = 0f;
	// Use this for initialization
	void Start () {
	
	}

    bool footstepped = false;
    void CheckForFootstep()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray, footSize, 1);
        for (int c = 0; c < hits.Length; ++c)
        {
            if (hits[c].transform.name == "Concrete")
            {
                if (!footstepped)
                {
                    int i = Random.Range(0, fsconcrete.Length - 1);
                    float f = Random.Range(0.1f, 0.3f);
                    AudioSource.PlayClipAtPoint(fsconcrete[i], transform.position, f);
                    i = Random.Range(0, fsextra.Length - 1);
                    f = Random.Range(0.15f, 0.6f);
                    AudioSource.PlayClipAtPoint(fsextra[i], transform.position, f);
                    footstepped = true;
                }
            } else
            {
                footstepped = false;
            }
        }
        if(hits.Length == 0)
        {
            footstepped = false;
        }
    }

	// Update is called once per frame
	void Update () {
        //Debug.DrawLine(transform.position, transform.position - Vector3.up * footSize, Color.red, 2f);
        CheckForFootstep();
	}
}
