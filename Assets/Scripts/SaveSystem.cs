using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class SaveSystem : MonoBehaviour {

    public string savePath;
    [System.Serializable]
    public class Skin {
        public string skinName;
        public bool hasPlayerSkin;
        public int skinMaterialIndex;
        public int cost;
        public bool selected;
    }

    [System.Serializable]
    public class WeaponShopSettings {
        public string weaponName;
        public bool hasPlayerWeapon;
        public List<Skin> skins;
        public int currentSkinIndex;
        public int cost;
    }

    [System.Serializable]
    public class PlayerData {
        public List<WeaponShopSettings> weapons = new List<WeaponShopSettings>();
        public int money;
    }

    //public PlayerData data;

    public PlayerData Load(string path) {
        Debug.Log(path + "/jsonPlayer.json");
        PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(File.ReadAllText(path + "/jsonPlayer.json"));
        return playerData;
    }
    public void Save(PlayerData data) {
        Debug.Log("Save");
        var playerData = data;

        File.WriteAllText(
            savePath + "/jsonPlayer.json",
            JsonConvert.SerializeObject(playerData, Formatting.Indented, new JsonSerializerSettings() {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore //Отмена залупливания обращений 
            })
        );
    }

}


