Shader "jjh/FOVStencilMask"
{
    SubShader
    {
        // Transparent-1(2999): 오버레이(3100)보다 먼저 렌더링해 스텐실 선점
        Tags { "Queue" = "Transparent-1" "RenderType" = "Transparent" }

        Pass
        {
            Stencil
            {
                Ref  1
                Comp Always
                Pass Replace    // FOV 영역 픽셀에 스텐실 = 1 기록
            }

            ColorMask 0         // 색 버퍼 쓰기 금지 (화면에 안 보임)
            ZWrite    Off
            ZTest     Always    // 깊이 무관하게 항상 스텐실 기록
            Cull      Off

            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; };
            struct v2f     { float4 pos    : SV_POSITION; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}
