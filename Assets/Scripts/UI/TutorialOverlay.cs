using UnityEngine;
using UnityEngine.UI;

namespace EndlessRails
{
    /// <summary>
    /// First-launch tutorial overlay explaining the swipe controls.
    /// Dismissed by tapping anywhere; never shown again after the first session.
    /// </summary>
    public class TutorialOverlay : MonoBehaviour
    {
        [SerializeField] private Text bodyText;

        private void Awake()
        {
            if (bodyText == null)
            {
                bodyText = transform.FindDeep<Text>("BodyText");
            }
            if (bodyText != null)
            {
                bodyText.text =
                    "SWIPE LEFT / RIGHT  -  change lane\n" +
                    "SWIPE UP  -  jump over barriers\n" +
                    "SWIPE DOWN  -  slide under beams\n" +
                    "TAP  -  pause the game\n\n" +
                    "Collect coins, grab power-ups, and don't get caught!\n\n" +
                    "TAP ANYWHERE TO START";
            }
        }

        private void OnEnable()
        {
            SwipeInputManager.OnTap += HandleTap;
        }

        private void OnDisable()
        {
            SwipeInputManager.OnTap -= HandleTap;
        }

        private void HandleTap()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            Dismiss();
        }

        public void Dismiss()
        {
            SaveSystem save = SaveSystem.Instance;
            if (save != null)
            {
                save.Data.tutorialShown = true;
                save.MarkDirty();
                save.Save();
            }
            gameObject.SetActive(false);
            UIManager.Instance?.Show("MainMenu");
        }
    }
}
