using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    private bool drawLine = false;
    private int segmentCount = 30;
    private float predictionTime = 1.0f;
    private float lineAlpha = 1f;
    private float lineFadeOutDeltaTime = 2f;

    [SerializeField]
    private Material arcMaterial;
    [SerializeField]
    private float arcWidth = 0.04f;

    private LineRenderer[] lineRenderers;

    private Vector3 initialVelocity;
    private Vector3 arcStartPosition;


    // Start is called before the first frame update
    void Start()
    {
        CreateLineRendererObjects();
    }

    // Update is called once per frame
    void Update()
    {
        if (drawLine) {
            // 放物線を表示
            float timeStep = predictionTime / segmentCount;
            bool draw = false;
            float hitTime = float.MaxValue;
            if (lineAlpha > 0) {
                lineAlpha -= Time.deltaTime * 1/lineFadeOutDeltaTime;
                //Debug.Log(lineAlpha);
            }
            for (int i = 0; i < segmentCount; i++)
            {
                // 線の座標を更新
                float startTime = timeStep * i;
                float endTime = startTime + timeStep;
                SetLineRendererPosition(i, startTime, endTime, !draw);

                // 衝突判定
                if (!draw)
                {
                    hitTime = GetArcHitTime(startTime, endTime);
                    if (hitTime != float.MaxValue)
                    {
                        draw = true; // 衝突したらその先の放物線は表示しない
                    }
                }
            }
        }
        else {
            // 放物線とマーカーを表示しない
            for (int i = 0; i < lineRenderers.Length; i++)
            {
                lineRenderers[i].enabled = false;
            }
        }
    }

    private Vector3 GetArcPositionAtTime(float time)
    {
        return (arcStartPosition + ((initialVelocity * time) + (0.5f * time * time) * Physics.gravity));
    }

    private void SetLineRendererPosition(int index, float startTime, float endTime, bool draw = true)
    {
        lineRenderers[index].SetPosition(0, GetArcPositionAtTime(startTime));
        lineRenderers[index].SetPosition(1, GetArcPositionAtTime(endTime));
        lineRenderers[index].enabled = draw;

        lineRenderers[index].SetColors(new Color(1, 1, 1, 0), new Color(1, 1, 1, 0));
    }

    private void CreateLineRendererObjects()
    {
        // 親オブジェクトを作り、LineRendererを持つ子オブジェクトを作る
        GameObject arcObjectsParent = new GameObject("ArcObject");

        lineRenderers = new LineRenderer[segmentCount];
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject newObject = new GameObject("LineRenderer_" + i);
            newObject.transform.SetParent(arcObjectsParent.transform);
            lineRenderers[i] = newObject.AddComponent<LineRenderer>();

            // 光源関連を使用しない
            lineRenderers[i].receiveShadows = false;
            lineRenderers[i].reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            lineRenderers[i].lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            lineRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            // 線の幅とマテリアル
            //lineRenderers[i].material = arcMaterial;
            lineRenderers[i].startWidth = arcWidth;
            lineRenderers[i].endWidth = arcWidth;
            lineRenderers[i].numCapVertices = 5;
            lineRenderers[i].enabled = false;
        }
    } 

    private float GetArcHitTime(float startTime, float endTime)
    {
        // Linecastする線分の始終点の座標
        Vector3 startPosition = GetArcPositionAtTime(startTime);
        Vector3 endPosition = GetArcPositionAtTime(endTime);

        // 衝突判定
        RaycastHit hitInfo;
        if (Physics.Linecast(startPosition, endPosition, out hitInfo))
        {
            // 衝突したColliderまでの距離から実際の衝突時間を算出
            float distance = Vector3.Distance(startPosition, endPosition);
            return startTime + (endTime - startTime) * (hitInfo.distance / distance);
        }
        return float.MaxValue;
    }

    public void LineDrawOn() {
        drawLine = true;
        lineAlpha = 1f;
    }
    public void LineDrawOff() {
        drawLine = false;
    }
    public void SetPosition(Vector3 pos) {
        arcStartPosition = pos;
    }

    public void SetVelocity(Vector3 vel) {
        initialVelocity = vel;
    }
}
