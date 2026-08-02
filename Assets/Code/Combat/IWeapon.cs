using FK.Tulip.Data.Items;

namespace FK.Tulip.Combat
{
    public interface IWeapon
    {
        WeaponAsset Asset { get; }
        Health Owner { get; }
    }
}
