Shader "Hidden/ViewRender"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture; 
            sampler2D _FovDepthTex;
            sampler2D _ObstacleMask;
            
            float4x4 _InvVP;   
            float4x4 _FovViewProj;
            fixed4 _FogColor;
            float _FogOpacity;

            float3 _PlayerPos;
            float3 _PlayerForward;
            float _MaxDist;
            float _ViewAngle;

            v2f vert (appdata v) 
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            int IsInsideFOV(float2 pixelWorldPos)
            {
                // 2. 플레이어와의 거리 및 방향 계산
                float2 dirToPixel = pixelWorldPos - _PlayerPos.xz;
                float dist = length(dirToPixel);
                float2 normDir = normalize(dirToPixel);

                // 3. 부채꼴 판별 조건
                // 조건 A: 거리가 최대 사거리 이내인가
                bool inRange = dist <= _MaxDist;
                
                // 조건 B: 플레이어 정면과 픽셀 방향 사이의 각도가 시야각 절반 이내인가
                float dotFwd = dot(normDir, normalize(_PlayerForward.xz));
                float cosHalfAngle = cos(radians(_ViewAngle * 0.5));
                bool inAngle = dotFwd >= cosHalfAngle;

                if(inRange && inAngle)
                    return 1; // 시야 내부
                else
                    return 0; // 시야 외부
            }

            fixed4 frag (v2f psInput) : SV_Target 
            {
                fixed4 col = tex2D(_MainTex, psInput.uv);
                float rawMainDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, psInput.uv);
            
                // 1. 월드 좌표 복원 (Reversed-Z 플랫폼 대응)
                #if defined(UNITY_REVERSED_Z)
                    float z = rawMainDepth;
                #else
                    float z = rawMainDepth * 2 - 1;
                #endif
            
                float4 ndc = float4(psInput.uv.x * 2 - 1, psInput.uv.y * 2 - 1, z, 1);
                #if UNITY_UV_STARTS_AT_TOP
                    ndc.y *= -1;
                #endif
            
                float4 worldPos = mul(_InvVP, ndc);
                worldPos /= worldPos.w;
            
                // 2. FOV 내부 판정
                float4 foggedCol = lerp(col, _FogColor, _FogOpacity);
                if (IsInsideFOV(worldPos.xz) == 0) 
                    return foggedCol;

                float4 stencilSample = tex2D(_ObstacleMask, psInput.uv);
                if(stencilSample.r == 1.0)
                    return col;
            
                // 3. FOV 카메라 기준으로 좌표 변환
                float4 fovClipPos = mul(_FovViewProj, float4(worldPos.xyz, 1));
                float2 fovUV = (fovClipPos.xy / fovClipPos.w) * 0.5 + 0.5;
            
                // [중요] fovClipPos.w 가 바로 카메라로부터의 실제 거리(Linear Depth)입니다.
                float currentPixelDist = fovClipPos.w; 
            
                // 4. 장애물 거리 샘플링
                float fovRawDepth = SAMPLE_DEPTH_TEXTURE(_FovDepthTex, fovUV);
                
                // fovCamera의 파라미터를 수동으로 계산하거나, 
                // fovCamera와 메인카메라의 Near/Far가 같다면 아래 함수 사용
                float obstacleDist = LinearEyeDepth(fovRawDepth); 
            
                // 5. 최종 비교 (약간의 Bias를 더해 샌드페이퍼 현상 방지)
                if(currentPixelDist > obstacleDist + 0.1)
                    return foggedCol;
               

                return col;
            }
            ENDCG
        }
    }
}