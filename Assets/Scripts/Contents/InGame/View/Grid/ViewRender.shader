Shader "jjh/ViewRender"
{
    SubShader
    {
        ZTest Always ZWrite Off Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma target 4.5
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            // [완성된 부분: 깊이 텍스처와 필터링된 그리드 텍스처]
            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture); // 엔진이 준 깊이 버퍼
            sampler2D _GridTex;
            sampler2D _ObstacleMask;

            float4   _GridTex_TexelSize; // xy = (1/width, 1/height) - Unity 자동 제공
            float4   _WorldOrigin;
            float    _WorldX;
            float    _WorldY;
            float4x4 _InvVP;
            float4   _FogColor;
            float    _FogOpacity;
            
            float3 _PlayerPos;
            int _GridX;
            int _GridY;
            float4x4 _ViewProj;


            fixed4 frag(v2f_img i) : SV_Target
            {
                // [완성된 부분: 월드 좌표 복원]
                // 1. 현재 픽셀의 깊이 값을 읽습니다.
                float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                
                // 2. 플랫폼별 Reversed-Z 이슈 해결하며 NDC 좌표 구성
                #if defined(UNITY_REVERSED_Z)
                    float z = rawDepth;
                #else
                    float z = rawDepth * 2.0 - 1.0;
                #endif

                float4 ndc = float4(i.uv.x * 2.0 - 1.0, i.uv.y * 2.0 - 1.0, z, 1.0);

                // 3. 역VP 행렬을 곱해서 진짜 월드 공간 끄집어내기 (C++ 엔진 개발자의 전공)
                float4 worldPos = mul(_InvVP, ndc);
                worldPos /= worldPos.w;

                // 4. 월드 XZ → 그리드 정규화 UV 좌표 변환
                float gfx = (worldPos.x - _WorldOrigin.x) / _WorldX;
                float gfz = (worldPos.z - _WorldOrigin.z) / _WorldY;

                // 그리드 범위 밖 처리
                if (gfx < 0.0 || gfx >= 1.0 || gfz < 0.0 || gfz >= 1.0)
                    return fixed4(_FogColor.rgb, _FogOpacity);

                float grid = tex2D(_GridTex, float2(gfx, gfz)).r;
                if(grid == 0.0)
                    return fixed4(_FogColor.rgb, _FogOpacity);

                bool bObstacle = tex2D(_ObstacleMask, i.uv).r == 1.0;
                if(bObstacle)
                    return tex2D(_MainTex, i.uv); // 장애물은 원래 텍스처 그대로 보여주기

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
                
                    float4 clip     = mul(_ViewProj, float4(checkWorld, 1.0));
                    float2 screenUV = clip.xy / clip.w * 0.5 + 0.5;
                
                    if (tex2D(_ObstacleMask, screenUV).r > 0.5)
                    {
                        blocked = true;
                        break;
                    }
                }
                
                if (blocked)
                    return fixed4(_FogColor.rgb, _FogOpacity);

                // 3x3 가우시안 블러로 장애물 그림자 경계의 계단 현상 제거
                // 가중치: [1 2 1 / 2 4 2 / 1 2 1] / 16
                float2 uv = float2(gfx, gfz);
                float2 ts = _GridTex_TexelSize.xy ;
                float visible =
                    tex2D(_GridTex, uv + float2(-ts.x, -ts.y)).r * (1.0 / 16.0) +
                    tex2D(_GridTex, uv + float2(  0.0, -ts.y)).r * (2.0 / 16.0) +
                    tex2D(_GridTex, uv + float2( ts.x, -ts.y)).r * (1.0 / 16.0) +
                    tex2D(_GridTex, uv + float2(-ts.x,   0.0)).r * (2.0 / 16.0) +
                    tex2D(_GridTex, uv                       ).r * (4.0 / 16.0) +
                    tex2D(_GridTex, uv + float2( ts.x,   0.0)).r * (2.0 / 16.0) +
                    tex2D(_GridTex, uv + float2(-ts.x,  ts.y)).r * (1.0 / 16.0) +
                    tex2D(_GridTex, uv + float2(  0.0,  ts.y)).r * (2.0 / 16.0) +
                    tex2D(_GridTex, uv + float2( ts.x,  ts.y)).r * (1.0 / 16.0);

                // 시야 안(1) → 투명, 시야 밖(0) → fogOpacity
                // smoothstep을 살짝 섞어 경계선을 더 부드럽게 연출
                float finalFog = _FogOpacity * (1.0 - visible);

                return fixed4(_FogColor.rgb, finalFog);
            }
            ENDCG
        }
    }
}