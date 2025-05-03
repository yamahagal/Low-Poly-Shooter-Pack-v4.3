using System.Collections;
using System.Collections.Generic;
using InfimaGames.LowPolyShooterPack;
using UnityEngine;
using static SaveSystem;

public class Shop : SaveSystem
{
    public PlayerData data;

    private void Awake() {
        savePath = "C:\\Unity\\Low Poly Shooter Pack v4.3\\Saves";
        data = Load(savePath);
    }

    void Start() {
        
    }

    void Update()
    {
        
    }

    public void Buy(ShopItem item) {
        Debug.Log("Buy");
        foreach (WeaponShopSettings weapon in data.weapons) {
            Debug.Log(weapon.weaponName);
            if (weapon.weaponName == item.itemName) {
                if (data.money >= weapon.cost) {
                    data.money -= weapon.cost;
                    weapon.hasPlayerWeapon = true;
                    item.buyButton.SetActive(false);
                    Save(data);
                } else {
                    Debug.Log("Недостаточно средств");
                    return;
                }
                return;

            }
            
        }
        
        Debug.LogError($"data.weapons has not weapon named {name}");
    }

    public void BuySkin(SkinItem item) {
        Debug.Log("BuySkin");
        foreach (WeaponShopSettings weapon in data.weapons) {
            
            if (weapon.weaponName == item.weaponName) {
                for (int i = 0; i < weapon.skins.Count; i++) {
                    if (item.skinName == weapon.skins[i].skinName) {
                        Debug.Log(item.skinName);
                        if(data.money >= weapon.skins[i].cost) {
                            data.money -= weapon.skins[i].cost;
                            weapon.skins[i].hasPlayerSkin = true;
                            item.hasPlayerSkin = true;
                            item.OnBuy();
                            Save(data);
                        } else {
                            Debug.Log("Недостаточно средств");
                            return;
                        }
                    }
                }
                
                return;

            }

        }

        Debug.LogError($"data.weapons has not weapon named {name}");
    }

}
