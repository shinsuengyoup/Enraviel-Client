using UnityEngine;

public class ConstData : MonoBehaviour
{
    public static readonly Color CLR_BLACK = new Color(0, 0, 0, 0);
    public static readonly Color CLR_RED_tile = new Color(255f / 255f, 0f, 0f, 100f / 255f);
    public static readonly Color CLR_Gray_tile = new Color(0f, 0f, 0f, 150f);

    public static readonly Vector3 gridMapOffset = new Vector3(0.25f, 0.25f, 0);
    public static readonly Vector3 tilePosOffset = new Vector3(0.25f, 0.5f, 0);
}
