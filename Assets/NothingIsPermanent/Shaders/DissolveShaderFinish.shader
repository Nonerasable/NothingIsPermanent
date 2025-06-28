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
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _NoiseTex;

            float4 _EdgeColor;
            float _EdgeWidth;
            float _Progress;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float noise = tex2D(_NoiseTex, i.uv).r;

                float dissolveThreshold = _Progress;
                float edgeRange = _EdgeWidth;

                // основа для сравнения
                float d = saturate((dissolveThreshold - noise) / edgeRange);

                // отбрасываем пиксели, полностью исчезнувшие
                if (noise < dissolveThreshold - edgeRange)
                    discard;

                // подсвечиваем края
                float edge = smoothstep(0.0, 1.0, d) * (1 - step(1.0, d));
                float3 edgeGlow = _EdgeColor.rgb * edge;

                float3 baseColor = tex2D(_MainTex, i.uv).rgb;
                float3 finalColor = baseColor + edgeGlow;

                return float4(finalColor, 1.0);
            }
            ENDCG
        }
    }
}