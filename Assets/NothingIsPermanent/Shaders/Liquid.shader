Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _Color("Color", Color) = (0, 0.5, 1, 0.7)
        _Fill("Fill", Range(0,1)) = 0.5
        _MinY("Min Y", Float) = 0.0
        _MaxY("Max Y", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 positionLS : TEXCOORD2;
                float3 normalWS : TEXCOORD1;
            };

            float4 _Color;
            float _Fill;
            float _MinY;
            float _MaxY;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionLS = IN.positionOS.xyz;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float fillY = lerp(_MinY, _MaxY, _Fill);
                if (IN.positionLS.y > fillY)
                    discard;

                Light mainLight = GetMainLight();
                float3 normal = normalize(IN.normalWS);
                float3 lightDir = normalize(mainLight.direction);
                float NdotL = saturate(dot(normal, lightDir)); // Свет направлен *вниз*

                float3 finalColor = _Color.rgb * (0.2 + NdotL * mainLight.color);
                return float4(finalColor, _Color.a);
            }
            ENDHLSL
        }
    }
}
