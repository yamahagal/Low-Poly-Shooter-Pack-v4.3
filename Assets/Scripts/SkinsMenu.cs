using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinsMenu : MonoBehaviour
{
    public WeaponSkinsPanel[] skinPanels;
    void Start()
    {
        skinPanels = GetComponentsInChildren<WeaponSkinsPanel>(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenPanel(int weaponIndex) {
        for (int i = 0; i < skinPanels.Length; i++) {
            skinPanels[i].gameObject.SetActive(false);
        }
        skinPanels[weaponIndex].gameObject.SetActive(true);

    }
}
