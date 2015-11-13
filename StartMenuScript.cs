using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class StartMenuScript : MonoBehaviour {
    public Text texthp, texthpregen, textstam, textstamregen, textrunspd, textequip, textswiftness, texttenacity, textspellpower, textTER, textLU, textSO, textIN, textCH, textLI, textDE;
    public Text textTERRESIST, textLURESIST, textSORESIST, textINRESIST, textCHRESIST, textLIRESIST, textDERESIST, textpois, textsoulshackle;
    public Text textWEAPON_NAME, textwepdmgTE, textwepdmgLU, textwepdmgSO, textwepdmgIN, textwepdmgCH, textwepdmgLI, textwepdmgDE, textwepdmgTOT;
    public Text[] textwepBLOCK;

    public Text wepSPECIAL, wepWEIGHT;
    public Text[] wepSCALING, wepBASE;
    public Image wepPIC;

    public RectTransform contentWEP;
    public RectTransform contentEQU;
    public Text EquipTextType;
    // Use this for initialization
    Player p;
    public PlayerMovement pm;

    public PlayerInventory pi;
   
    WeaponScript wep;

	void Start () {
        p = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        wep = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().sword.GetComponent<WeaponScript>();
	}

    public void StartUpMenu()
    {
        StartCoroutine(PopulateWeaponList());
        StartCoroutine(SaveInitialStats());
        StartCoroutine(ColorStatChanges());
        EquipPaneChanged("Hats");
    }

    string slamstring(int x)
    {
        if (x == 0)
        {
            return "Te: ";
        }
        if (x == 1)
        {
            return "Lu: ";
        }
        if (x == 2)
        {
            return "So: ";
        }
        if (x == 3)
        {
            return "In: ";
        }
        if(x == 4)
        {
            return "Ch: ";
        }
        if(x == 5)
        {
            return "Lu: ";
        }
        if(x == 6)
        {
            return "De: ";
        }
        return "De: ";
    }

    /*
        ---- WEAPONS ----
        */
    public string GetReadableScaling(int i, WeaponScript ws)
    {
        string ans = "";
        if (i == 0)
        {
            char[] car = ws.SCALING_Te;
            for (int c = 0; c < car.Length; ++c)
            {
                if (car[c] != 'f' && car[c] != '\0')
                {
                    ans = ans + slamstring(c) + char.ToUpper(car[c]).ToString() + ",";
                }
            }
        } else if (i == 1)
        {
            char[] car = ws.SCALING_Lu;
            for (int c = 0; c < car.Length; ++c)
            {
                if (car[c] != 'f' && car[c] != '\0')
                {
                    ans = ans + slamstring(c) + char.ToUpper(car[c]).ToString() + ",";
                }
            }
        }
        else if (i == 2)
        {
            char[] car = ws.SCALING_So;
            for (int c = 0; c < car.Length; ++c)
            {
                if (car[c] != 'f' && car[c] != '\0')
                {
                    ans = ans + slamstring(c) + char.ToUpper(car[c]).ToString() + ",";
                }
            }
        }
        else if (i == 3)
        {
            char[] car = ws.SCALING_In;
            for (int c = 0; c < car.Length; ++c)
            {
                if (car[c] != 'f' && car[c] != '\0')
                {
                    ans = ans + slamstring(c) + char.ToUpper(car[c]).ToString() + ",";
                }
            }
        }
        else if (i == 4)
        {
            char[] car = ws.SCALING_Ch;
            for (int c = 0; c < car.Length; ++c)
            {
                if (car[c] != 'f' && car[c] != '\0')
                {
                    ans = ans + slamstring(c) + char.ToUpper(car[c]).ToString() + ",";
                }
            }
        }
        else if (i == 5)
        {
            char[] car = ws.SCALING_Li;
            for (int c = 0; c < car.Length; ++c)
            {
                if (car[c] != 'f' && car[c] != '\0')
                {
                    ans = ans + slamstring(c) + char.ToUpper(car[c]).ToString() + ",";
                }
            }
        }
        else if (i == 6)
        {
            char[] car = ws.SCALING_De;
            for (int c = 0; c < car.Length; ++c)
            {
                if (car[c] != 'f' && car[c] != '\0')
                {
                    ans = ans + slamstring(c) + char.ToUpper(car[c]).ToString() + ",";
                }
            }
        }
        if (ans == "")
        {
            ans = "--,";
        }
        return ans.Substring(0, ans.Length - 1);
    }

    public void EquipWeapon(int i)
    {
        StartCoroutine(EquipWeaponCR(i));
    }

    // Equips Weapon
    public IEnumerator EquipWeaponCR(int i)
    {
        if (pm.sword != null)
        {
            Destroy(pm.sword);
        }
        p.EquippedWeapon = weps[i].WeaponID;
        pm.WeaponChanged();
        pm.instantiateSheathedSword();
        WeaponScript newwep = pm.sword.GetComponent<WeaponScript>();
        newwep.LEVEL = weps[i].UpgradeLevel;
        newwep.CheckForUpgraded();
        yield return new WaitForFixedUpdate();
        newwep.RecalculateBlocking();
        newwep.RecalculateDamage();
        /*
        wepTITLE.text = weps[i].WeaponID;
        wepDESCRIP.text = weps[i].Description;
        GameObject tempwep = pm.sword;
        WeaponScript ws = tempwep.GetComponent<WeaponScript>();
        wepBASE[0].text = ws.BASE_Te.ToString();
        wepBASE[1].text = ws.BASE_Lu.ToString();
        wepBASE[2].text = ws.BASE_So.ToString();
        wepBASE[3].text = ws.BASE_In.ToString();
        wepBASE[4].text = ws.BASE_Ch.ToString();
        wepBASE[5].text = ws.BASE_Li.ToString();
        wepBASE[6].text = ws.BASE_De.ToString();

        wepSCALING[0].text = GetReadableScaling(0, ws);
        wepSCALING[1].text = GetReadableScaling(1, ws);
        wepSCALING[2].text = GetReadableScaling(2, ws);
        wepSCALING[3].text = GetReadableScaling(3, ws);
        wepSCALING[4].text = GetReadableScaling(4, ws);
        wepSCALING[5].text = GetReadableScaling(5, ws);
        wepSCALING[6].text = GetReadableScaling(6, ws);
        
        ws.RecalculateDamage();
        ws.RecalculateBlocking();
        wepTOTALDMG[0].text = ws.TOTAL_Te.ToString();*/
        StartCoroutine(ColorStatChanges());
    }

    List<PlayerInventory.Weapon> weps = new List<PlayerInventory.Weapon>();

    public IEnumerator PopulateWeaponList()
    {
        yield return new WaitForFixedUpdate();
        // get lsit of weapons
        weps = pi.Weapons;
        // delete all kids
        foreach(Transform child in contentWEP.transform)
        {
            Destroy(child.gameObject);
        }
        // WepButton
        for(int c = 0; c < weps.Count; ++c)
        {
            GameObject butt = Instantiate(Resources.Load("UI/WepButton")) as GameObject;
            butt.GetComponent<Hov>().ID = pi.Weapons[c].WeaponID;
            butt.transform.SetParent(contentWEP);
            Text t = butt.transform.GetChild(0).GetComponent<Text>();
            t.text = weps[c].WeaponID;
            Button b = butt.GetComponent<Button>();
            AddListener(b, c);
        }
    }

    void AddListener(Button b, int value)
    {
        b.onClick.AddListener(() => EquipWeapon(value));
    }
    

    /*
        -- END OF WEAPONS -- 
    */
    /*
        EQUIPMENT
    */

    string EquipmentPaneOpen = "Hats";

    public void EquipPaneChanged(string to)
    {
        EquipmentPaneOpen = to;
        EquipTextType.text = EquipmentPaneOpen;
        StartCoroutine(PopulateEquipment());
    }
    
    public void EquipHat(int c)
    {
        if(pi.Hats[c].ID == p.EquippedHat)
        {
            return;
        }
        p.EquippedHat = pi.Hats[c].ID;
        p.LoadHat();
        EquipPaneChanged(EquipmentPaneOpen);
    }

    public void EquipAmulet(int c)
    {
        if(pi.Amulets[c].ID == p.EquippedAmulet)
        {
            return;
        }
        p.EquippedAmulet = pi.Amulets[c].ID;
        p.LoadAmulet(); EquipPaneChanged(EquipmentPaneOpen);
    }

    public void EquipTunic(int c)
    {
        if(pi.Tunics[c].ID == p.EquippedTunic)
        {
            return;
        }
        p.EquippedTunic = pi.Tunics[c].ID;
        p.LoadTunic(); EquipPaneChanged(EquipmentPaneOpen);
    }

    public void EquipRing(int c, int slot)
    {
        if((pi.Rings[c].ID == p.EquippedRing1 || pi.Rings[c].ID == p.EquippedRing2) && pi.Rings[c].ID != "None")
        {
            return;
        }
        if(slot == 1)
        { 
            p.EquippedRing1 = pi.Rings[c].ID;
            p.LoadRing1();
        }  else
        {
            p.EquippedRing2 = pi.Rings[c].ID;
            p.LoadRing2();
        }
        EquipPaneChanged(EquipmentPaneOpen);
    }

    public void EquipArmor(int c)
    {
        if(pi.Armors[c].ID == p.EquippedArmor)
        {
            return;
        }
        p.EquippedArmor = pi.Armors[c].ID;
        p.LoadArmor(); EquipPaneChanged(EquipmentPaneOpen);
    }

    public void EquipCape(int c)
    {
        if (pi.Capes[c].ID == p.EquippedCape)
        {
            return;
        }
        p.EquippedCape = pi.Capes[c].ID;
        p.LoadCape(); EquipPaneChanged(EquipmentPaneOpen);
    }

    void AddListenerHat(Button b, int c)
    {
        b.onClick.AddListener(() => EquipHat(c));
    }
    void AddListenerAmulet(Button b, int c)
    {
        b.onClick.AddListener(() => EquipAmulet(c));
    }
    void AddListenerTunic(Button b, int c)
    {
        b.onClick.AddListener(() => EquipTunic(c));
    }

    void AddListenerRing(Button b, int c, int d)
    {
        b.onClick.AddListener(() => EquipRing(c, d));
    }

    void AddListenerArmor(Button b, int c)
    {
        b.onClick.AddListener(() => EquipArmor(c));
    }

    void AddListenerCape(Button b, int c)
    {
        b.onClick.AddListener(() => EquipCape(c));
    }

    public IEnumerator PopulateEquipment()
    {
        yield return new WaitForFixedUpdate();
        // delete all kids
        foreach (Transform child in contentEQU.transform)
        {
            Destroy(child.gameObject);
        }
        bool eqd = false;
        // equipbutton
        if (EquipmentPaneOpen == "Hats")
        {
            for (int c = 0; c < pi.Hats.Count; ++c)
            {
                GameObject butt = Instantiate(Resources.Load("UI/EquipButton")) as GameObject;
                butt.GetComponent<Hov>().ID = pi.Hats[c].ID;
                butt.transform.SetParent(contentEQU);
                Text t = butt.transform.GetChild(0).GetComponent<Text>();
                t.text = pi.Hats[c].ID;
                Button b = butt.GetComponent<Button>();
                
                AddListenerHat(b, c);
                if(pi.Hats[c].ID == p.EquippedHat && !eqd)
                {
                    ColorBlock d = b.colors;
                    d.normalColor = Color.yellow;
                    b.colors = d;
                    eqd = true;
                }
            }
        } else if(EquipmentPaneOpen == "Amulets")
        {
            for (int c = 0; c < pi.Amulets.Count; ++c)
            {
                GameObject butt = Instantiate(Resources.Load("UI/EquipButton")) as GameObject;
                butt.GetComponent<Hov>().ID = pi.Amulets[c].ID;
                butt.transform.SetParent(contentEQU);
                Text t = butt.transform.GetChild(0).GetComponent<Text>();
                t.text = pi.Amulets[c].ID;
                Button b = butt.GetComponent<Button>();
                AddListenerAmulet(b, c);
                if (pi.Amulets[c].ID == p.EquippedAmulet && !eqd)
                {
                    ColorBlock d = b.colors;
                    d.normalColor = Color.yellow;
                    b.colors = d; eqd = true;
                }
            }
        }
        else if (EquipmentPaneOpen == "Tunics")
        {
            for (int c = 0; c < pi.Tunics.Count; ++c)
            {
                GameObject butt = Instantiate(Resources.Load("UI/EquipButton")) as GameObject;
                butt.transform.SetParent(contentEQU);
                Text t = butt.transform.GetChild(0).GetComponent<Text>();
                t.text = pi.Tunics[c].ID;
                Button b = butt.GetComponent<Button>();
                AddListenerTunic(b, c);
            }
        }
        else if (EquipmentPaneOpen == "Rings1")
        {
            bool alreadyequipped = false;
            for (int c = 0; c < pi.Rings.Count; ++c)
            {
                if (pi.Rings[c].ID == p.EquippedRing2)
                {
                    if (!alreadyequipped)
                    {
                        alreadyequipped = true;
                        continue;
                    }
                }
                GameObject butt = Instantiate(Resources.Load("UI/EquipButton")) as GameObject;
                butt.GetComponent<Hov>().ID = pi.Rings[c].ID;
                butt.transform.SetParent(contentEQU);
                Text t = butt.transform.GetChild(0).GetComponent<Text>();
                t.text = pi.Rings[c].ID;
                Button b = butt.GetComponent<Button>();
                AddListenerRing(b, c, 1);
                if (pi.Rings[c].ID == p.EquippedRing1 && !eqd)
                {
                    ColorBlock d = b.colors;
                    d.normalColor = Color.yellow;
                    b.colors = d; eqd = true;
                }
            }
        }
        else if (EquipmentPaneOpen == "Rings2")
        {
            bool alreadyequipped = false;
            for (int c = 0; c < pi.Rings.Count; ++c)
            {
                if(pi.Rings[c].ID == p.EquippedRing1)
                {
                    if (!alreadyequipped)
                    {
                        alreadyequipped = true;
                        continue;
                    }
                }
                GameObject butt = Instantiate(Resources.Load("UI/EquipButton")) as GameObject;
                butt.GetComponent<Hov>().ID = pi.Rings[c].ID;
                butt.transform.SetParent(contentEQU);
                Text t = butt.transform.GetChild(0).GetComponent<Text>();
                t.text = pi.Rings[c].ID;
                Button b = butt.GetComponent<Button>();
                AddListenerRing(b, c, 2);
                if (pi.Rings[c].ID == p.EquippedRing2 && !eqd)
                {
                    ColorBlock d = b.colors;
                    d.normalColor = Color.yellow;
                    b.colors = d; eqd = true;
                }
            }
        } else if (EquipmentPaneOpen == "Armor")
        {
            for (int c = 0; c < pi.Armors.Count; ++c)
            {
                GameObject butt = Instantiate(Resources.Load("UI/EquipButton")) as GameObject;
                butt.GetComponent<Hov>().ID = pi.Armors[c].ID;
                butt.transform.SetParent(contentEQU);
                Text t = butt.transform.GetChild(0).GetComponent<Text>();
                t.text = pi.Armors[c].ID;
                Button b = butt.GetComponent<Button>();
                AddListenerArmor(b, c);
                if (pi.Armors[c].ID == p.EquippedArmor && !eqd)
                {
                    ColorBlock d = b.colors;
                    d.normalColor = Color.yellow;
                    b.colors = d; eqd = true;
                }
            }
        }
        else if (EquipmentPaneOpen == "Cape")
        {
            for (int c = 0; c < pi.Capes.Count; ++c)
            {
                GameObject butt = Instantiate(Resources.Load("UI/EquipButton")) as GameObject;
                butt.GetComponent<Hov>().ID = pi.Capes[c].ID;
                butt.transform.SetParent(contentEQU);
                Text t = butt.transform.GetChild(0).GetComponent<Text>();
                t.text = pi.Capes[c].ID;
                Button b = butt.GetComponent<Button>();
                AddListenerCape(b, c);
                if (pi.Capes[c].ID == p.EquippedCape && !eqd)
                {
                    ColorBlock d = b.colors;
                    d.normalColor = Color.yellow;
                    b.colors = d; eqd = true;
                }
            }
        }
    }


    // init stats... weight, total damage, all blocks
    List<Text> initStats = new List<Text>();
    List<int> initStatsNumbers = new List<int>();
    public IEnumerator SaveInitialStats()
    {
        yield return new WaitForFixedUpdate();
        initStats.Clear();
        initStatsNumbers.Clear();
        if (wep == null)
        {
            wep = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().sword.GetComponent<WeaponScript>();
        }
        wep.RecalculateDamage();
        wep.RecalculateDamage();
        initStats.Add(wepWEIGHT);
        initStatsNumbers.Add(Mathf.CeilToInt(wep.Weight));
        initStats.Add(textwepdmgTOT);
        initStatsNumbers.Add((wep.TOTAL_Te + wep.TOTAL_Ch + wep.TOTAL_De + wep.TOTAL_In + wep.TOTAL_Li + wep.TOTAL_Lu + wep.TOTAL_So));
    }

    public IEnumerator ColorStatChanges()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        // weight
        if (Mathf.CeilToInt(wep.Weight) > initStatsNumbers[0])
        {
            initStats[0].color = Color.red;
        } else if (Mathf.CeilToInt(wep.Weight) < initStatsNumbers[0]) {
            initStats[0].color = Color.green;
        } else
        {
            initStats[0].color = Color.black;
        }
        // total dmg
        if((wep.TOTAL_Te + wep.TOTAL_Ch + wep.TOTAL_De + wep.TOTAL_In + wep.TOTAL_Li + wep.TOTAL_Lu + wep.TOTAL_So) < initStatsNumbers[1])
        {
            initStats[1].color = Color.red;
        }
        else if ((wep.TOTAL_Te + wep.TOTAL_Ch + wep.TOTAL_De + wep.TOTAL_In + wep.TOTAL_Li + wep.TOTAL_Lu + wep.TOTAL_So) > initStatsNumbers[1])
        {
            initStats[1].color = Color.green;
        }
        else
        {
            initStats[1].color = Color.black;
        }
    }

    public void UpdateUIStats()
    {
        texthp.text = p.CurrentHealth + @"/" + p.MaximumHealth;
        texthpregen.text = p.HealthRegenPerSecond.ToString("F3");
        textstam.text = p.CurrentStamina.ToString("F1") + @"/" + p.MaximumStamina.ToString("F1");
        textstamregen.text = p.StaminaRegenPerSecond.ToString("F3");
        textrunspd.text = Mathf.Round(p.RunSpeed / 3f * 100f).ToString() + "%";
        textequip.text = p.EquipLoad_Current.ToString("F1") + @"/" + p.EquipLoad_Normal.ToString("F1") + " (" + Mathf.Round(p.EquipLoad_Current / p.EquipLoad_Normal * 100f) + "%)";
        textswiftness.text = Mathf.Round((1f - p.Swiftness) * 100f) + "%";
        texttenacity.text = Mathf.Round((1f - p.Tenacity) * 100f) + "%";
        float sp = (p.spellPower - 1f) * 100f;
        textspellpower.text = (sp > 0) ? "+" + Mathf.Round(sp) + "%" : Mathf.Round(sp) + "%";

        textTER.text = p.TerrestrialRES.ToString();
        textSO.text = p.SolarRES.ToString();
        textLU.text = p.LunarRES.ToString();
        textIN.text = p.InfernalRES.ToString();
        textCH.text = p.ChillRES.ToString();
        textLI.text = p.LifeRES.ToString();
        textDE.text = p.DeathRES.ToString();

        textTERRESIST.text = (p.TerrestrialResistance * 100f).ToString("F1") + "%";
        textLURESIST.text = (p.LunarResistance * 100f).ToString("F1") + "%";
        textSORESIST.text = (p.SolarResistance * 100f).ToString("F1") + "%";
        textINRESIST.text = (p.InfernalResistance * 100f).ToString("F1") + "%";
        textCHRESIST.text = (p.ChillResistance * 100f).ToString("F1") + "%";
        textLIRESIST.text = (p.LifeResistance * 100f).ToString("F1") + "%";
        textDERESIST.text = (p.DeathResistance * 100f).ToString("F1") + "%";

        textpois.text = p.PoisonResistance.ToString("F1");
        textsoulshackle.text = p.SoulShackleResistance.ToString("F1");
        if (wep != null)
        {
            wep.RecalculateBlocking();
            wep.RecalculateDamage();
            textWEAPON_NAME.text = wep.WeaponName + " +" + wep.LEVEL;
            textwepdmgTE.text = wep.TOTAL_Te.ToString();
            textwepdmgLU.text = wep.TOTAL_Lu.ToString();
            textwepdmgSO.text = wep.TOTAL_So.ToString();
            textwepdmgIN.text = wep.TOTAL_In.ToString();
            textwepdmgCH.text = wep.TOTAL_Ch.ToString();
            textwepdmgLI.text = wep.TOTAL_Li.ToString();
            textwepdmgDE.text = wep.TOTAL_De.ToString();
            textwepdmgTOT.text = "Total Damage: " + (wep.TOTAL_Te + wep.TOTAL_Ch + wep.TOTAL_De + wep.TOTAL_In + wep.TOTAL_Li + wep.TOTAL_Lu + wep.TOTAL_So). ToString();

            textwepBLOCK[0].text = (wep.TOTALBLOCK_Te * 100f).ToString("F0") + "%";
            textwepBLOCK[1].text = (wep.TOTALBLOCK_Lu * 100f).ToString("F0") + "%";
            textwepBLOCK[2].text = (wep.TOTALBLOCK_So * 100f).ToString("F0") + "%";
            textwepBLOCK[3].text = (wep.TOTALBLOCK_In * 100f).ToString("F0") + "%";
            textwepBLOCK[4].text = (wep.TOTALBLOCK_Ch * 100f).ToString("F0") + "%";
            textwepBLOCK[5].text = (wep.TOTALBLOCK_Li * 100f).ToString("F0") + "%";
            textwepBLOCK[6].text = (wep.TOTALBLOCK_De * 100f).ToString("F0") + "%";


            wepSCALING[0].text = GetReadableScaling(0, wep);
            wepSCALING[1].text = GetReadableScaling(1, wep);
            wepSCALING[2].text = GetReadableScaling(2, wep);
            wepSCALING[3].text = GetReadableScaling(3, wep);
            wepSCALING[4].text = GetReadableScaling(4, wep);
            wepSCALING[5].text = GetReadableScaling(5, wep);
            wepSCALING[6].text = GetReadableScaling(6, wep);

            wepBASE[0].text = wep.BASE_Te.ToString();
            wepBASE[1].text = wep.BASE_Lu.ToString();
            wepBASE[2].text = wep.BASE_So.ToString();
            wepBASE[3].text = wep.BASE_In.ToString();
            wepBASE[4].text = wep.BASE_Ch.ToString();
            wepBASE[5].text = wep.BASE_Li.ToString();
            wepBASE[6].text = wep.BASE_De.ToString();

            wepWEIGHT.text = "Weight: " + wep.Weight.ToString();
            wepSPECIAL.text = "Special: " + wep.WeaponSpecialDescrip;
        } else
        {
            wep = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().sword.GetComponent<WeaponScript>();
        }
        if(pi == null)
        {
            pi = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        UpdateUIStats();
	}
}
