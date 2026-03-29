Shader "jjh/FOVOverlay"
{
    Properties
    {
        _Color   ("Fog Color",  Color)       = (0, 0, 0, 1)
        _Opacity ("Opacity",    Range(0, 1)) = 0.7
    }

    SubShader
    {
        // Queue 태그는 CommandBuffer Blit 에서 무시됨 — 카메라 이벤트 타이밍으로 제어
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            Stencil
            {
                Ref  1
                Comp NotEqual   // 스텐실 != 1 (FOV 밖) 픽셀에만 색 적용
            }

            Blend  SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest  Always   // 깊이 무관, 모든 픽셀을 후처리로 덮음
            Cull   Off

            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float  _Opacity;

            // Blit 이 TEXCOORD0 을 넘겨주므로 struct 에 포함 (UV 는 실제로 사용 안 함)
            struct appdata
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(_Color.rgb, _Opacity);
            }
            ENDCG
        }
    }
}
