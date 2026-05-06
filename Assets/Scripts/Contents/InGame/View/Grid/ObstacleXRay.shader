Shader "jjh/ObstacleXRay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // addshadow: 그림자 패스도 동일한 clip() 조건 적용 (투명 구역은 그림자도 없음)
        #pragma surface surf Lambert addshadow
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4    _Color;

        // ViewRenderer에서 Shader.SetGlobalVector/Float 로 매 프레임 갱신됩니다.
        float3 _PlayerPos;
        float  _XRayRadius;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            // 스포트라이트: 원점=카메라, 방향=카메라→플레이어
            float3 camPos     = _WorldSpaceCameraPos;
            float3 toCam2Player = _PlayerPos - camPos;
            float  playerDist   = length(toCam2Player);
            float3 spotDir      = toCam2Player / max(playerDist, 0.001);

            // 카메라→픽셀 벡터를 스포트라이트 축에 투영
            float3 toPixel = IN.worldPos - camPos;
            float  pixLen  = length(toPixel);
            float  proj    = dot(toPixel, spotDir);

            // 카메라-플레이어 구간 내 (0 < proj < playerDist) 이면서 원뿔 각도 이내인 픽셀 제거
            if (proj > 0.0 && proj < playerDist)
            {
                // cos(픽셀 방향, 스포트라이트 방향) = proj / pixLen
                float cosAngle     = proj / max(pixLen, 0.001);
                // cos(반각): 플레이어 거리에서 반지름이 _XRayRadius인 원뿔
                float cosHalfAngle = playerDist / sqrt(playerDist * playerDist + _XRayRadius * _XRayRadius);

                // 원뿔 내부면 discard (clip(-1))
                clip(cosAngle >= cosHalfAngle ? -1 : 1);
            }

            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha  = c.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
