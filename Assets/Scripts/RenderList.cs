using UnityEngine;

public class RenderList : MonoBehaviour
{
    public static string[] WeaponIndex = new string[6] { "Hand", "Axe", "Spear", "Bow", "Staff", "Sword" };

    public static RuntimeAnimatorController[,,] Animates = new RuntimeAnimatorController[6,10,4];

    public static RuntimeAnimatorController GetAnimate(int weaponType, int style, int color)
    {
        if (Animates[weaponType, style, color] != null)
            return Animates[weaponType, style, color];

        int seq = style + color * 10;
        string pnum = seq.ToString().PadLeft(2, '0');

        var animate = Resources.Load("Player/Animate/" + WeaponIndex[weaponType] + "/" + pnum
            , typeof(RuntimeAnimatorController));

        Animates[weaponType, style, color] = (RuntimeAnimatorController)animate;
        return (RuntimeAnimatorController)animate;
    }
}
