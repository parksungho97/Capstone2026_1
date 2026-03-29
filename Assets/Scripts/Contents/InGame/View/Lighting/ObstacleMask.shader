Shader "Hidden/ObstacleMask"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-1" }
        
        Pass
        {
ZWrite Off
ZTest LEqual    // depth 복사본으로 판단
ColorMask R
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace // 항상 스텐실 값을 1로 교체
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 pos : SV_POSITION; };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
    {
        // 통과된 픽셀(보이는 장애물 영역)에 1(흰색)을 채움
        return fixed4(1, 0, 0, 1);
    }
            ENDCG
        }
    }
}