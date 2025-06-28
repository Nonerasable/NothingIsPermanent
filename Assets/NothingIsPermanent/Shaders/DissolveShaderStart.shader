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

        Pass
        {
            Name "ForwardLit"
            Tags {"LightMode"="UniversalForward"}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 worldPos    : TEXCOORD1;
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
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 worldPos = IN.worldPos;

                float dist = distance(worldPos, _HitPoint);

                float noise = tex2D(_NoiseTex, IN.uv).r;

                float dissolveValue = saturate((_Radius - dist + noise * _EdgeWidth) / _EdgeWidth);

                // Край — яркий "ободок"
                float edgeLine = smoothstep(0.45, 0.5, dissolveValue) - smoothstep(0.5, 0.55, dissolveValue);
                float3 edgeGlow = _EdgeColor.rgb * edgeLine * 2.0;

                float3 mainColor = tex2D(_MainTex, IN.uv).rgb;
                float3 burntColor = tex2D(_BurntTex, IN.uv).rgb;
                float3 finalColor = lerp(mainColor, burntColor, dissolveValue);

                finalColor += edgeGlow;

                return float4(finalColor, 1.0);
            }

            ENDHLSL
        }
    }
}
