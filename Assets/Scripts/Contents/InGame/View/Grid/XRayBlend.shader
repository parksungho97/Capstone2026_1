Shader "jjh/XRayBlend"
{
    Properties
    {
        _MainTex ("Source", 2D) = "white" {}
        _NoObstacleRT ("Background (No Obstacles)", 2D) = "white" {}
        _Alpha ("Transparency", Range(0, 1)) = 0.5 // 섞는 비율 (0: 원래 벽, 1: 완전 투명)
    }

    SubShader
    {
        // 후처리 단계이므로 깊이/컬링은 끄고, 스텐실만 체크합니다.
        ZTest Always ZWrite Off Cull Off

        Stencil
        {
            Ref 1
            Comp Equal
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _NoObstacleRT;
            float _Alpha;

            fixed4 frag(v2f_img i) : SV_Target
            {
                // 1. 벽이 있는 원래 화면 샘플링
                fixed4 original = tex2D(_MainTex, i.uv);
                
                // 2. 벽을 제외하고 미리 그려둔 배경 화면 샘플링
                fixed4 background = tex2D(_NoObstacleRT, i.uv);

                // 3. 두 화면을 _Alpha 비율로 섞음
                // _Alpha가 0.7이면 배경이 70%, 벽이 30% 보이게 됨
                return lerp(original, background, 0.8);
            }
            ENDCG
        }
    }
}