using System.Collections;
using System.Collections.Generic;
using InfimaGames.LowPolyShooterPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SaveSystem;

public class SkinItem : MonoBehaviour
{
    public string skinName;
    public string weaponName;
    public GameObject buyButton;
    public Shop shop;
    public TMP_Text nameText;
    public TMP_Text costText;
    [SerializeField] private int itemCost;
    private int selectedInd = -1;
    private Image image;
    public bool selected = false;
    private bool notBasic = true;
    private int thisIndex = -1;
    private int basicInd = -1;
    private bool isBuyed;
    public bool hasPlayerSkin;

    void Start() {
        image = GetComponent<Image>();
        nameText.text = skinName;
        foreach (WeaponShopSettings weapon in shop.data.weapons) {
            if (weapon.weaponName == weaponName) {
                for (int i = 0; i < weapon.skins.Count - 1; i++) {
                    //weapon.skins[i].selected = false;
                    if (weapon.skins[i].skinName == skinName) {
                        itemCost = weapon.skins[i].cost;
                    }
                }
            }
        }
        costText.text = $"{itemCost}$";
    }
    public void OnBuy() {
        int ind = -1;
        if (costText.text == "Select") {
            foreach (WeaponShopSettings weapon in shop.data.weapons) {
                if (weapon.weaponName == weaponName) {
                    for (int i = 0; i < weapon.skins.Count; i++) {
                        weapon.skins[i].selected = false;
                        if (weapon.skins[i].skinName == skinName) {
                            ind = i;
                        }
                    }
                    if (ind != -1) {
                        weapon.skins[ind].selected = true;
                        weapon.currentSkinIndex = ind;
                    } else {
                        Debug.LogError("CHECK IND VALUE");
                    }
                }
            }
        } else {
            //costText.text = "Select";
        }
    }

    void Update() {
        if (itemCost == 0) {
            foreach (WeaponShopSettings weapon in shop.data.weapons) {
                if (weapon.weaponName == weaponName) {
                    for (int i = 0; i < weapon.skins.Count; i++) {
                        if (weapon.skins[i].skinName == skinName) {
                            weapon.skins[i].hasPlayerSkin = true;
                            hasPlayerSkin = true;
                            break;
                        }
                    }
                    break;
                }
            }
        }

        selectedInd = -1;
        thisIndex = -1;
        basicInd = -1;
        foreach (WeaponShopSettings weapon in shop.data.weapons) {
            if (weapon.weaponName == weaponName) {
                for (int i = 0; i < weapon.skins.Count; i++) {

                    if (weapon.skins[i].skinName == skinName) {
                        thisIndex = i;
                    }
                    if (weapon.skins[i].selected) {
                        Debug.Log($"Selected {weapon.skins[i].skinName}");
                        selectedInd = i;
                    }

                    if (weapon.skins[i].skinName == "Basic_01") {
                        basicInd = i;
                    }
                }
                Debug.Log(thisIndex);
                if (thisIndex != -1) {
                    itemCost = weapon.skins[thisIndex].cost;
                } else {
                    Debug.Log(-1);
                }
                
                break;
            }

        }


        if (selectedInd == -1) {
            selectedInd = basicInd;
        }
        if (selectedInd == thisIndex) {
            image.color = Color.green;
            buyButton.GetComponent<Image>().color = Color.blue;
            buyButton.gameObject.SetActive(false);
        } else if (hasPlayerSkin) {
            image.color = Color.blue;
            buyButton.GetComponent<Image>().color = Color.green;
            buyButton.gameObject.SetActive(true);
            costText.text = "Select";
        } else {
            image.color = Color.blue;
            buyButton.GetComponent<Image>().color = Color.green;
            buyButton.gameObject.SetActive(true);
            costText.text = $"{itemCost}$";
        }
    }
}
