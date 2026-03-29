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

    [SerializeField] private Camera mainCamera;

    [SerializeField] private uint gridX;
    [SerializeField] private uint gridY;
    [SerializeField] private float worldX;
    [SerializeField] private float worldY;

    [SerializeField] private GameObject player;
    [SerializeField] private FOV[] fovs;
    [SerializeField] private GameObject[] obstacles;
    [SerializeField] private GameObject[] cullableObjects;

    [SerializeField] private LayerMask[] layers;

    public ViewInfo ViewInfo { get; private set; }

    private void Awake()
    {
        // 장애물 스텐실을 먼저 구워야됌
        ViewInfo = new ViewInfo(gridX, gridY, worldX, worldY);
        viewBuilder = new ViewBuilder(viewBuildComputeShader);
        viewBuilder.BindViewGridBuffer((int)gridX, (int)gridY);
        viewRenderer = new ViewRenderer(mainCamera);

        for (int i = 0; i < obstacles.Length; i++)
            ViewInfo.RegistObstacle(obstacles[i]);

        viewCulling = new ViewCulling();
        foreach (var cullableObject  in cullableObjects)
            viewCulling.RegistCulledObject(cullableObject);

        foreach(var layer  in layers)
            viewCulling.AddObstacleLayer(layer);
    }


    private void LateUpdate()
    {
        foreach(var fov  in fovs)
        {
                ViewInfo.AddFovInfo(
                    fov.transform.position,
                    fov.transform.forward,
                    fov.Angle,
                    fov.Radius);
        }

        viewCulling.Cull(ViewInfo);

        viewBuilder.BuildViewGrid(ViewInfo);
        viewRenderer.SetFogColor(_fogColor, _fogOpacity);
        viewRenderer.RenderView(viewBuilder.ViewGridBuffer, ViewInfo, player.transform.position);

        ViewInfo.Clear();
    }

    private void OnDisable()
    {
        viewBuilder.Cleanup();
        viewRenderer.Cleanup();
    }

    private ViewCulling viewCulling;
    private ViewBuilder viewBuilder;
    private ViewRenderer viewRenderer;
}