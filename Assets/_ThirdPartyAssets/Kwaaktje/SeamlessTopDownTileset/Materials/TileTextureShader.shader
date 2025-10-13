Shader "Custom/TileMapOverlayShader"
{
    // Thanks jess::codes for this shader
     Properties
        {
            _OverlayTex("Overlay Texture", 2D) = "white" {}
            _Scale("Scale", Float) = 0.006944444 
        }
        SubShader
        {
            Tags { "RenderType"="Transparent" "Queue"="Transparent" }
            LOD 200
        
            Pass
            {
                Blend SrcAlpha OneMinusSrcAlpha
                Cull Off
                ZWrite Off

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                sampler2D _MainTex; 
                sampler2D _OverlayTex; 
                float _Scale; 

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
                
                    float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                    o.worldPos = worldPos.xy;
                
                    o.uv = v.texcoord;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 color = tex2D(_MainTex, i.uv);
                
                    float mixAmount = floor(color.r);

                    float2 overlayUV = i.worldPos * _Scale;
                    fixed4 overlayColor = tex2D(_OverlayTex, overlayUV);
                    fixed4 newColor = lerp(color, overlayColor, mixAmount);
                    newColor.a = color.a*newColor.a;
                    return newColor;
                }
                ENDCG
            }
        }
}