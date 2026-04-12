// Shader "jjh/ViewRender"
// {
//     SubShader
//     {
//         // 이 방식은 그냥 렌더 타겟 위에 색이랑 투명도를 이용하여 덮는 과정임
//         // 따라서 MainTex읽을 필요 없음
//         ZTest Always ZWrite Off Cull Off
//         Blend SrcAlpha OneMinusSrcAlpha
//         // Blend Off
//         Pass
//         {
//             CGPROGRAM
//             #pragma vertex vert_img
//             #pragma fragment frag
//             #pragma target 4.5
//             #include "UnityCG.cginc"
// 
//             // [완성된 부분: 깊이 텍스처와 필터링된 그리드 텍스처]
//             UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture); // 엔진이 준 깊이 버퍼
//             sampler2D _GridTex;
//             sampler2D _ObstacleMask;
// 
//             float4   _GridTex_TexelSize; // xy = (1/width, 1/height) - Unity 자동 제공
//             float4   _WorldOrigin;
//             float    _WorldX;
//             float    _WorldY;
//             float4x4 _InvVP;
//             float4   _FogColor;
//             float    _FogOpacity;
//             
//             float3 _PlayerPos;
//             int _GridX;
//             int _GridY;
//             float4x4 _ViewProj;
// 
// 
//             fixed4 frag(v2f_img i) : SV_Target
//             {
//                 // [완성된 부분: 월드 좌표 복원]
//                 // 1. 현재 픽셀의 깊이 값을 읽습니다.
//                 float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
//                 
//                 // 2. 플랫폼별 Reversed-Z 이슈 해결하며 NDC 좌표 구성
//                 #if defined(UNITY_REVERSED_Z)
//                     float z = rawDepth;
//                 #else
//                     float z = rawDepth * 2.0 - 1.0;
//                 #endif
// 
//                 float4 ndc = float4(i.uv.x * 2.0 - 1.0, i.uv.y * 2.0 - 1.0, z, 1.0);
// 
//                 // 3. 역VP 행렬을 곱해서 진짜 월드 공간 끄집어내기 (C++ 엔진 개발자의 전공)
//                 float4 worldPos = mul(_InvVP, ndc);
//                 worldPos /= worldPos.w;
// 
//                 // 4. 월드 XZ → 그리드 정규화 UV 좌표 변환
//                 float gfx = (worldPos.x - _WorldOrigin.x) / _WorldX;
//                 float gfz = (worldPos.z - _WorldOrigin.z) / _WorldY;
// 
//                 // 그리드 범위 밖 처리
//                 if (gfx < 0.0 || gfx >= 1.0 || gfz < 0.0 || gfz >= 1.0)
//                     return fixed4(_FogColor.rgb, _FogOpacity);
// 
//                 float grid = tex2D(_GridTex, float2(gfx, gfz)).r;
//                 if(grid == 0.0)
//                     return fixed4(_FogColor.rgb, _FogOpacity);
// 
//                 bool bObstacle = tex2D(_ObstacleMask, i.uv).r > 0.5;
//                 if(bObstacle)
//                 {
//                     float4 col = float4(1.0, 0.0, 0.0, 0.0);
//                     return col;
//                 }
// 
//                 float3 pixelWorld  = worldPos.xyz;
//                 float3 playerWorld = _PlayerPos;
//                 
//                 float3 toPlayer   = playerWorld - pixelWorld;
//                 float  worldDist  = length(toPlayer);
//                 float  cellSize   = _WorldX / (float)_GridX;
//                 int    stepCount  = max(1, (int)(worldDist / cellSize));
//                 float3 stepWorld  = toPlayer / (float)stepCount;
//                 
//                 bool blocked = false;
//                 [loop]
//                 for (int s = 1; s < stepCount; ++s)
//                 {
//                     float3 checkWorld = pixelWorld + stepWorld * (float)s;
//                 
//                     float4 clip     = mul(_ViewProj, float4(checkWorld, 1.0));
//                     float2 screenUV = clip.xy / clip.w * 0.5 + 0.5;
// 
//                     if (tex2D(_ObstacleMask, screenUV).r > 0.5)
//                     {
//                         blocked = true;
//                         break;
//                     }
//                 }
//                 
//                 if (blocked)
//                     return fixed4(_FogColor.rgb, _FogOpacity);
// 
//                 // 3x3 가우시안 블러로 장애물 그림자 경계의 계단 현상 제거
//                 // 가중치: [1 2 1 / 2 4 2 / 1 2 1] / 16
//                 float2 uv = float2(gfx, gfz);
//                 float2 ts = _GridTex_TexelSize.xy ;
//                 float visible =
//                     tex2D(_GridTex, uv + float2(-ts.x, -ts.y)).r * (1.0 / 16.0) +
//                     tex2D(_GridTex, uv + float2(  0.0, -ts.y)).r * (2.0 / 16.0) +
//                     tex2D(_GridTex, uv + float2( ts.x, -ts.y)).r * (1.0 / 16.0) +
//                     tex2D(_GridTex, uv + float2(-ts.x,   0.0)).r * (2.0 / 16.0) +
//                     tex2D(_GridTex, uv                       ).r * (4.0 / 16.0) +
//                     tex2D(_GridTex, uv + float2( ts.x,   0.0)).r * (2.0 / 16.0) +
//                     tex2D(_GridTex, uv + float2(-ts.x,  ts.y)).r * (1.0 / 16.0) +
//                     tex2D(_GridTex, uv + float2(  0.0,  ts.y)).r * (2.0 / 16.0) +
//                     tex2D(_GridTex, uv + float2( ts.x,  ts.y)).r * (1.0 / 16.0);
// 
//                 // 시야 안(1) → 투명, 시야 밖(0) → fogOpacity
//                 // smoothstep을 살짝 섞어 경계선을 더 부드럽게 연출
//                 float finalFog = _FogOpacity * (1.0 - visible);
// 
//                 return fixed4(_FogColor.rgb, finalFog);
//             }
//             ENDCG
//         }
//     }
// }

Shader "jjh/ViewRender"
{
    Properties
    {
        _MainTex ("Source Texture", 2D) = "white" {}
    }

    SubShader
    {
        ZTest Always ZWrite Off Cull Off
        Blend Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma target 4.5
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
            sampler2D _GridTex;
            sampler2D _ObstacleMask;
            float4   _GridTex_TexelSize;
            float4   _WorldOrigin;
            float    _WorldX;
            float    _WorldY;
            float4x4 _InvVP;
            float4   _FogColor;
            float    _FogOpacity;
            
            float3   _PlayerPos;
            int      _GridX;
            int      _GridY;
            float4x4 _ObstacleMaskVP;
            float4   _ObstacleMask_TexelSize;

            bool SampleObstacleMask(float2 uv)
            {
                // return tex2D(_ObstacleMask, uv).r >= 1.0f;
                // 3x3 max 필터링 주석을 푸는 것을 추천합니다 (외곽선 보정용)
                 float2 ts = _ObstacleMask_TexelSize.xy;
                 float maxVal = 0;
                 [unroll] for (int dy = -1; dy <= 1; dy++)
                 [unroll] for (int dx = -1; dx <= 1; dx++)
                      maxVal = max(maxVal, tex2D(_ObstacleMask, uv + float2(dx, dy) * ts).r);
                 return maxVal >= 0.5;
            }

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                fixed4 screenCol = tex2D(_MainTex, uv);

                float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
                
                #if defined(UNITY_REVERSED_Z)
                    float z = rawDepth;
                #else
                    float z = rawDepth * 2.0 - 1.0;
                #endif

                float4 ndc = float4(uv.x * 2.0 - 1.0, uv.y * 2.0 - 1.0, z, 1.0);
                float4 worldPos = mul(_InvVP, ndc);
                worldPos /= worldPos.w;

                float gfx = (worldPos.x - _WorldOrigin.x) / _WorldX;
                float gfz = (worldPos.z - _WorldOrigin.z) / _WorldY;

                if (gfx < 0.0 || gfx >= 1.0 || gfz < 0.0 || gfz >= 1.0)
                    return fixed4(lerp(screenCol.rgb, _FogColor.rgb, _FogOpacity), screenCol.a);

                float grid = tex2D(_GridTex, float2(gfx, gfz)).r;
                if(grid == 0.0)
                    return fixed4(lerp(screenCol.rgb, _FogColor.rgb, _FogOpacity), screenCol.a);

                // [4] 장애물 자체 마스크 체크
                float4 topDownClip4 = mul(_ObstacleMaskVP, float4(worldPos.xyz, 1.0));
                float2 topDownUV = topDownClip4.xy / topDownClip4.w * 0.5 + 0.5;
                bool bObstacle = SampleObstacleMask(topDownUV);
                if (bObstacle)
                    return fixed4(screenCol.rgb, screenCol.a); // XRay 패스가 이미 배경으로 교체함
                
                // [5] Ray-Stepping (문제 인지 주석 유지)
                float3 pixelWorld  = worldPos.xyz;
                float3 playerWorld = _PlayerPos;
                
                float3 toPlayer   = playerWorld - pixelWorld;
                float  worldDist  = length(toPlayer);
                float  cellSize   = _WorldX / (float)_GridX;
                int    stepCount  = max(1, (int)(worldDist / cellSize));
                float3 stepWorld  = toPlayer / (float)stepCount;
                
                bool blocked = false;
                [loop]
                for (int s = 1; s < stepCount; ++s)
                {
                    float3 checkWorld = pixelWorld + stepWorld * (float)s;
                    float4 topDownCheckClip = mul(_ObstacleMaskVP, float4(checkWorld, 1.0));
                    float2 topDownCheckUV = topDownCheckClip.xy / topDownCheckClip.w * 0.5 + 0.5;

                    if (SampleObstacleMask(topDownCheckUV))
                    {
                        blocked = true;
                        break;
                    }
                }
                
                if (blocked)
                {
                    return fixed4(lerp(screenCol.rgb, _FogColor.rgb, _FogOpacity), screenCol.a);
                }

                // [6] 3x3 가우시안 블러
                float2 guv = float2(gfx, gfz);
                float2 ts = _GridTex_TexelSize.xy;
                
                float visible =
                    tex2D(_GridTex, guv + float2(-ts.x, -ts.y)).r * (1.0 / 16.0) +
                    tex2D(_GridTex, guv + float2( 0.0, -ts.y)).r * (2.0 / 16.0) +
                    tex2D(_GridTex, guv + float2( ts.x, -ts.y)).r * (1.0 / 16.0) +
                    tex2D(_GridTex, guv + float2(-ts.x,  0.0)).r * (2.0 / 16.0) +
                    tex2D(_GridTex, guv                        ).r * (4.0 / 16.0) +
                    tex2D(_GridTex, guv + float2( ts.x,  0.0)).r * (2.0 / 16.0) +
                    tex2D(_GridTex, guv + float2(-ts.x,  ts.y)).r * (1.0 / 16.0) +
                    tex2D(_GridTex, guv + float2(  0.0,  ts.y)).r * (2.0 / 16.0) +
                    tex2D(_GridTex, guv + float2( ts.x,  ts.y)).r * (1.0 / 16.0);

                // [7] 최종 합성
                float fogFactor = _FogOpacity * (1.0 - visible);
                fixed3 finalRGB = lerp(screenCol.rgb, _FogColor.rgb, fogFactor);
                
                return fixed4(finalRGB, screenCol.a);
            }
            ENDCG
        }
    }
}