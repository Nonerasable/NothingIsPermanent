Shader "Custom/GlobalDissolve"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _EdgeColor ("Edge Color", Color) = (0,1,0,1)
        _EdgeWidth ("Edge Width", Float) = 0.2
        _Progress ("Dissolve Progress", Range(0,1)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        // Main Pass with lighting
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _EdgeColor;
            float _EdgeWidth;
            float _Progress;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 worldPos    : TEXCOORD1;
                float3 normalWS    : TEXCOORD2;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float noise = tex2D(_NoiseTex, IN.uv).r;

                float d = saturate((_Progress - noise) / _EdgeWidth);
                if (noise < _Progress - _EdgeWidth)
                    discard;

                float edge = smoothstep(0.0, 1.0, d) * (1 - step(1.0, d));
                float3 edgeGlow = _EdgeColor.rgb * edge;

                float3 baseColor = tex2D(_MainTex, IN.uv).rgb;

                // Простейшее освещение от Directional Light
                Light mainLight = GetMainLight();
                float3 normalWS = normalize(IN.normalWS);
                float NdotL = saturate(dot(normalWS, normalize(mainLight.direction)));
                float3 litColor = baseColor * mainLight.color.rgb * NdotL;

                return float4(litColor + edgeGlow, 1.0);
            }

            ENDHLSL
        }

        // ShadowCaster pass (чтобы объект отбрасывал тень)
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragShadow
            #pragma multi_compile_shadowcaster
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            sampler2D _NoiseTex;
            float _Progress;
            float _EdgeWidth;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionHCS = TransformWorldToHClip(positionWS + normalWS * 0.005);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 fragShadow(Varyings IN) : SV_Target
            {
                float noise = tex2D(_NoiseTex, IN.uv).r;
                if (noise < _Progress - _EdgeWidth)
                    discard;
                return 0;
            }

            ENDHLSL
        }
    }
}