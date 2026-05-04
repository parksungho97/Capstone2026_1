using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

public class ViewContext : MonoBehaviour
{
    [Header("Compute")]
    [SerializeField] private ComputeShader viewBuildComputeShader;

    [Header("Post Process")]
    [SerializeField] private Color _fogColor = Color.black;
    [SerializeField][Range(0f, 1f)] private float _fogOpacity = 0.7f;

    [SerializeField] private uint gridX;
    [SerializeField] private uint gridY;
    [SerializeField] private float worldX;
    [SerializeField] private float worldY;

    [SerializeField] private string playerTag;

    [SerializeField] private LayerMask obstacleLayer;

    [SerializeField] private List<FOV> fovs = new List<FOV>();

    public ViewInfo ViewInfo { get; private set; }

    private void Awake()
    {
        // 장애물 스텐실을 먼저 구워야됌
        ViewInfo = new ViewInfo(gridX, gridY, worldX, worldY);
        viewBuilder = new ViewBuilder(viewBuildComputeShader);
        viewBuilder.BindViewGridBuffer((int)gridX, (int)gridY);
        viewRenderer = new ViewRenderer(Camera.main);

        GameObject[] objects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        for (int i = 0; i < objects.Length; i++)
        {
            if ((1 << objects[i].layer & obstacleLayer) != 0)
                ViewInfo.RegistObstacle(objects[i]);
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        viewCulling = new ViewCulling();
        foreach (var cullableObject in players)
        {
            if (cullableObject != gameObject)
                viewCulling.RegistCulledObject(cullableObject);
        }

        viewCulling.AddObstacleLayer(obstacleLayer);
    }

    private void LateUpdate()
    {
        if (refGameObject == null)
            return;

        foreach (var fov in fovs)
        {
            ViewInfo.AddFovInfo(
                refGameObject.transform.position,
                refGameObject.transform.forward,
                fov.angle,
                fov.radius);
        }

        viewCulling.Cull(ViewInfo);

        viewBuilder.BuildViewGrid(ViewInfo);
        viewRenderer.SetFogColor(_fogColor, _fogOpacity);
        viewRenderer.RenderView(viewBuilder.ViewGridBuffer, ViewInfo, refGameObject.transform.position);

        ViewInfo.Clear();
    }

    private void OnDisable()
    {
        viewBuilder.Cleanup();
        viewRenderer.Cleanup();
    }

    public void Initalize(GameObject refGameObject)
    {
        Debug.Assert(refGameObject);
        this.refGameObject = refGameObject;
    }

    private ViewCulling viewCulling;
    private ViewBuilder viewBuilder;
    private ViewRenderer viewRenderer;
    private GameObject refGameObject;
}