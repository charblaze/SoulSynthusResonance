using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class Player : Character
{
    // Other Player Components
    PlayerMovement pm;
    PlayerInventory pi;

    // PLAYER STATS
    // A number that represents the equip load number where a player moves normally (median). Can be upgraded
    public float EquipLoad_Normal;
    // represents the total weight of equipment currently equipped
    public float EquipLoad_Current;

    // NUMBER FROM 0 TO 1 (usually close to 1) that shortens the effect of certain stuns on the player, like landing lag and hit lag
    // PREFER to not be lower than 0.5, and cannot at all be lower than ~0.2
    public float Tenacity = 1f;

    // float THAT is usually close to 1. Represents the speed of roll animations. Affected by stats and current equipload
    // CANNOT BE LOWER THAN 0.8 (extremely fast), CANNOT BE HIGHER THAN 2 (fat rolling)
    public float Swiftness = 1.5f;

    // name of currently equipped weapon. Must be a valid name
    public string EquippedWeapon;

    public string EquippedHat, EquippedAmulet, EquippedTunic, EquippedRing1, EquippedRing2, EquippedArmor, EquippedCape;

    // Multiplied by all spell base damage. RAISES HIGHEST STAT OF SPELL
    public float spellPower = 1f;

    // Head reference for placing Hats
    public Transform head;
    // Buffs reference for placing Buffs
    public Transform buffs;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Use this for initialization
    void Start()
    {
        // test starting class : The Warrior
        pi = GetComponent<PlayerInventory>();
        BaseHealth = 250;
        BaseStamina = 20;
        /*
        // NOBLE
        TerrestrialRES = 6;
        LunarRES = 6;
        SolarRES = 6;
        InfernalRES = 4;
        ChillRES = 3;
        LifeRES = 2;
        DeathRES = 1;

        // REAPER
        TerrestrialRES = 2;
        LunarRES = 2;
        SolarRES = 2;
        InfernalRES = 5;
        ChillRES = 6;
        LifeRES = 6;
        DeathRES = 6;
        */
        // WARRIOR 
        TerrestrialRES = 6;
        LunarRES = 3;
        SolarRES = 3;
        InfernalRES = 4;
        ChillRES = 3;
        LifeRES = 35;
        DeathRES = 1;
        CurrentStamina = MaximumStamina;
        EquippedWeapon = "Great Scythe";
        EquippedHat = "Sancta Hat";
        EquippedCape = "White Banner";
        EquippedRing1 = "None";
        EquippedRing2 = "None";
        EquippedArmor = "Tightened Steel Plate";
        EquippedAmulet = "None";
        CurrentHealth = MaximumHealth;
        pm = GetComponent<PlayerMovement>();
        LoadHat();
        pi.AddWeapon("Great Scythe");
        pi.AddWeapon("Edged Rapier", 1);
        pi.AddWeapon("Claymore");
        pi.AddHat("None");
        pi.AddAmulet("None");
        pi.AddRing("None");
        pi.AddRing("None");
        pi.AddHat("Regia Hat");
        pi.AddHat("Sancta Hat");
        pi.AddAmulet("Sapphire Amulet");
        pi.AddAmulet("Ruby Amulet");
        pi.AddRing("Ruby Ring");
        pi.AddRing("Ruby Ring");
        pi.AddTunic("Chain Curiass");
        pi.AddArmor("Tightened Steel Plate");
        pi.AddArmor("Magister's Regalia");
        pi.AddCape("None");
        pi.AddCape("White Banner");
        pi.AddCape("Black Banner");
        pi.AddHat("Magus Hood");

        //LoadArmor();
    }

    // INSTANCE OF CURRENTLY EQUIPPED HAT
    public GameObject Hat, Hair;

    // location of neck
    public Transform Neck;

    /// <summary>
    /// Reloads the player's hat that is currently equipped
    /// </summary>
    public void LoadHat()
    {
        if(EquippedHat == null)
        {
            return;
        }
        if(Hat != null)
        {
            Destroy(Hat);
        }
        Hat = Instantiate(Resources.Load("Hats/" + EquippedHat)) as GameObject;
        Hat.transform.SetParent(head, false);
        HatScript h = Hat.GetComponent<HatScript>();
        Hat.transform.localPosition = h.pos;
        Hat.transform.localRotation = h.rot;
        if (h.hideshair)
        {
            Hair.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled = false;
            //Hair.SetActive(false);
        } else
        {
            Hair.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled = true;

            //Hair.SetActive(true);
        }
    }

    public void LoadArmor()
    {
        if (EquippedArmor == null || EquippedArmor == "None")
        {
            return;
        }
        GameObject MODELr = Instantiate(Resources.Load("Armors/" + EquippedArmor)) as GameObject;
        Equipm e = MODELr.transform.GetChild(0).gameObject.GetComponent<Equipm>();
        e.target = MODEL.transform.GetChild(0).gameObject;
        MODELr.transform.GetChild(1).gameObject.SetActive(false);
        MODELr.transform.SetParent(gameObject.transform);
        MODELr.transform.position = MODEL.transform.position;
        MODELr.transform.rotation = MODEL.transform.rotation;
        e.AssignBones();
        if (MODEL != null)
        {
            Destroy(MODEL);
        }
        MODEL = MODELr;
    }

    public void LoadCape()
    {
        if(EquippedCape == null)
        {
            return;
        }
        GameObject cap = Instantiate(Resources.Load("Capes/" + EquippedCape)) as GameObject;
        cap.transform.SetParent(Neck);
        cap.transform.position = CAPE.transform.position;
        cap.transform.rotation = CAPE.transform.rotation;
        cap.GetComponent<Cloth>().capsuleColliders = COLLS;
        Destroy(CAPE);
        CAPE = cap;
        GetComponent<WindHandler>().cape = CAPE.GetComponent<Cloth>();
        GetComponent<WindHandler>().NewCape();
    }

    // INSTANCES OF PLAYERS EQUIPMENT
    public GameObject Amulet, Tunic, Ring1, Ring2, MODEL, CAPE;

    // COLLIDERS FOR CAPES AND HITS
    public CapsuleCollider[] COLLS;

    /// <summary>
    /// Reloads the player's amulet that is currently equipped
    /// </summary>
    public void LoadAmulet()
    {
        if (EquippedAmulet == null || EquippedAmulet == "None")
        {
            Destroy(Amulet);
            return;
        }
        if (Amulet != null)
        {
            Destroy(Amulet);
        }
        Amulet = Instantiate(Resources.Load("Amulets/" + EquippedAmulet)) as GameObject;
        Amulet.transform.SetParent(buffs, false);
    }

    /// <summary>
    /// Reloads the player's tunic that is currently equipped
    /// </summary>
    public void LoadTunic()
    {
        if (EquippedTunic == null || EquippedTunic == "None")
        {
            Destroy(Tunic);
            return;
        }
        if (Tunic != null)
        {
            Destroy(Tunic);
        }
        Tunic = Instantiate(Resources.Load("Tunics/" + EquippedTunic)) as GameObject;
        Tunic.transform.SetParent(buffs, false);
    }

    /// <summary>
    /// Reloads the player's 1st ring slot that is currently equipped
    /// </summary>
    public void LoadRing1()
    {
        if (EquippedRing1 == null || EquippedRing1 == "None")
        {
            Destroy(Ring1);
            return;
        }
        if (Ring1 != null)
        {
            Destroy(Ring1);
        }
        Ring1 = Instantiate(Resources.Load("Rings/" + EquippedRing1)) as GameObject;
        Ring1.transform.SetParent(buffs, false);
    }

    /// <summary>
    /// Reloads the player's second ring slot that is currently equipped
    /// </summary>
    public void LoadRing2()
    {
        if (EquippedRing2 == null || EquippedRing2 == "None")
        {
            Destroy(Ring2);
            return;
        }
        if (Ring2 != null)
        {
            Destroy(Ring2);
        }
        Ring2 = Instantiate(Resources.Load("Rings/" + EquippedRing2)) as GameObject;
        Ring2.transform.SetParent(buffs, false);
    }

    // any and all health bonuses
    public int HEALTH_weaponbonus = 0;
    public int HEALTH_armorbonus = 0;
    public int HEALTH_hatbonus = 0;
    public int HEALTH_ringbonus = 0;
    public int HEALTH_amuletbonus = 0;
    public int HEALTH_buffbonus = 0;

    public float STAMINA_weaponbonus = 0;
    public float STAMINA_armorbonus = 0;
    public float STAMINA_hatbonus = 0;
    public float STAMINA_ringbonus = 0;
    public float STAMINA_amuletbonus = 0;
    public float STAMINA_buffbonus = 0;

    public float HEALTHREGENweaponbonus = 0;
    public float HEALTHREGENarmorbonus = 0;
    public float HEALTHREGENhatbonus = 0;
    public float HEALTHREGENringbonus = 0;
    public float HEALTHREGENamuletbonus = 0;
    public float HEALTHREGENbuffbonus = 0;

    public float STAMINAREGEN_weaponbonus = 0;
    public float STAMINAREGEN_armorbonus = 0;
    public float STAMINAREGEN_hatbonus = 0;
    public float STAMINAREGEN_ringbonus = 0;
    public float STAMINAREGEN_amuletbonus = 0;
    public float STAMINAREGEN_buffbonus = 0;

    public float EQUIPLOAD_weaponbonus = 0;
    public float EQUIPLOAD_armorbonus = 0;
    public float EQUIPLOAD_hatbonus = 0;
    public float EQUIPLOAD_ringbonus = 0;
    public float EQUIPLOAD_amuletbonus = 0;
    public float EQUIPLOAD_buffbonus = 0;

    public float MAXEQUIPLOAD_weaponbonus = 0;
    public float MAXEQUIPLOAD_armorbonus = 0;
    public float MAXEQUIPLOAD_hatbonus = 0;
    public float MAXEQUIPLOAD_ringbonus = 0;
    public float MAXEQUIPLOAD_amuletbonus = 0;
    public float MAXEQUIPLOAD_buffbonus = 0;

    public float RUNSPEED_weaponbonus = 0;
    public float RUNSPEED_armorbonus = 0;
    public float RUNSPEED_hatbonus = 0;
    public float RUNSPEED_ringbonus = 0;
    public float RUNSPEED_amuletbonus = 0;
    public float RUNSPEED_buffbonus = 0;

    public float POISONRESIST_weaponbonus = 0;
    public float POISONRESIST_armorbonus = 0;
    public float POISONRESIST_hatbonus = 0;
    public float POISONRESIST_ringbonus = 0;
    public float POISONRESIST_amuletbonus = 0;
    public float POISONRESIST_buffbonus = 0;

    public float SSRESIST_weaponbonus = 0;
    public float SSRESIST_armorbonus = 0;
    public float SSRESIST_hatbonus = 0;
    public float SSRESIST_ringbonus = 0;
    public float SSRESIST_amuletbonus = 0;
    public float SSRESIST_buffbonus = 0;

    public float TENACITY_weaponbonus = 0;
    public float TENACITY_armorbonus = 0;
    public float TENACITY_hatbonus = 0;
    public float TENACITY_ringbonus = 0;
    public float TENACITY_amuletbonus = 0;
    public float TENACITY_buffbonus = 0;

    public float SWIFNTESS_weaponbonus = 0;
    public float SWIFNTESS_armorbonus = 0;
    public float SWIFNTESS_hatbonus = 0;
    public float SWIFNTESS_ringbonus = 0;
    public float SWIFNTESS_amuletbonus = 0;
    public float SWIFNTESS_buffbonus = 0;

    public float TRESIST_weaponbonus = 0;
    public float TRESIST_armorbonus = 0;
    public float TRESIST_hatbonus = 0;
    public float TRESIST_ringbonus = 0;
    public float TRESIST_amuletbonus = 0;
    public float TRESIST_buffbonus = 0;

    public float LURESIST_weaponbonus = 0;
    public float LURESIST_armorbonus = 0;
    public float LURESIST_hatbonus = 0;
    public float LURESIST_ringbonus = 0;
    public float LURESIST_amuletbonus = 0;
    public float LURESIST_buffbonus = 0;

    public float SORESIST_weaponbonus = 0;
    public float SORESIST_armorbonus = 0;
    public float SORESIST_hatbonus = 0;
    public float SORESIST_ringbonus = 0;
    public float SORESIST_amuletbonus = 0;
    public float SORESIST_buffbonus = 0;

    public float INRESIST_weaponbonus = 0;
    public float INRESIST_armorbonus = 0;
    public float INRESIST_hatbonus = 0;
    public float INRESIST_ringbonus = 0;
    public float INRESIST_amuletbonus = 0;
    public float INRESIST_buffbonus = 0;

    public float CHRESIST_weaponbonus = 0;
    public float CHRESIST_armorbonus = 0;
    public float CHRESIST_hatbonus = 0;
    public float CHRESIST_ringbonus = 0;
    public float CHRESIST_amuletbonus = 0;
    public float CHRESIST_buffbonus = 0;

    public float LIRESIST_weaponbonus = 0;
    public float LIRESIST_armorbonus = 0;
    public float LIRESIST_hatbonus = 0;
    public float LIRESIST_ringbonus = 0;
    public float LIRESIST_amuletbonus = 0;
    public float LIRESIST_buffbonus = 0;

    public float DERESIST_weaponbonus = 0;
    public float DERESIST_armorbonus = 0;
    public float DERESIST_hatbonus = 0;
    public float DERESIST_ringbonus = 0;
    public float DERESIST_amuletbonus = 0;
    public float DERESIST_buffbonus = 0;

    public float SPELLPOWER_weaponbonus = 0;
    public float SPELLPOWER_armorbonus = 0;
    public float SPELLPOWER_hatbonus = 0;
    public float SPELLPOWER_ringbonus = 0;
    public float SPELLPOWER_amuletbonus = 0;
    public float SPELLPOWER_buffbonus = 0;

    // Mathematical constant e
    float MATH_e = 2.71828f;

    /// <summary>
    /// Recalculates all player stats based on all bonuses the player is receiving through equipment / buffs and debuffs and the player's stats
    /// </summary>
    public override void RecalculateStats()
    {
        int statHealthBonus = BaseHealth;
        float stathpregenbonus = 0;
        float statstamregenbonus = 5;
        float statmaxstambonus = BaseStamina;
        float statmaxequipbonus = 50f;
        float soulshacklebonus = 50f;
        float tenacitybonus = 1f;
        float poisonresistbonus = 50f;
        float spellpowerbonus = 1f;
        // TERRESTRIAL BONUSES
        statHealthBonus += Mathf.CeilToInt(((2272.43f - 2317.87f * Mathf.Pow(MATH_e, -.0198026f * TerrestrialRES))) * (0.25f));
        statmaxequipbonus += (555.482f - 566.591f * Mathf.Pow(MATH_e, -.0198026f * TerrestrialRES)) * (0.25f);
        stathpregenbonus += (24.2492f - 25.754f * Mathf.Pow(MATH_e, -.0198026f * TerrestrialRES)) * (0.15f);

        // LUNAR BONUSES
        statHealthBonus += Mathf.CeilToInt(((2272.43f - 2317.87f * Mathf.Pow(MATH_e, -.0198026f * LunarRES))) * (1f/12f));
        statstamregenbonus += (35.3488f - 36.0558f * Mathf.Pow(MATH_e, -.0198026f * LunarRES)) * (0.5f) * 3f;
        statmaxstambonus += (555.482f - 566.591f * Mathf.Pow(MATH_e, -.0198026f * LunarRES)) * (0.5f);

        // SOLAR BONUSES
        statHealthBonus += Mathf.CeilToInt(((2272.43f - 2317.87f * Mathf.Pow(MATH_e, -.0198026f * SolarRES))) * (1f/ 12f));
        statmaxequipbonus += (555.482f - 566.591f * Mathf.Pow(MATH_e, -.0198026f * SolarRES)) * (0.5f);

        // INFERNAL BONUSES
        statmaxequipbonus += (555.482f - 566.591f * Mathf.Pow(MATH_e, -.0198026f * InfernalRES)) * (0.25f);
        statstamregenbonus += (35.3488f - 36.0558f * Mathf.Pow(MATH_e, -.0198026f * InfernalRES)) * (0.5f) * 3f;
        soulshacklebonus += (555.482f - 566.591f * Mathf.Pow(MATH_e, -.0198026f * InfernalRES)) * (0.5f);

        // CHILL BONUSES
        tenacitybonus -= (1.16146f - 1.18469f * Mathf.Pow(MATH_e, -.0198026f * ChillRES)) * (0.5f);
        statmaxstambonus += (555.482f - 566.591f * Mathf.Pow(MATH_e, -.0198026f * ChillRES)) * (0.25f);
        statHealthBonus += Mathf.CeilToInt(((2272.43f - 2317.87f * Mathf.Pow(MATH_e, -.0198026f * ChillRES))) * (1f / 12f));

        // LIFE BONUSES
        statHealthBonus += Mathf.CeilToInt(((2272.43f - 2317.87f * Mathf.Pow(MATH_e, -.0198026f * LifeRES))) * (1f / 12f));
        statmaxstambonus += (555.482f - 566.591f * Mathf.Pow(MATH_e, -.0198026f * LifeRES)) * (0.25f);
        poisonresistbonus += (555.482f - 566.591f * Mathf.Pow(MATH_e, -.0198026f * LifeRES)) * (0.5f);
        spellpowerbonus += (1.16146f - 1.18469f * Mathf.Pow(MATH_e, -.0198026f * LifeRES)) * (0.5f);

        // Death BONUSES
        poisonresistbonus += (555.482f - 566.591f * Mathf.Pow(MATH_e, -.0198026f * DeathRES)) * (0.5f);
        soulshacklebonus += (555.482f - 566.591f * Mathf.Pow(MATH_e, -.0198026f * DeathRES)) * (0.5f);
        tenacitybonus -= (1.16146f - 1.18469f * Mathf.Pow(MATH_e, -.0198026f * DeathRES)) * (0.5f);
        spellpowerbonus += (1.16146f - 1.18469f * Mathf.Pow(MATH_e, -.0198026f * DeathRES)) * (0.5f);

        MaximumHealth = statHealthBonus + HEALTH_weaponbonus + HEALTH_armorbonus + HEALTH_hatbonus + HEALTH_ringbonus + HEALTH_amuletbonus + HEALTH_buffbonus;

        MaximumStamina = statmaxstambonus + STAMINA_weaponbonus + STAMINA_armorbonus + STAMINA_hatbonus + STAMINA_ringbonus + STAMINA_amuletbonus + STAMINA_buffbonus;
        
        HealthRegenPerSecond = stathpregenbonus + HEALTHREGENweaponbonus + HEALTHREGENarmorbonus + HEALTHREGENhatbonus + HEALTHREGENringbonus + HEALTHREGENamuletbonus + HEALTHREGENbuffbonus;

        StaminaRegenPerSecond = statstamregenbonus + STAMINAREGEN_weaponbonus + STAMINAREGEN_armorbonus + STAMINAREGEN_hatbonus + STAMINAREGEN_ringbonus + STAMINAREGEN_amuletbonus + STAMINAREGEN_buffbonus;

        EquipLoad_Current = EQUIPLOAD_weaponbonus + EQUIPLOAD_armorbonus + EQUIPLOAD_hatbonus + EQUIPLOAD_ringbonus + EQUIPLOAD_amuletbonus + EQUIPLOAD_buffbonus;

        EquipLoad_Normal = statmaxequipbonus + MAXEQUIPLOAD_weaponbonus + MAXEQUIPLOAD_armorbonus + MAXEQUIPLOAD_hatbonus + MAXEQUIPLOAD_ringbonus + MAXEQUIPLOAD_amuletbonus + MAXEQUIPLOAD_buffbonus;

        // run speed is calculated by euip load
        float percent = EquipLoad_Current / EquipLoad_Normal;
        float rsmf = (Mathf.Pow(-1.1f * percent + 1f, 3f) + 3f / 2f) * 2f;
        RunSpeed = rsmf + RUNSPEED_weaponbonus + RUNSPEED_armorbonus + RUNSPEED_hatbonus + RUNSPEED_ringbonus + RUNSPEED_amuletbonus + RUNSPEED_buffbonus;
        RunSpeed = Mathf.Clamp(RunSpeed, 1f, 8f);
        if(percent >= 2 || percent < 0)
        {
            RunSpeed = 0.5f;
        }

        PoisonResistance = poisonresistbonus + POISONRESIST_weaponbonus + POISONRESIST_armorbonus + POISONRESIST_hatbonus + POISONRESIST_ringbonus + POISONRESIST_amuletbonus + POISONRESIST_buffbonus;

        SoulShackleResistance = soulshacklebonus + SSRESIST_weaponbonus + SSRESIST_armorbonus + SSRESIST_hatbonus + SSRESIST_ringbonus + SSRESIST_amuletbonus + SSRESIST_buffbonus;

        // tenacity 
        Tenacity = tenacitybonus + TENACITY_weaponbonus + TENACITY_armorbonus + TENACITY_hatbonus + TENACITY_ringbonus + TENACITY_amuletbonus + TENACITY_buffbonus;
        Tenacity = Mathf.Clamp(Tenacity, 0.2f, 2f);

        // swiftness is calculated by equip load
        float xesdf = (Mathf.Pow(1.1f * percent - 1f, 3f) + 1f);
        Swiftness = xesdf + SWIFNTESS_weaponbonus + SWIFNTESS_armorbonus + SWIFNTESS_hatbonus + SWIFNTESS_ringbonus + SWIFNTESS_amuletbonus + SWIFNTESS_buffbonus;
        Swiftness = Mathf.Clamp(Swiftness, 0.8f, 2f);
        if(percent >= 2)
        {
            Swiftness = 2f;
        }

        spellPower = spellpowerbonus + SPELLPOWER_weaponbonus + SPELLPOWER_armorbonus + SPELLPOWER_hatbonus + SPELLPOWER_ringbonus + SPELLPOWER_amuletbonus + SPELLPOWER_buffbonus;


        TerrestrialResistance = 0 + TRESIST_weaponbonus + TRESIST_armorbonus + TRESIST_hatbonus + TRESIST_ringbonus + TRESIST_amuletbonus + TRESIST_buffbonus;
        LunarResistance = 0 + LURESIST_weaponbonus + LURESIST_armorbonus + LURESIST_hatbonus + LURESIST_ringbonus + LURESIST_amuletbonus + LURESIST_buffbonus;
        SolarResistance = 0 + SORESIST_weaponbonus + SORESIST_armorbonus + SORESIST_hatbonus + SORESIST_ringbonus + SORESIST_amuletbonus + SORESIST_buffbonus;
        InfernalResistance = 0 + INRESIST_weaponbonus + INRESIST_armorbonus + INRESIST_hatbonus + INRESIST_ringbonus + INRESIST_amuletbonus + INRESIST_buffbonus;
        ChillResistance = 0 + CHRESIST_weaponbonus + CHRESIST_armorbonus + CHRESIST_hatbonus + CHRESIST_ringbonus + CHRESIST_amuletbonus + CHRESIST_buffbonus;
        DeathResistance = 0 + DERESIST_weaponbonus + DERESIST_armorbonus + DERESIST_hatbonus + DERESIST_ringbonus + DERESIST_amuletbonus + DERESIST_buffbonus;
        LifeResistance = 0 + LIRESIST_weaponbonus + LIRESIST_armorbonus + LIRESIST_hatbonus + LIRESIST_ringbonus + LIRESIST_amuletbonus + LIRESIST_buffbonus;


        boostedstamregen = StaminaRegenPerSecond * 1.3f;
        unboostedstamregen = StaminaRegenPerSecond;
    }

    public AudioClip[] takedmgS;
    public AudioClip[] takedmgextraS;
    public AudioClip blockS;
    public AudioClip parryS;

    void TakeHitSound()
    {
        int i = Random.Range(0, takedmgS.Length-1);
        float f = Random.Range(0.8f, 1f);
        AudioSource.PlayClipAtPoint(takedmgS[i], transform.position, f);
        if(i == (takedmgS.Length - 1))
        {
            AudioSource.PlayClipAtPoint(takedmgextraS[0], transform.position, 1);
        }
        if(i == 0)
        {
            AudioSource.PlayClipAtPoint(takedmgextraS[1], transform.position, 1);
        }
    }

    void BlockSound()
    {
        float f = Random.Range(0.5f, 1f);
       // AudioSource.PlayClipAtPoint(blockS, transform.position, f);
    }

    void ParrySound()
    {
       // AudioSource.PlayClipAtPoint(parryS, transform.position, 1f);

    }
    
    /// <summary>
    /// Take A Hit (damage) from any source. Relocates damage if the player is blocking. Calculates resistances and then subtracts from health/stamina
    /// </summary>
    /// <param name="DAMAGE_Te">Total Terrestial Damage Taken</param>
    /// <param name="DAMAGE_Lu">Total Lunar Damage</param>
    /// <param name="DAMAGE_So">Total Solar Damage</param>
    /// <param name="DAMAGE_In">Total Infernal Damage</param>
    /// <param name="DAMAGE_Ch">Total Chill Damage</param>
    /// <param name="DAMAGE_Li">Total Life Damage</param>
    /// <param name="DAMAGE_De">Total Death Damage</param>
    /// <param name="source">Location of the thing that hits. Used to determine if backhit</param>
    /// <param name="ovr">Override for blocking. Should be false.</param>
    public override void TakeAHit(int DAMAGE_Te, int DAMAGE_Lu, int DAMAGE_So, int DAMAGE_In, int DAMAGE_Ch, int DAMAGE_Li, int DAMAGE_De, Vector3 source, bool ovr = false)
    {
        if (immunity)
        {
            return;
        }
        if (pm.isBlocking && !ovr)
        {
            BlockAHit(DAMAGE_Te, DAMAGE_Lu, DAMAGE_So, DAMAGE_In, DAMAGE_Ch, DAMAGE_Li, DAMAGE_De, source);
            return;
        }
        bool behind = pm.GetHurtAnimation(PlayerMovement.TakeDamageAnimationType.NORMAL, source);
        int totaldamage = Mathf.CeilToInt(DAMAGE_Te * (1 - TerrestrialResistance) + DAMAGE_Lu * (1 - LunarResistance) + DAMAGE_So * (1 - SolarResistance) + DAMAGE_In * (1 - InfernalResistance) + DAMAGE_Ch * (1 - ChillResistance) + DAMAGE_Li * (1 - LifeResistance) + DAMAGE_De * (1 - DeathResistance));
        if (behind)
        {
            LoseHealth(totaldamage * 2);
        } else
        {
            LoseHealth(totaldamage);
        }
        TakeHitSound();
    }

    /// <summary>
    /// Block A Hit (damage) from any source. This function should not be called outside the TakeAHit() Function
    /// </summary>
    public void BlockAHit(int DAMAGE_Te, int DAMAGE_Lu, int DAMAGE_So, int DAMAGE_In, int DAMAGE_Ch, int DAMAGE_Li, int DAMAGE_De, Vector3 source)
    {
        bool behind = pm.GetHurtAnimation(PlayerMovement.TakeDamageAnimationType.BLOCKING, source);
        if (behind)
        {
            TakeAHit(DAMAGE_Te, DAMAGE_Lu, DAMAGE_So, DAMAGE_In, DAMAGE_Ch, DAMAGE_Li, DAMAGE_De, source, true);
        } else
        {
            WeaponScript ws = pm.sword.GetComponent<WeaponScript>();
            ws.RecalculateBlocking();
            BlockSound();
            // TO DO PUT IN BLOCK DAMPENING
            CurrentStamina -= DAMAGE_Te * (1 - ws.TOTALBLOCK_Te) + DAMAGE_Lu * (1 - ws.TOTALBLOCK_Lu) + DAMAGE_So * (1 - ws.TOTALBLOCK_So) + DAMAGE_In * (1 - ws.TOTALBLOCK_In) + DAMAGE_Ch * (1 - ws.TOTALBLOCK_Ch) + DAMAGE_Li * (1 - ws.TOTALBLOCK_Ch) + DAMAGE_De * (1 - ws.TOTALBLOCK_De);
            if(CurrentStamina < 0)
            {
                BlockBroken(source);
            }
        }
    }

    /// <summary>
    /// Function called when the player's block breaks. Calls the falling down animation
    /// </summary>
    /// <param name="source">Source of the hit</param>
    public void BlockBroken(Vector3 source)
    {
        bool behind = pm.GetHurtAnimation(PlayerMovement.TakeDamageAnimationType.FALLDOWN, source);
    }

    // Stamina Regen Boosts Due to Walking
    float boostedstamregen;
    float unboostedstamregen;
    /// <summary>
    /// Function called in Update that increases player's stamina regen if he is walking.
    /// </summary>
    public void WalkBonus()
    {
        if(pm.Walking < 0.5)
        {
            StaminaRegenPerSecond = boostedstamregen;
        } else
        {
            StaminaRegenPerSecond = unboostedstamregen;
        }
    }
    
    /// <summary>
    /// Function called upon player's death
    /// </summary>
    public override void Die()
    {
        Application.LoadLevel(0);
    }

    // Update is called once per frame
    void Update()
    {

        StaminaRegen();
        DashLock();
        WalkBonus();
        

    }
}