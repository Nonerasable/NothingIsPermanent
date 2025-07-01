Shader "Custom/DissolveShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _BurntTex ("Burnt Texture", 2D) = "black" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}

        _HitPoint ("Hit Point", Vector) = (0,0,0,0)
        _Radius ("Dissolve Radius", Float) = 0
        _EdgeWidth ("Edge Width", Float) = 0.1
        _EdgeColor ("Edge Color", Color) = (0,1,0,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        // --------------------------
        // Pass: Main Lighting Pass
        // --------------------------
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

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
                float3 viewDirWS   : TEXCOORD3;
            };

            sampler2D _MainTex;
            sampler2D _BurntTex;
            sampler2D _NoiseTex;

            float3 _HitPoint;
            float _Radius;
            float _EdgeWidth;
            float4 _EdgeColor;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceViewDir(OUT.worldPos);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 worldPos = IN.worldPos;
                float3 normalWS = normalize(IN.normalWS);
                float3 viewDirWS = normalize(IN.viewDirWS);

                float dist = distance(worldPos, _HitPoint);
                float noise = tex2D(_NoiseTex, IN.uv).r;

                float dissolveValue = saturate((_Radius - dist + noise * _EdgeWidth) / _EdgeWidth);
                float edgeLine = smoothstep(0.45, 0.5, dissolveValue) - smoothstep(0.5, 0.55, dissolveValue);
                float3 edgeGlow = _EdgeColor.rgb * edgeLine * 2.0;

                float3 mainColor = tex2D(_MainTex, IN.uv).rgb;
                float3 burntColor = tex2D(_BurntTex, IN.uv).rgb;
                float3 baseColor = lerp(mainColor, burntColor, dissolveValue);

                // Простое ламбертовое освещение от основного Directional Light
                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);
                float NdotL = saturate(dot(normalWS, lightDir));
                float3 lighting = baseColor * mainLight.color.rgb * NdotL;

                return float4(lighting + edgeGlow, 1.0);
            }

            ENDHLSL
        }

        // --------------------------
        // Pass: ShadowCaster (чтобы объект отбрасывал тень)
        // --------------------------
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                float4 shadowPos = TransformWorldToHClip(positionWS + normalWS * 0.005); // small bias
                OUT.positionHCS = shadowPos;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                return 0; // Shadow pass doesn't output color
            }

            ENDHLSL
        }
    }
}
