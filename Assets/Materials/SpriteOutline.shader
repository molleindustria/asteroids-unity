Shader "SpriteOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        [HDR]_OutlineColor ("Outline Color", Color) = (1, 0, 0, 1) // HDR color for emission
        _OutlineThickness ("Outline Thickness", Float) = 1.0
        _HDRIntensity ("Tint Emission", Float) = 1.0 // Intensity multiplier for SpriteRenderer tint
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _OutlineColor; // HDR color for the outline
            float _OutlineThickness;
            float _HDRIntensity; // Intensity multiplier
            fixed4 _Color; // Passed from SpriteRenderer
            float4 _MainTex_TexelSize; // Automatically provided by Unity: (1/width, 1/height, width, height)

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; // Vertex color from SpriteRenderer

            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; // Pass vertex color to fragment shader

            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color; // Pass vertex color to the fragment shader
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // If outline thickness is zero, skip the outline logic
                if (_OutlineThickness <= 0.0)
                {
                    // Render only the sprite with tint color
                    return tex2D(_MainTex, i.uv) * float4(i.color.rgb*_HDRIntensity, i.color.a);
                }

                float2 texelSize = _MainTex_TexelSize.xy * _OutlineThickness;
                float alpha = 0.0;

                // Check surrounding pixels for the outline
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        float2 offset = float2(x, y) * texelSize;
                        alpha += tex2D(_MainTex, i.uv - offset).a; // Internal outline: subtract offset
                    }
                }

                float currentAlpha = tex2D(_MainTex, i.uv).a;

                // Internal outline: Draw the outline only where current alpha exists
                if (alpha > 0.0 && currentAlpha > 0.0 && alpha < 8.0)
                {
                    return float4(_OutlineColor.rgb, 1.0); // HDR outline
                }

                // Tint the sprite with HDR color
                return tex2D(_MainTex, i.uv) * float4(i.color.rgb*_HDRIntensity, i.color.a);
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}
