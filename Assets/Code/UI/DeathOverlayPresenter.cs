using SaintsField;
using Tulip.Character;
using Tulip.Gameplay;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tulip.UI
{
    public sealed class DeathOverlayPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] UIDocument document;
        [SerializeField, Required] Health health;
        [SerializeField, Required] Respawner respawner;

        // ReSharper disable UnusedMember.Local
        [CreateProperty] bool IsOverlayDisplayed => health.IsDead;
        [CreateProperty] bool IsRespawnButtonDisplayed => respawner.CanRespawn;
        [CreateProperty] bool IsCountdownDisplayed => !respawner.CanRespawn;
        [CreateProperty] int SecondsUntilRespawn => Mathf.CeilToInt(respawner.SecondsUntilRespawn);

        [CreateProperty] string DeathReason =>
            health.LatestDeathSource ? health.LatestDeathSource.Entity.Name : "No Death Source";
        // ReSharper restore UnusedMember.Local

        private VisualElement root;
        private Button respawnButton;

        private void OnEnable()
        {
            document.enabled = true;

            root = document.rootVisualElement;
            root.dataSource = this;

            respawnButton = root.Q<Button>();
            respawnButton.RegisterCallback<ClickEvent>(HandleRespawnClicked);
        }

        private void OnDisable() => respawnButton.UnregisterCallback<ClickEvent>(HandleRespawnClicked);

        private void HandleRespawnClicked(ClickEvent _) => respawner.TryRespawn();
    }
}
