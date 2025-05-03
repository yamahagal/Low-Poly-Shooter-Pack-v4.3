using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponParametersPanel : MonoBehaviour
{
    public List<GameObject> WeaponPrefabs;
    public List<TMP_Text> WeaponParameters;
    public Transform spawnPoint;
    public TMP_Text WeaponName;
    public TMP_Text WeaponDescription;
    public GameObject WeaponPrefab;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenParameters(int weaponIndex) {
        gameObject.SetActive(true);
        WeaponPrefab = Instantiate(WeaponPrefabs[weaponIndex], spawnPoint);

    }

    public void CloseParameters() {
        Destroy(WeaponPrefab);
        gameObject.SetActive(false);
    }
}
