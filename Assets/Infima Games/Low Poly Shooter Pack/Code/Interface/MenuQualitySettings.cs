//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;
using UnityEngine.SceneManagement;
// Post Processing Stack v1 несовместим с HDRP. Для HDRP требуется UnityEngine.Rendering.HighDefinition
// using UnityEngine.Rendering.PostProcessing;

namespace InfimaGames.LowPolyShooterPack.Interface
{
    /// <summary>
    /// Quality Settings Menu.
    /// </summary>
    public class MenuQualitySettings : Element
    {
        #region FIELDS SERIALIZED
        
        [Title(label: "Settings")]

        [Tooltip("Canvas to play animations on.")]
        [SerializeField]
        private GameObject animatedCanvas;

        [Tooltip("Animation played when showing this menu.")]
        [SerializeField]
        private AnimationClip animationShow;

        [Tooltip("Animation played when hiding this menu.")]
        [SerializeField]
        private AnimationClip animationHide;

        #endregion
        
        #region FIELDS
        
        /// <summary>
        /// Animation Component.
        /// </summary>
        private Animation animationComponent;
        /// <summary>
        /// If true, it means that this menu is enabled and showing properly.
        /// </summary>
        private bool menuIsEnabled;

        // Post Processing отключен из-за несовместимости с HDRP
        // private PostProcessVolume postProcessingVolume;
        // private PostProcessVolume postProcessingVolumeScope;
        // private DepthOfField depthOfField;

        #endregion

        #region UNITY

        private void Start()
        {
            //Hide pause menu on start.
            animatedCanvas.GetComponent<CanvasGroup>().alpha = 0;
            //Get canvas animation component.
            animationComponent = animatedCanvas.GetComponent<Animation>();

            // Post Processing отключен из-за несовместимости с HDRP
            // postProcessingVolume = GameObject.Find("Post Processing Volume")?.GetComponent<PostProcessVolume>();
            // postProcessingVolumeScope = GameObject.Find("Post Processing Volume Scope")?.GetComponent<PostProcessVolume>();
            // if(postProcessingVolume != null)
            //     postProcessingVolume.profile.TryGetSettings(out depthOfField);
        }

        protected override void Tick()
        {
            //Switch. Fades in or out menu based on cursor's state.
            bool cursorLocked = characterBehaviour.IsCursorLocked();
            switch (cursorLocked)
            {
                //Hide.
                case true when menuIsEnabled:
                    Hide();
                    break;
                //Show.
                case false when !menuIsEnabled:
                    Show();
                    break;
            }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Shows menu by playing an animation.
        /// </summary>
        private void Show()
        {
            //Enabled.
            menuIsEnabled = true;

            //Play Clip.
            animationComponent.clip = animationShow;
            animationComponent.Play();

            // Post Processing отключен из-за несовместимости с HDRP
            // if(depthOfField != null)
            //     depthOfField.active = true;
        }
        /// <summary>
        /// Hides menu by playing an animation.
        /// </summary>
        private void Hide()
        {
            //Disabled.
            menuIsEnabled = false;

            //Play Clip.
            animationComponent.clip = animationHide;
            animationComponent.Play();

            // Post Processing отключен из-за несовместимости с HDRP
            // if(depthOfField != null)
            //     depthOfField.active = false;
        }

        /// <summary>
        /// Sets whether post processing is enabled, or disabled.
        /// </summary>
        private void SetPostProcessingState(bool value = true)
        {
            // Post Processing отключен из-за несовместимости с HDRP
            // if(postProcessingVolume != null)
            //     postProcessingVolume.enabled = value;
            // if(postProcessingVolumeScope != null)
            //     postProcessingVolumeScope.enabled = value;
        }

        /// <summary>
        /// Sets graphic quality to very low.
        /// </summary>
        public void SetQualityVeryLow()
        {
            //Set Quality.
            QualitySettings.SetQualityLevel(0);
            //Disable Post Processing.
            SetPostProcessingState(false);
        }
        /// <summary>
        /// Sets graphic quality to low.
        /// </summary>
        public void SetQualityLow()
        {
            //Set Quality.
            QualitySettings.SetQualityLevel(1);
            //Disable Post Processing.
            SetPostProcessingState(false);
        }

        /// <summary>
        /// Sets graphic quality to medium.
        /// </summary>
        public void SetQualityMedium()
        {
            //Set Quality.
            QualitySettings.SetQualityLevel(2);
            //Enable Post Processing.
            SetPostProcessingState();
        }
        /// <summary>
        /// Sets graphic quality to high.
        /// </summary>
        public void SetQualityHigh()
        {
            //Set Quality.
            QualitySettings.SetQualityLevel(3);
            //Enable Post Processing.
            SetPostProcessingState();
        }

        /// <summary>
        /// Sets graphic quality to very high.
        /// </summary>
        public void SetQualityVeryHigh()
        {
            //Set Quality.
            QualitySettings.SetQualityLevel(4);
            //Enable Post Processing.
            SetPostProcessingState();
        }
        /// <summary>
        /// Sets graphic quality to ultra.
        /// </summary>
        public void SetQualityUltra()
        {
            //Set Quality.
            QualitySettings.SetQualityLevel(5);
            //Enable Post Processing.
            SetPostProcessingState();
        }

        public void Restart()
        {
            //Path.
            string sceneToLoad = SceneManager.GetActiveScene().path;
             
            #if UNITY_EDITOR
            //Load scene.
            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(sceneToLoad, new LoadSceneParameters(LoadSceneMode.Single));
            #else
            //Load scene.
            SceneManager.LoadSceneAsync(sceneToLoad, new LoadSceneParameters(LoadSceneMode.Single));
            #endif
        }
        public void Quit()
        {
            //Quit.
            Application.Quit();
            //new SceneLoader().OpenMenuScene();
        }

        #endregion
    }
}
