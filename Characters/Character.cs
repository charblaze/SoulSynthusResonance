using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

    // BASE CLASS FOR ANYTHING THAT CAN BE KILLED BASICALLY (enemies, player, npcs)

    public Transform buffloc;

    // basic RPG Stats
    public int BaseHealth = 250;
    public float BaseStamina;
    public float MaximumStamina;
    public float CurrentStamina;
    public int MaximumHealth;
    public int CurrentHealth;
    public float StaminaRegenPerSecond = 10f;
    public bool isRecoveringStamina = true;
    public float HealthRegenPerSecond = 0f;
    int TR, LuR, SR, IR, CR, LiR, DR;

    // BONUS that is gained when this character parries successfully
    public bool ParryBonus = false;

    // Represents the speed at which the player runs. Multiplied by 1.5 to get dash speed. Affected by equip load. Also affects dodge distnace
    public float RunSpeed;

    // Resistances to the elemnts, affected directly by stats. FLOATS FROM 0 TO 1, USUALLY CLOSE TO 1
    public float TerrestrialResistance =1f, LunarResistance = 1f, SolarResistance = 1f, InfernalResistance =1f, ChillResistance =1f, LifeResistance =1f, DeathResistance =1f;

    // Represents a gauge of poison resistance that when fills up, poisons the character for a few minutes. Length of the gauge.
    public float PoisonResistance;

    // Gauge that when fills up, frails the character and makes his health stuck at 1.
    public float SoulShackleResistance;

    //// whether this character can get hit or not
    public bool immunity = false;
    

    // most important base stats of all characters. Determines health bonuses and such
    public int TerrestrialRES
    {
        set
        {
            TR = value;
            RecalculateStats();
        }
        get
        {
            return TR;
        }
    }

    public int LunarRES
    {
        set
        {
            LuR = value;
            RecalculateStats();
        }
        get
        {
            return LuR;
        }
    }

    public int SolarRES
    {
        set
        {
            SR = value;
            RecalculateStats();
        }
        get
        {
            return SR;
        }
    }

    public int InfernalRES
    {
        set
        {
            IR = value;
            RecalculateStats();
        }
        get
        {
            return IR;
        }
    }

    public int ChillRES
    {
        set
        {
            CR = value;
            RecalculateStats();
        }
        get
        {
            return CR;
        }
    }

    public int LifeRES
    {
        set
        {
            LiR = value;
            RecalculateStats();
        }
        get
        {
            return LiR;
        }
    }

    public int DeathRES
    {
        set
        {
            DR = value;
            RecalculateStats();
        }
        get
        {
            return DR;
        }
    }

    /// <summary>
    /// Virtual RecalculateStats Function that should be overwritten
    /// </summary>
    public virtual void RecalculateStats()
    {
        MaximumStamina = BaseStamina;
        MaximumHealth = BaseHealth;
    }

    /// <summary>
    /// Function called every Update that regens the player's stamina in accordance to the player's stamina regen
    /// </summary>
    public void StaminaRegen()
    {
        if (isRecoveringStamina)
        {
            CurrentStamina += StaminaRegenPerSecond * Time.deltaTime;
        }

        if (CurrentStamina >= MaximumStamina)
        {
            CurrentStamina = MaximumStamina;
        }
    }

    Coroutine stmrgncrtn;

    /// <summary>
    /// Coroutine that should bec alled from SuspendStaminaRegenForSecs
    /// </summary>
    IEnumerator SuspendStaminaRegen(float f)
    {
        isRecoveringStamina = false;
        yield return new WaitForSeconds(f);
        isRecoveringStamina = true;
    }

    /// <summary>
    /// Stop Stamina Regen for given seconds
    /// </summary>
    /// <param name="f">Time in seconds</param>
    public void SuspendStaminaRegenForSecs(float f)
    {
        if(stmrgncrtn != null)
        {
            StopCoroutine(stmrgncrtn);
        }
        stmrgncrtn = StartCoroutine(SuspendStaminaRegen(f));
    }

    /// <summary>
    /// Lose an amount of stamina specified, and suspends stamina regen for a certain amount of time.
    /// </summary>
    /// <param name="amount">Amount of Stamina Lost</param>
    /// <param name="punish">Time without regen after</param>
    public void LoseStamina(float amount, float punish = 0.6f)
    {
        SuspendStaminaRegenForSecs(punish);
        CurrentStamina -= amount;
    }

    public bool isAbleToAttack
    {
        get
        {
            if(CurrentStamina <= 0)
            {
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Function that should be called from update that makes the player lose a gradual amount of specified stamina
    /// </summary>
    /// <param name="amountpersec">amount to lose per second</param>
    /// <param name="none">Set to true if you desire the palyer to simply not recover stamina while this function is called</param>
    public void LoseGradualStamina(float amountpersec, bool none = false)
    {
        if (none)
        {
            if (isRecoveringStamina)
            {
                CurrentStamina -= StaminaRegenPerSecond * Time.deltaTime;
            }
        }
        else
        {
            CurrentStamina -= amountpersec * Time.deltaTime;
        }
    }

    IEnumerator dashslam()
    {
        isAbleToDashAgain = false;
        yield return new WaitForSeconds(1.3f);
        isAbleToDashAgain = true;
    }

    public bool isAbleToDashAgain = true;
    Coroutine dashal;
    /// <summary>
    /// Function called when stamina reaches below zero. Prevents character for dashing for a few seconds.
    /// </summary>
    public void DashLock()
    {
        if(CurrentStamina <= 0)
        {
            if (dashal != null)
            {
                StopCoroutine(dashal);
            }
            dashal = StartCoroutine(dashslam());
        }
    }

    /// <summary>
    /// Returns and RGB color this characer, which is based off the character's stats.
    /// </summary>
    public Color CharacterColor
    {
        get
        {
            int maxstat = Mathf.Max(TR, LuR, SR, IR, CR, LiR, DR);
            if(maxstat == TR)
            {
                return new Color(92f/255f, 181f/255f, 20f/255f);
            } else if(maxstat == LuR)
            {
                return new Color(205f / 255f, 130f / 255f, 255f / 255f);
            } else if(maxstat== SR)
            {
                return new Color(255f / 255f, 204f / 255f, 0);
            } else if(maxstat == IR)
            {
                return new Color(235f / 255f, 0, 0);
            } else if(maxstat == CR)
            {
                return new Color(24f / 255f, 50f / 255f, 199f / 255f);
            } else if(maxstat == LiR)
            {
                return Color.white;
            }

            return Color.black;
        }
    }

    Coroutine imm;
    IEnumerator immunityFrames(float t)
    {
        immunity = true;
        yield return new WaitForSeconds(t);
        immunity = false;
    }

    /// <summary>
    /// Grants the character immunity
    /// </summary>
    /// <param name="secs">Duration of immunity</param>
    public void GrantImmunity(float secs)
    {
        if (imm != null)
        {
            StopCoroutine(imm);
        }
        imm = StartCoroutine(immunityFrames(secs));
    }

    /// <summary>
    /// Lose a raw amount of health. For scaled damage use TakeAHit()
    /// </summary>
    /// <param name="amount">Amount of health to lose</param>
    public void LoseHealth(int amount)
    {
        CurrentHealth -= amount;
        if(CurrentHealth < 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Gain a raw amount of health.
    /// </summary>
    /// <param name="amount"></param>
    public void GainHealth(int amount)
    {
        CurrentHealth += amount;
        if(CurrentHealth > MaximumHealth)
        {
            CurrentHealth = MaximumHealth;
        }
    }

    /// <summary>
    /// A function that returns true if the specified source hit the character from behind
    /// </summary>
    /// <param name="source">Source of Hits</param>
    /// <returns>If the hit came from behind</returns>
    public bool CheckForBackHits(Vector3 source)
    {
        Vector3 s = (source - transform.position).normalized * 2;
        float xs = Vector3.Dot(s, transform.forward) * 2f;
        if (xs < 0.1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Death function for this character. Usually overwritten
    /// </summary>
    public virtual void Die()
    {
        //Destroy(gameObject);
    }

    /// <summary>
    /// BASIC FUNCTION FOR TAKING DAMAGE. USUALLY OVERWRITTEN
    /// </summary>
    /// <param name="DAMAGE_Te"></param>
    /// <param name="DAMAGE_Lu"></param>
    /// <param name="DAMAGE_So"></param>
    /// <param name="DAMAGE_In"></param>
    /// <param name="DAMAGE_Ch"></param>
    /// <param name="DAMAGE_Li"></param>
    /// <param name="DAMAGE_De"></param>
    /// <param name="source"></param>
    /// <param name="ovr"></param>
    public virtual void TakeAHit(int DAMAGE_Te, int DAMAGE_Lu, int DAMAGE_So, int DAMAGE_In, int DAMAGE_Ch, int DAMAGE_Li, int DAMAGE_De , Vector3 source, bool ovr = false)
    {
        bool behind = CheckForBackHits(source);
        int totaldamage = Mathf.CeilToInt(DAMAGE_Te * TerrestrialResistance + DAMAGE_Lu * LunarResistance + DAMAGE_So * SolarResistance + DAMAGE_In * InfernalResistance + DAMAGE_Ch * ChillResistance + DAMAGE_Li * LifeResistance + DAMAGE_De * DeathResistance);
        if (behind)
        {
            LoseHealth(Mathf.CeilToInt(totaldamage * 1.5f));
        }
        else
        {
            LoseHealth(totaldamage);
        }

    }

	// Use this for initialization
	void Start () {
	}

	
	// Update is called once per frame
	void Update () {
    }
}
