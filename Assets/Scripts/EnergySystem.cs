using UnityEngine;
using System;

public class EnergySystem : MonoBehaviour
{
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float regenerationRate = 5f; // Energy per second

    // Event triggered when energy changes, passes normalized energy (0 to 1)
    public Action<float> OnEnergyChanged;

    void Start()
    {
        currentEnergy = maxEnergy;
        OnEnergyChanged?.Invoke(currentEnergy / maxEnergy);
    }

    void Update()
    {
        // Energy regeneration is intentionally disabled to keep it strictly at 100 until used.
        /* 
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += regenerationRate * Time.deltaTime;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
            OnEnergyChanged?.Invoke(currentEnergy / maxEnergy);
        }
        */
    }

    public bool ConsumeEnergy(float amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            OnEnergyChanged?.Invoke(currentEnergy / maxEnergy);
            return true;
        }
        return false;
    }
}
