using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "Usables/Weapon", order = 0)]
public class WeaponSO : UsableSO
{
    public short MaxDamage;
    public short MinDamage;

    public byte FalloffStart;
    public byte FalloffEnd;

    public short SpreadDegrees;
    public short Penetration;
    public short Destruction;
    public byte APCost;
    public byte ShotsPerBurst;
    public byte SoundRadiusTenths;
}
