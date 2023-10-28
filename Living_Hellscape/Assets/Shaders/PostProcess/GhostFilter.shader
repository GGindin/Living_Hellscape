Shader "Living_Hellscape/PostProcess/GhostFilter"
{
//https://www.patreon.com/posts/shader-tutorial-68425011?l=it
//the above link will get us color replacement so we can have certain colors come through in the greyscale
//like red for enemies, or blue for friendly

    Properties
    {
        [HideInInspector]_MainTex ("Main Tex", 2D) = "white" {}
        _GhostAmount ("Ghost Amount", Range(0, 1.0)) = 0.0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _GhostAmount;


            fixed4 frag (v2f i) : SV_Target
            {
                //get the screenPixel
                float4 screenColor = tex2D(_MainTex, i.uv);

                float4 col = screenColor;

                //rough gray scale https://forum.unity.com/threads/how-to-make-standard-shader-to-grayscale.767255/
                col.rgb = (0.299 *col.r) + (0.587 * col.g) + (0.114 * col.b);


                //return the lerp
                return lerp(screenColor, col, _GhostAmount);
            }
            ENDCG
        }
    }
}
