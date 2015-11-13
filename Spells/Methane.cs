using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Methane : MonoBehaviour
{

    GameObject p;

    public float deltaTime, deleteTime;
    public ParticleSystem[] ps;

    Dictionary<GameObject, bool> dict = new Dictionary<GameObject, bool>();

    public Collider col;

    public int[] Damage;

    public int ScalingFactor;

    public Vector3 RotOrigin;

    public float foffset, uoffset;
    

    // Use this for initialization
    void Start()
    {
        p = GameObject.FindGameObjectWithTag("Player") as GameObject;
        if (RotOrigin != Vector3.zero)
        {
            transform.rotation = Quaternion.Euler(RotOrigin);
        }
        gameObject.transform.SetParent(p.transform);

        // spell power
        float sp = p.GetComponent<Player>().spellPower;

        Damage[ScalingFactor] = Mathf.CeilToInt((float)(Damage[ScalingFactor]) *sp);

    }



    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            if (!dict.ContainsKey(other.gameObject)) 
            {
                Enemy e = other.GetComponent<Enemy>();
                e.TakeAHit(Damage[0], Damage[1], Damage[2], Damage[3], Damage[4], Damage[5], Damage[6], transform.position + transform.forward * foffset + transform.up * uoffset);
                dict[other.gameObject] = true;
            } else
            {

            }
        }
    }





    // Update is called once per frame
    void Update()
    {
        deltaTime -= Time.deltaTime;
        deleteTime -= Time.deltaTime;
        if (deltaTime < 0)
        {
            for(int c = 0; c < ps.Length; ++c)
            {
                ps[c].enableEmission = false;
            }
            col.enabled = false;
        }
        if (deleteTime < 0)
        {
            Destroy(gameObject);
        }
    }
}
