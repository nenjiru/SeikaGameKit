Shader "SeikaGameKit/ScaleGrid"
{
    Properties
    {
        _MainTex ("Grid Texture", 2D) = "white" {}
        _Color ("Grid Color", Color) = (1,1,1,1)
        _BackgroundColor ("Background Color", Color) = (0.2,0.2,0.2,0.5)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #if defined(HDRP_RENDERER)
                #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #elif defined(UNIVERSAL_RENDERER)
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #else // Built-in
                #include "UnityCG.cginc"
            #endif

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _BackgroundColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 localXZ : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.localXZ = v.vertex.xz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 worldX = float3(1.0, 0.0, 0.0);
                float worldScale = length(mul((float3x3)unity_ObjectToWorld, worldX));
                float2 scaledUV = i.worldPos.xz;
                scaledUV /= 5.0;
                fixed4 gridColor = tex2D(_MainTex, scaledUV) * _Color;
                return lerp(_BackgroundColor, gridColor, gridColor.a);
            }

            ENDHLSL
        }
    }
}