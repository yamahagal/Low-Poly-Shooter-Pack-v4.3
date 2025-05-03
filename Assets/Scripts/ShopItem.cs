using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static SaveSystem;

public class ShopItem : MonoBehaviour
{
    public string itemName;
    public GameObject buyButton;
    public GameObject lockIcon;
    public TMP_Text nameText;
    public TMP_Text costText;
    public Shop shop;

    [SerializeField] private int itemCost;

    void Start() {
        nameText.text = itemName;
        foreach (WeaponShopSettings weapon in shop.data.weapons) {
            if (weapon.weaponName == itemName) {
                Debug.Log(weapon.weaponName);
                itemCost = weapon.cost;
                if (weapon.hasPlayerWeapon) {
                    Debug.Log(weapon.hasPlayerWeapon);
                    if (buyButton != null)
                        buyButton.SetActive(false);
                }
            }

        }
        if (costText != null)
            costText.text = $"{itemCost}$";
    }
}

