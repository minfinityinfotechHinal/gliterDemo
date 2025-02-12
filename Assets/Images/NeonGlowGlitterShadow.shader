Shader "Custom/NeonGlowGlitterShadow"
{
    Properties
    {
        _MainTex ("Brush Texture", 2D) = "white" {} // Stroke texture
        _Color ("Neon Color", Color) = (1,1,1,1) // Base neon color
        _GlowIntensity ("Glow Intensity", Range(1, 10)) = 5 // Glow brightness
        _ShadowStrength ("Shadow Strength", Range(0, 1)) = 0.3 // Dark outline strength
        _GlitterStrength ("Glitter Strength", Range(0, 1)) = 0.5 // Glitter power
        _NoiseScale ("Noise Scale", Range(10, 100)) = 50 // Density of glitter
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha One // Additive blending for glow
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _GlowIntensity;
            float _ShadowStrength;
            float _GlitterStrength;
            float _NoiseScale;

            // Random noise for glitter effect
            float randomNoise(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv); // Brush texture alpha
                float noise = randomNoise(i.uv * _NoiseScale); // Glitter noise

                // Glitter effect by mixing random noise
                float glitterEffect = smoothstep(0.3, 1.0, noise) * _GlitterStrength;

                // Dark inner shadow effect
                float shadow = smoothstep(0.1, 0.5, texColor.a) * _ShadowStrength;

                // Combine glow, glitter, and shadow
                float3 finalColor = (_Color.rgb * (1.0 + glitterEffect) * _GlowIntensity) - shadow;
                float finalAlpha = texColor.a * (_GlowIntensity * 0.5);

                return fixed4(finalColor, finalAlpha);
            }
            ENDCG
        }
    }
}
