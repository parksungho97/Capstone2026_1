Shader "jjh/ObstacleMask"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-1" }
        
        Pass
        {
            ZWrite On
            ZTest LEqual
            ColorMask R

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
        return fixed4(1.5, 0, 0, 1);
    }
            ENDCG
        }
    }
}