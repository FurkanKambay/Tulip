using UnityEngine.UIElements;

namespace FK.Common.Extensions
{
    public enum UINavigationStatus
    {
        NoFocus,
        Allowed,
        Locked
    }

    public static class UIToolkitExtensions
    {
        public static UINavigationStatus GetNavigationStatus(this Focusable self, out Focusable focusedElement)
        {
            focusedElement = self?.focusController?.focusedElement;
            return focusedElement?.delegatesFocus switch
            {
                true => UINavigationStatus.Allowed,
                false => UINavigationStatus.Locked,
                null => UINavigationStatus.NoFocus
            };
        }

        public static void ForceSetFocus(this VisualElement self, bool focused)
        {
            if (self == null)
                return;

            self.visible = focused;

            if (focused)
                self.Focus();
            else
                self.Blur();
        }
    }
}
