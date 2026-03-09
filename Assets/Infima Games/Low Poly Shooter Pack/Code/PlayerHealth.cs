using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using UnityEngine.Rendering.PostProcessing;
//using UnityEngine.Rendering;
//using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;

namespace InfimaGames.LowPolyShooterPack
{
    public class PlayerHealth : MonoBehaviour
    {
        public float currentHealth;
        public float maxHealth;
        public TMP_Text healthText;
        public TMP_Text distanceText;
        //public PostProcessVolume postProcessVolume;
        //private Vignette vignette;
        public Transform camera;

        void Start()
        {
            currentHealth = maxHealth;
            healthText.text = $"Health({currentHealth}/{maxHealth})";
            //vignette = postProcessVolume.profile.GetSetting<Vignette>();
        }

        // Update is called once per frame
        void Update() {
            /*if (vignette.intensity.value > 0) {
                vignette.intensity.value -= Time.deltaTime * 0.05f;
            }*/
            RaycastHit hit;
            Debug.DrawRay(camera.transform.position, camera.forward, Color.red);
            if (Physics.Raycast(camera.transform.position, camera.forward, out hit)) {
                distanceText.text = $"Distance: {hit.distance} m";
            }
        }

        public void CheckHit(float damage) {
            currentHealth -= damage;
            healthText.text = $"Health({currentHealth}/{maxHealth})";
            //vignette.intensity.value = 0.43f;
            if (currentHealth <= 0) {
                #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
                #else
                        Application.Quit();
                #endif
            }
        }
    }
}
