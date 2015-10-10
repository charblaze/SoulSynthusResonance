using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponScript : MonoBehaviour
{
    // CONTAINS ALL UNIQUE WEAPON INFO.
    // To be added to all weapon Prefabs under /resources/weapons/[weaponname]/base

    public string WeaponName;
    public string WeaponSpecialDescrip;
    public string WeaponParentSheathed;
    public string WeaponParent;
    public Vector3 WeaponLocationSheathed;
    public Quaternion WeaponRotationSheathed;
    public Vector3 WeaponLocation;
    public Quaternion WeaponRotation;

    public float HeavyAttackModifier;
    public float ParryBonus;

    public int BASE_Te, BASE_Lu, BASE_So, BASE_In, BASE_Ch, BASE_Li, BASE_De;
    public char[] SCALING_Te, SCALING_Lu, SCALING_So, SCALING_In, SCALING_Ch, SCALING_Li, SCALING_De;

    public int TOTAL_Te, TOTAL_Lu, TOTAL_So, TOTAL_In, TOTAL_Ch, TOTAL_Li, TOTAL_De;

    public float BASEBLOCK_Te, BASEBLOCK_Lu, BASEBLOCK_So, BASEBLOCK_In, BASEBLOCK_Ch, BASEBLOCK_Li, BASEBLOCK_De;
    public char SCALINGBLOCK_Te, SCALINGBLOCK_Lu, SCALINGBLOCK_So, SCALINGBLOCK_In, SCALINGBLOCK_Ch, SCALINGBLOCK_Li, SCALINGBLOCK_De;
    public float TOTALBLOCK_Te, TOTALBLOCK_Lu, TOTALBLOCK_So, TOTALBLOCK_In, TOTALBLOCK_Ch, TOTALBLOCK_Li, TOTALBLOCK_De;

    public MultiDimensionalInt[] WeaponUpgradeScalingBASE;
    public MultiDimensionalChar[] WeaponUpgradeScalingSCALINGTE;
    public MultiDimensionalChar[] WeaponUpgradeScalingSCALINGLU;
    public MultiDimensionalChar[] WeaponUpgradeScalingSCALINGSO;
    public MultiDimensionalChar[] WeaponUpgradeScalingSCALINGIN;
    public MultiDimensionalChar[] WeaponUpgradeScalingSCALINGCH;
    public MultiDimensionalChar[] WeaponUpgradeScalingSCALINGLI;
    public MultiDimensionalChar[] WeaponUpgradeScalingSCALINGDE;

    public MultiDimensionalFloat[] WeaponUpgradeScalingBlockBASE;

    [System.Serializable]
    public class MultiDimensionalInt
    {
        public int[] c;
    }
    [System.Serializable]
    public class MultiDimensionalChar
    {
        public char[] c;
    }
    [System.Serializable]
    public class MultiDimensionalFloat
    {
        public float[] c;
    }

    // the actual damage this weapon deals with a swing
    public int TotalDamage;

    // weight of sword in kg
    public float Weight;

    public float STAMCOST_Light;
    public float STAMCOST_Heavy;
    public float STAMCOST_Charge;
    public float STAMCOST_Roll;
    public float STAMCOST_Special;

    bool isLethal = false;
    public GameObject mainNode;
    public GameObject[] nodes;

    // Contain all unique animations for this weapon
    public AnimationClip WeaponHoldTarget, WeaponHold, HeavyATK, HeavyATK2, LightATK, LightATK2, LightATK3, ChargeAttack, RollAttack, SpecialAttack, Sheathe, Unsheathe;
    Dictionary<GameObject, bool> hitGOs = new Dictionary<GameObject, bool>();

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

                    for(int d = 0; d < hits.Length; ++d)
                    {
                        // SPARK EFFECTS
                        if(hits[d].transform.tag == "Wall" || hits[d].transform.tag == "Floor")
                        {
                            Instantiate(Resources.Load("Effects/Sparks"), hits[d].point, transform.rotation);
                        }

                        // HITS
                        if(hits[d].transform.tag == "Enemy")
                        {
                            Character ch = hits[d].transform.gameObject.GetComponent<Character>();
                            // blood effects
                            GameObject efx = Instantiate(Resources.Load("Effects/WeaponSparks"), hits[d].point, transform.rotation) as GameObject;
                            efx.GetComponent<ParticleSystem>().startColor = ch.CharacterColor;

                            // if the enemy you hit is there 
                            if (hitGOs.ContainsKey(hits[d].transform.gameObject))
                            {
                                // do nothing .?
                            } else
                            {
                                hitGOs.Add(hits[d].transform.gameObject, true);
                                if(ch != null)
                                {
                                    print("HURT");
                                    CalculateAndDealDamageTo(ch, mainNode.transform.position);
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
                        if (hits[d].transform.tag == "Enemy")
                        {
                            Character ch = hits[d].transform.gameObject.GetComponent<Character>();
                            // blood effects
                            GameObject efx = Instantiate(Resources.Load("Effects/WeaponSparks"), hits[d].point, transform.rotation) as GameObject;
                            efx.GetComponent<ParticleSystem>().startColor = ch.CharacterColor;

                            // if the enemy you hit is there 
                            if (hitGOs.ContainsKey(hits[d].transform.gameObject))
                            {
                                // do nothing .?
                            }
                            else
                            {
                                hitGOs.Add(hits[d].transform.gameObject, true);
                                if (ch != null)
                                {
                                    print("HURT");
                                    CalculateAndDealDamageTo(ch, mainNode.transform.position);
                                }
                            }
                        }

                    }

                    // LASTLY, for things like rapiers, check if the point inside is in an enemy collider
                    Collider[] clds = Physics.OverlapSphere(mainNode.transform.position, 0.1f);
                    for(int d = 0; d < clds.Length; ++d)
                    {
                        // HITS
                        if (clds[d].transform.tag == "Enemy")
                        {
                            Character ch = clds[d].transform.gameObject.GetComponent<Character>();
                            // blood effects
                            GameObject efx = Instantiate(Resources.Load("Effects/WeaponSparks"), mainNode.transform.position, transform.rotation) as GameObject;
                            efx.GetComponent<ParticleSystem>().startColor = ch.CharacterColor;

                            // if the enemy you hit is there 
                            if (hitGOs.ContainsKey(clds[d].transform.gameObject))
                            {
                                // do nothing .?
                            }
                            else
                            {
                                hitGOs.Add(clds[d].transform.gameObject, true);
                                if (ch != null)
                                {
                                    print("HURT");
                                    CalculateAndDealDamageTo(ch, mainNode.transform.position);
                                }
                            }
                        }
                    }

                    //Physics.Raycast(nodes1[c], nodes[c].transform.position - nodes1[c], (nodes[c].transform.position - nodes1[c]).magnitude, 0);
                    //Debug.DrawLine(nodes1[c], nodes[c].transform.position, new Color(1, 1, 1), 1f, false);
                }
            } else
            {
                    hitGOs.Clear();
                
                yield return new WaitForEndOfFrame();
            }
        }
    }

    int SCALING(int c, char scaling)
    {
        int stat = 0;
        switch (c)
        {
            case 0: stat = p.TerrestrialRES; break;
            case 1: stat = p.LunarRES; break;
            case 2: stat = p.SolarRES; break;
            case 3: stat = p.InfernalRES; break;
            case 4: stat = p.ChillRES; break;
            case 5: stat = p.LifeRES; break;
            case 6: stat = p.DeathRES; break;
            default: break;
        }
        switch (scaling)
        {
            case 's': return Mathf.CeilToInt(706.997f - 721.116f * Mathf.Pow(2.71828f, -.0198026f * stat));
            case 'a': return Mathf.CeilToInt(353.488f - 360.558f * Mathf.Pow(2.71828f, -.0198026f * stat));
            case 'b': return Mathf.CeilToInt(201.993f - 206.033f * Mathf.Pow(2.71828f, -.0198026f * stat));
            case 'c': return Mathf.CeilToInt(100.997f - 103.017f * Mathf.Pow(2.71828f, -.0198026f * stat));
            case 'd': return Mathf.CeilToInt(50.4983f - 51.5083f * Mathf.Pow(2.71828f, -.0198026f * stat));
            default: return 0;
        }
    }

    public float SCALINGBLOCK(int stat, char scaling)
    {
        switch (scaling)
        {
            case 's': return (0.757475f - 0.772625f * Mathf.Pow(2.71828f, -.0198026f * stat));
            case 'a': return (0.757475f - 0.772625f * Mathf.Pow(2.71828f, -.0198026f * stat)) * (.5f);
            case 'b': return (0.757475f - 0.772625f * Mathf.Pow(2.71828f, -.0198026f * stat)) * (.25f);
            default: return 0;
        }
    }

    public void RecalculateBlocking()
    {
        TOTALBLOCK_Te = BASEBLOCK_Te + SCALINGBLOCK(p.TerrestrialRES, SCALINGBLOCK_Te);
        TOTALBLOCK_Lu = BASEBLOCK_Lu + SCALINGBLOCK(p.LunarRES, SCALINGBLOCK_Lu);
        TOTALBLOCK_So = BASEBLOCK_So + SCALINGBLOCK(p.SolarRES, SCALINGBLOCK_So);
        TOTALBLOCK_In = BASEBLOCK_In + SCALINGBLOCK(p.InfernalRES, SCALINGBLOCK_In);
        TOTALBLOCK_Ch = BASEBLOCK_Ch + SCALINGBLOCK(p.ChillRES, SCALINGBLOCK_Ch);
        TOTALBLOCK_Li = BASEBLOCK_Li + SCALINGBLOCK(p.LifeRES, SCALINGBLOCK_Li);
        TOTALBLOCK_De = BASEBLOCK_De + SCALINGBLOCK(p.DeathRES, SCALINGBLOCK_De);

    }

    public int LEVEL = 0;

    public void CheckForUpgraded()
    {
        if(LEVEL == 0)
        {
            return;
        }
        try
        {
            BASE_Te = WeaponUpgradeScalingBASE[LEVEL-1].c[0];
            BASE_Lu = WeaponUpgradeScalingBASE[LEVEL-1].c[1];
            BASE_So = WeaponUpgradeScalingBASE[LEVEL-1].c[2];
            BASE_In = WeaponUpgradeScalingBASE[LEVEL-1].c[3];
            BASE_Ch = WeaponUpgradeScalingBASE[LEVEL-1].c[4];
            BASE_Li = WeaponUpgradeScalingBASE[LEVEL-1].c[5];
            BASE_De = WeaponUpgradeScalingBASE[LEVEL-1].c[6];

            SCALING_Te = WeaponUpgradeScalingSCALINGTE[LEVEL-1].c;
            SCALING_Lu = WeaponUpgradeScalingSCALINGLU[LEVEL-1].c;
            SCALING_So = WeaponUpgradeScalingSCALINGSO[LEVEL-1].c;
            SCALING_In = WeaponUpgradeScalingSCALINGIN[LEVEL-1].c;
            SCALING_Ch = WeaponUpgradeScalingSCALINGCH[LEVEL-1].c;
            SCALING_Li = WeaponUpgradeScalingSCALINGLI[LEVEL-1].c;
            SCALING_De = WeaponUpgradeScalingSCALINGDE[LEVEL-1].c;

            BASEBLOCK_Te = WeaponUpgradeScalingBlockBASE[LEVEL-1].c[0];
            BASEBLOCK_Lu = WeaponUpgradeScalingBlockBASE[LEVEL-1].c[1];
            BASEBLOCK_So = WeaponUpgradeScalingBlockBASE[LEVEL-1].c[2];
            BASEBLOCK_In = WeaponUpgradeScalingBlockBASE[LEVEL-1].c[3];
            BASEBLOCK_Ch = WeaponUpgradeScalingBlockBASE[LEVEL-1].c[4];
            BASEBLOCK_Li = WeaponUpgradeScalingBlockBASE[LEVEL-1].c[5];
            BASEBLOCK_De = WeaponUpgradeScalingBlockBASE[LEVEL-1].c[6];
        }
        catch
        {
            print("Couldn't handle weapon upgrade");
        }
    }

    public void RecalculateDamage()
    {
        CheckForUpgraded();
        TOTAL_Te = BASE_Te;
        for(int c = 0; c < 7; ++c)
        {
            TOTAL_Te += SCALING(c, SCALING_Te[c]);
        }

        TOTAL_Lu = BASE_Lu;
        for (int c = 0; c < 7; ++c)
        {
            TOTAL_Lu += SCALING(c, SCALING_Lu[c]);
        }

        TOTAL_So = BASE_So;
        for (int c = 0; c < 7; ++c)
        {
            TOTAL_So += SCALING(c, SCALING_So[c]);
        }

        TOTAL_In = BASE_In;
        for (int c = 0; c < 7; ++c)
        {
            TOTAL_In += SCALING(c, SCALING_In[c]);
        }

        TOTAL_Ch = BASE_Ch;
        for (int c = 0; c < 7; ++c)
        {
            TOTAL_Ch += SCALING(c, SCALING_Ch[c]);
        }

        TOTAL_Li = BASE_Li;
        for (int c = 0; c < 7; ++c)
        {
            TOTAL_Li += SCALING(c, SCALING_Li[c]);
        }

        TOTAL_De = BASE_De;
        for (int c = 0; c < 7; ++c)
        {
            TOTAL_De += SCALING(c, SCALING_De[c]);
        }
    }

    void CalculateAndDealDamageTo(Character ch, Vector3 edge)
    {
        RecalculateDamage();
        if (p.ParryBonus)
        {
            ch.TakeAHit(Mathf.CeilToInt(TOTAL_Te * ParryBonus), Mathf.CeilToInt(TOTAL_Lu * ParryBonus), Mathf.CeilToInt(TOTAL_So * ParryBonus), Mathf.CeilToInt(TOTAL_In * ParryBonus), Mathf.CeilToInt(TOTAL_Ch * ParryBonus), Mathf.CeilToInt(TOTAL_Li * ParryBonus), Mathf.CeilToInt(TOTAL_De * ParryBonus), edge);
            return;
        }
        if (pm.isHeavyAttacking)
        {
            ch.TakeAHit(Mathf.CeilToInt(TOTAL_Te* HeavyAttackModifier), Mathf.CeilToInt(TOTAL_Lu* HeavyAttackModifier), Mathf.CeilToInt(TOTAL_So* HeavyAttackModifier), Mathf.CeilToInt(TOTAL_In* HeavyAttackModifier), Mathf.CeilToInt(TOTAL_Ch* HeavyAttackModifier), Mathf.CeilToInt(TOTAL_Li* HeavyAttackModifier), Mathf.CeilToInt(TOTAL_De* HeavyAttackModifier), edge);
        } else
        {
            ch.TakeAHit(TOTAL_Te, TOTAL_Lu, TOTAL_So, TOTAL_In, TOTAL_Ch, TOTAL_Li, TOTAL_De, edge);
        }
    }


    Player p;
    PlayerMovement pm;
    GameObject player;
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        p = player.GetComponent<Player>();
        pm = player.GetComponent<PlayerMovement>();

        p.EQUIPLOAD_weaponbonus = Weight;
        RecalculateBlocking();
        p.RecalculateStats();
        StartCoroutine(HitCor());
    }

    // Update is called once per frame
    void Update()
    {
        // total calculated damage varies depending on if you are strong attacking
        // or light attacking. update these modifiers here
        if (pm.WeaponIsHITTING)
        {
            isLethal = true;
        } else
        {
            isLethal = false;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        // spark effects for hitting a wall or the floor
        /*if (collision.transform.tag == "Wall" || collision.transform.tag == "Floor")
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Instantiate(Resources.Load("Effects/Sparks"), contact.point, transform.rotation);
            }
        }

        // blood effects
        if (collision.transform.tag == "Enemy")
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Instantiate(Resources.Load("Effects/Blood"), contact.point, transform.rotation);
            }
        }*/
        
        // check if you are hitting an enemy/*
        /*
        if (collision.transform.tag == "Enemy" && !pm.isParrying)
        {
            // all enemies must be characters
            Character c = collision.gameObject.GetComponent<Character>();
            c.TakeAHit(TotalDamage, transform.position);
        }*/
    }

    public void OnTriggerEnter(Collider other)
    {
        // check for successfuly parry
        if (other.tag == "Blade" && pm.isParrying)
        {
            // PARRY! Now you get invincibility frames and a counter attack
            p.GrantImmunity(1f);
            // put a huge spark on the sword
            Instantiate(Resources.Load("Effects/HugeSparks"), transform.position, transform.rotation);
            pm.ParryAnimEnd();
            pm.ParryEnd();
            pm.RollAttack();
        }
    }
    

    public int MaxCollisionsForStun = 6;

    // check if ur going ham on the wall
    public void OnCollisionStay(Collision collision)
    {
        // If too far in the wall
        if (collision.transform.tag == "Wall" || collision.transform.tag == "Floor")
        {
            if (collision.contacts.Length >= MaxCollisionsForStun)
            {
                pm.GetHurtAnimation(PlayerMovement.TakeDamageAnimationType.NORMAL, collision.contacts[0].point);
            }
        }
    }
}
