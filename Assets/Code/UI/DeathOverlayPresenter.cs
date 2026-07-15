using Furkan.Common;
using Tulip.Character;
using Tulip.Combat;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tulip.UI
{
    public sealed class DeathOverlayPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Required] PanelRenderer panelRenderer;

        [Header("Injected State")]
        [SerializeField, Required] Health health;
        [SerializeField, Required] Respawner respawner;

        // ReSharper disable UnusedMember.Local
        [CreateProperty] bool IsOverlayDisplayed => health && health.IsDead;
        [CreateProperty] bool IsRespawnButtonDisplayed => respawner && respawner.CanRespawn;
        [CreateProperty] bool IsCountdownDisplayed => respawner && !respawner.CanRespawn;
        [CreateProperty] int SecondsUntilRespawn => !respawner ? 0 : Mathf.CeilToInt(respawner.SecondsUntilRespawn);

        [CreateProperty] string DeathReason => !health ? "" :
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
