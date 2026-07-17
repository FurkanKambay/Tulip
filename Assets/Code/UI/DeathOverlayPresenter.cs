using FK.Common;
using FK.Tulip.Character;
using FK.Tulip.Combat;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace FK.Tulip.UI
{
    public sealed class DeathOverlayPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] private PanelRenderer panelRenderer;

        [Header("Injected State")]
        [SerializeField, Required] private Health health;
        [SerializeField, Required] private Respawner respawner;

        // ReSharper disable UnusedMember.Local
        [CreateProperty] private bool IsOverlayDisplayed => health && health.IsDead;
        [CreateProperty] private bool IsRespawnButtonDisplayed => respawner && respawner.CanRespawn;
        [CreateProperty] private bool IsCountdownDisplayed => respawner && !respawner.CanRespawn;
        [CreateProperty] private int SecondsUntilRespawn => !respawner ? 0 : Mathf.CeilToInt(respawner.SecondsUntilRespawn);

        [CreateProperty] private string DeathReason => !health ? "" :
            health.LatestDeathSource ? health.LatestDeathSource.Entity.Name : "No Death Source";
        // ReSharper restore UnusedMember.Local

        private VisualElement root;
        private Button respawnButton;

        public void SetPlayer(TangibleEntity playerEntity)
        {
            health = playerEntity.Health;
            respawner = playerEntity.GetComponentInChildren<Respawner>();
        }

        private void OnEnable()
        {
            panelRenderer.enabled = true;
            panelRenderer.RegisterUIReloadCallback(UI_Reloaded);
        }

        private void OnDisable()
        {
            panelRenderer.UnregisterUIReloadCallback(UI_Reloaded);
            respawnButton?.UnregisterCallback<ClickEvent>(HandleRespawnClicked);
        }

        private void UI_Reloaded(PanelRenderer renderer, VisualElement rootElement)
        {
            root = rootElement;
            root.dataSource = this;

            respawnButton = root.Q<Button>();
            respawnButton.RegisterCallback<ClickEvent>(HandleRespawnClicked);
        }

        private void HandleRespawnClicked(ClickEvent _) =>
            respawner.TryRespawn();
    }
}
