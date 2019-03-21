using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public GameObject heartPrefab;
    public float heartDis = 55f;
    public List<GameObject> healthHearts = new List<GameObject>();

    public Image energySlider;

    private void Awake()
    {
        Byte.instance.HealthSetEvent += UpdateHealth;
        Byte.instance.EnergySetEvent += UpdateEnergy;
    }

    public void UpdateHealth(float currentHealth)
    {
        if(healthHearts.Count > currentHealth)
        {
            Debug.Log("Destroy Hearts");
            for (int i = (int)currentHealth; i < healthHearts.Count; i++)
            {
                Destroy(healthHearts[i]);
                healthHearts.RemoveAt(i);
            }
        }
        else
        {
            Debug.Log("Spawn Heart");
            for (int i = healthHearts.Count; i < currentHealth; i++)
            {
                GameObject spawnedHeart = Instantiate(heartPrefab, new Vector3(this.transform.position.x + (heartDis * i), transform.position.y, transform.position.x), Quaternion.identity);
                spawnedHeart.transform.SetParent(transform);
                healthHearts.Add(spawnedHeart);
            }
        }
        Debug.Log("Update Health");
    }

    public void UpdateEnergy(float current, float max)
    {
        float set = current / max;
        energySlider.fillAmount = set;
    }

}
