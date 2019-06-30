
using UnityEngine;

public class DayTime : MonoBehaviour
{
    public RectTransform G1;
    public RectTransform G2;

    float G1x = 0f;

    float G2x = 256f;

    float ftime = 0;
    void FixedUpdate()
    {
        if (ftime < 10)
        {
            ftime += Time.fixedDeltaTime;
            return;
        }
        else
        {
            ftime = 0;
        }

        if (G1.position.x <= -256f)
        {
            G1x = 256;
        }
        G1x -= 10;

        G1.localPosition = new Vector2(G1x, 0);

        if (G2.position.x <= -256f)
        {
            G2x = 256f;
        }
        G2x -= 10;
        G2.localPosition = new Vector2(G2x, 0);
    }
}
