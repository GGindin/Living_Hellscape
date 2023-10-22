Shader "Living_Hellscape/PostProcess/Vignette"
{
    Properties
    {
        [HideInInspector]_MainTex ("Main Tex", 2D) = "white" {}
        _Color ("Color", Color) = (0, 0, 0, 1)
        _Amount ("Amount", Range(0, 1.0)) = 0.0
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
                float4 screenPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            sampler2D _MainTex;
            float _Amount;
            float4 _Color;

            float2 _FocusPos;

            float InverseLerp(float lower, float upper, float value){
                return (value - lower) / (upper - lower);
            }

            float RemapFloat(float In, float2 InMinMax, float2 OutMinMax)
            {
                return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }

            float Unity_SphereMask_float4(float2 Coords, float2 Center, float Radius, float Hardness)
            {
                return 1 - saturate((distance(Coords, Center) - Radius) / (1 - Hardness));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //get the screenPixel
                float4 screenColor = tex2D(_MainTex, i.uv);

                //get aspect ratio
                float aspect = _ScreenParams.x / _ScreenParams.y;

                //get aspect corrected uv
                float2 texCoord = i.screenPos.xy / i.screenPos.w;
                texCoord = texCoord * 2 - 1;
                texCoord.x = texCoord.x * aspect;
                texCoord = texCoord * .5 + .5;

                //get aspect corrected focusPoint
                float2 focus = _FocusPos;
                focus = focus * 2 - 1;
                focus.x = focus.x * aspect;
                focus = focus * .5 + .5;

                //calculate distance
                float dist = distance(focus, texCoord);

                //get max distance to edge
                float maxDistToEdgeHor = max(1.0 - focus.x, focus.x);
                float maxDistToEdgeVert = max(1.0 - focus.y, focus.y);

                //need to adjust for the aspect ratio
                float cornerLength = length(float2(maxDistToEdgeHor, maxDistToEdgeVert)) * aspect;

                //remap to be for the new corner distance
                float amount = RemapFloat(1 - _Amount, float2(0, 1), float2(0, cornerLength));

                //sphere mask with hard edge
                float mask = Unity_SphereMask_float4(texCoord, focus, amount, .999);

                //mask sure if small value it drops to zero
                mask *= step(.01, mask);

                //return the lerp
                return lerp(_Color, screenColor, mask);
            }
            ENDCG
        }
    }
}
