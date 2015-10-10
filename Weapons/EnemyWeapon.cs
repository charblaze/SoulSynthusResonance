using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyWeapon : MonoBehaviour
{
    // COMPONENT to be added to all lethal parts of enemies

    public Collider col;
    public float damage;
    public bool isLethal;
    public GameObject[] nodes;
    public GameObject mainNode;
    public bool AlreadyHit;

    public int[] DamageDoing;

    // Use this for initialization
    void Start()
    {
        col = GetComponent<Collider>();
        StartCoroutine(HitCor());
    }

    void HITEM(Character ch)
    {
        if (!ch.immunity)
        {
            PlayerMovement pm = ch.gameObject.GetComponent<PlayerMovement>();
            if (pm.isParrying)
            {
                Instantiate(Resources.Load("Effects/Sparks"),mainNode.transform.position, transform.rotation);

                Instantiate(Resources.Load("Effects/Sparks"), mainNode.transform.position, transform.rotation);

                Instantiate(Resources.Load("Effects/Sparks"), mainNode.transform.position, transform.rotation);

                Instantiate(Resources.Load("Effects/Sparks"), mainNode.transform.position, transform.rotation);
                // PARRY! Now you get invincibility frames and a counter attack
                ch.GrantImmunity(1f);
                // put a huge spark on the sword
                Instantiate(Resources.Load("Effects/HugeSparks"), transform.position, transform.rotation);
                pm.ParryAnimEnd();
                pm.ParryEnd();
                ch.ParryBonus = true;
                pm.RollAttack();
            }
            else
            {
                ch.TakeAHit(DamageDoing[0], DamageDoing[1], DamageDoing[2], DamageDoing[3], DamageDoing[4], DamageDoing[5], DamageDoing[6], mainNode.transform.position);
            }
        }
    }


    IEnumerator HitCor()
    {
        while (true)
        {
            if (isLethal)
            {// EACH FRAME
                List<Vector3> nodes1 = new List<Vector3>();
                for (int c = 0; c < nodes.Length; ++c)
                {
                    nodes1.Add(nodes[c].transform.position);
                }
                yield return new WaitForEndOfFrame();
                // now draw raycasts from prev pos to current pos
                for (int c = 0; c < nodes.Length; ++c)
                {
                    Ray ray = new Ray(nodes1[c], nodes[c].transform.position - nodes1[c]);
                    RaycastHit[] hits = Physics.RaycastAll(ray, (nodes[c].transform.position - nodes1[c]).magnitude, 1);

                    for (int d = 0; d < hits.Length; ++d)
                    {
                        // SPARK EFFECTS
                        if (hits[d].transform.tag == "Wall" || hits[d].transform.tag == "Floor")
                        {
                            Instantiate(Resources.Load("Effects/Sparks"), hits[d].point, transform.rotation);
                        }

                        // HITS
                        if (hits[d].transform.tag == "Player"   && hits[d].collider.name == "HURTBOX")
                        {
                            Player ch = hits[d].transform.gameObject.GetComponent<Player>();
                            // blood effects
                            if (!ch.immunity)
                            {
                                GameObject efx = Instantiate(Resources.Load("Effects/WeaponSparks"), hits[d].point, transform.rotation) as GameObject;
                                efx.GetComponent<ParticleSystem>().startColor = ch.CharacterColor;
                            }
                            // if the enemy you hit is there 
                            if (AlreadyHit)
                            {
                                // do nothing .?
                            }
                            else
                            {
                                AlreadyHit = true;
                                if (ch != null)
                                {
                                    HITEM(ch);
                                }
                            }
                        }

                    }
                    // REVERSE RAY
                    ray = new Ray(nodes[c].transform.position - nodes1[c], nodes1[c]);
                    hits = Physics.RaycastAll(ray, (nodes[c].transform.position - nodes1[c]).magnitude, 1);

                    for (int d = 0; d < hits.Length; ++d)
                    {
                        // SPARK EFFECTS
                        if (hits[d].transform.tag == "Wall" || hits[d].transform.tag == "Floor")
                        {
                            Instantiate(Resources.Load("Effects/Sparks"), hits[d].point, transform.rotation);
                        }

                        // HITS
                        if (hits[d].transform.tag == "Player" && hits[d].collider.name == "HURTBOX")
                        {
                            Player ch = hits[d].transform.gameObject.GetComponent<Player>();
                            // blood effects
                            if (!ch.immunity)
                            {
                                GameObject efx = Instantiate(Resources.Load("Effects/WeaponSparks"), hits[d].point, transform.rotation) as GameObject;
                                efx.GetComponent<ParticleSystem>().startColor = ch.CharacterColor;
                            }
                            // if the enemy you hit is there 
                            if (AlreadyHit)
                            {
                                // do nothing .?
                            }
                            else
                            {
                                AlreadyHit = true;
                                if (ch != null)
                                {
                                    HITEM(ch);
                                }
                            }
                        }

                    }

                    // LASTLY, for things like rapiers, check if the point inside is in an enemy collider
                    Collider[] clds = Physics.OverlapSphere(mainNode.transform.position, 0.1f);
                    for (int d = 0; d < clds.Length; ++d)
                    {
                        // HITS
                        if (clds[d].transform.tag == "Player" && clds[d].name == "HURTBOX")
                        {
                            Player ch = clds[d].transform.gameObject.GetComponent<Player>();
                            // blood effects
                            if (!ch.immunity)
                            {
                                GameObject efx = Instantiate(Resources.Load("Effects/WeaponSparks"), mainNode.transform.position, transform.rotation) as GameObject;
                                efx.GetComponent<ParticleSystem>().startColor = ch.CharacterColor;
                            }
                            // if the enemy you hit is there 
                            if (AlreadyHit)
                            {
                                // do nothing .?
                            }
                            else
                            {
                                AlreadyHit = true;
                                if (ch != null)
                                {
                                    HITEM(ch);
                                }
                            }
                        }
                    }

                    //Physics.Raycast(nodes1[c], nodes[c].transform.position - nodes1[c], (nodes[c].transform.position - nodes1[c]).magnitude, 0);
                    //Debug.DrawLine(nodes1[c], nodes[c].transform.position, new Color(1, 1, 1), 1f, false);
                }
            }
            else
            {
                AlreadyHit = false;

                yield return new WaitForEndOfFrame();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
