using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EmeraldAI
{
    public class HealthDrawer : MonoBehaviour
    {
        public EmeraldGeneralTargetBridge emeraldGeneralTargetBridge;

        public Image healthBar;
        void Start()
        {
            healthBar.fillAmount = (float)emeraldGeneralTargetBridge.Health / (float)emeraldGeneralTargetBridge.StartingHealth;
        }
        public void UbdateHealthBar()
        {
            healthBar.fillAmount = (float)emeraldGeneralTargetBridge.Health / (float)emeraldGeneralTargetBridge.StartingHealth;
        }
    }
}


