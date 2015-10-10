using UnityEngine;
using System.Collections;

public class WindHandler : MonoBehaviour {
    public Cloth cape;
    public Animator hair;

    Vector3 wind = Vector3.zero;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        float x = Vector3.Dot(wind, transform.forward);
        float z = Vector3.Dot(wind, transform.right);
        hair.SetFloat("WindZ", -x);
        hair.SetFloat("WindX", z);
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Environment")
        {
            ZoneScript zs = other.GetComponent<ZoneScript>();
            wind = zs.WindDIR;
            float x = Vector3.Dot(wind, transform.forward);
            float z = Vector3.Dot(wind, transform.right);
            hair.SetFloat("WindMagnitude", zs.MAGNITUDE);
            hair.SetFloat("WindZ", -x);
            hair.SetFloat("WindX", z);

            cape.externalAcceleration = zs.Wind;
            cape.randomAcceleration = zs.WindRANDOM;
        }
    }
}
