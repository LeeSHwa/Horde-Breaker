Shader "Custom/TileMapOverlayShader3Textures"
{
    // Thanks jess::codes for this shader
     Properties
        {
            _OverlayTexR("Overlay Texture R-Channel", 2D) = "white" {} // Texture to overlay
            _OverlayTexG("Overlay Texture G-Channel", 2D) = "white" {} // Texture to overlay
            _OverlayTexB("Overlay Texture B-Channel", 2D) = "white" {} // Texture to overlay
            _ScaleR("Scale Texture R", Float) = 0.006944444 // Scale factor
            _ScaleG("Scale Texture G", Float) = 0.006944444 // Scale factor
            _ScaleB("Scale Texture B", Float) = 0.006944444 // Scale factor
        }
        SubShader
        {

            Tags { "RenderType"="Transparent" "Queue"="Transparent" }
            LOD 200
            LOD 200
        
            Pass
            {
                Blend SrcAlpha OneMinusSrcAlpha
                Cull Off
                ZWrite Off

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile _ PIXELSNAP_ON

                #include "UnityCG.cginc"

                sampler2D _MainTex; 
                sampler2D _OverlayTexR; 
                sampler2D _OverlayTexG; 
                sampler2D _OverlayTexB; 
                float _ScaleR; 
                float _ScaleG; 
                float _ScaleB; 

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float2 worldPos : TEXCOORD1;
                    float4 vertex : SV_POSITION;
                };

                v2f vert(appdata_t v)
                {
                    v2f o;

                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #ifdef PIXELSNAP_ON
                        o.vertex = UnityPixelSnap(o.vertex);
                    #endif
                
                    float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                    o.worldPos = worldPos.xy;
                
                    o.uv = v.texcoord;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 color = tex2D(_MainTex, i.uv);
                
                    float mixAmountR = floor(color.r+0.1);
                    float mixAmountG = floor(color.g+0.1);
                    float mixAmountB = floor(color.b+0.1);

                    float2 overlayUVR = i.worldPos * _ScaleR;
                    float2 overlayUVG = i.worldPos * _ScaleG;
                    float2 overlayUVB = i.worldPos * _ScaleB;
                    fixed4 overlayColorR = tex2D(_OverlayTexR, overlayUVR);
                    fixed4 overlayColorG = tex2D(_OverlayTexG, overlayUVG);
                    fixed4 overlayColorB = tex2D(_OverlayTexB, overlayUVB);
                    return lerp(lerp(lerp(color, overlayColorR, mixAmountR), overlayColorB, mixAmountB), overlayColorG, mixAmountG);
                }
                ENDCG
            }
        }
}