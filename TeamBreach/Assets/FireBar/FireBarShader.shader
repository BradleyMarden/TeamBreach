// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/FireBarShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Fuel("Fuel Amount", Range(0,1)) = 1
        _ScrollYSpeed("Y", Range(0,10)) = 3
        _ScrollXSpeed("X", Range(0,10)) = 3
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue"="Transparent"}

            Pass
            {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uvTwo : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Fuel;
            float _ScrollYSpeed;
            float _ScrollXSpeed;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // Gets the xy position of the vertex in worldspace.
                float2 worldXY = mul(unity_ObjectToWorld, v.vertex).xy;
                // Use the worldspace coords instead of the mesh's UVs.
                o.uv = TRANSFORM_TEX(worldXY, _MainTex);
                o.uvTwo = v.uv;
                return o;
            }


            float InverLerp(float a, float b, float v)
            {
            
                return (v - a) / (b - a);

            }
            fixed4 frag(v2f i) : SV_Target
            {
                float fireBarMask = _Fuel > i.uvTwo.x;
            //Scrolls Texture
                fixed yScrollValue = _ScrollYSpeed * _Time;
                fixed xScrollValue = _ScrollXSpeed * _Time;
                float2 uv = float2((xScrollValue - i.uv.x),  (yScrollValue- i.uv.y));
                float3 scrolled = tex2D(_MainTex, uv);
                
                //float3 col = tex2D(_MainTex, scrolled);
                //return col;

                float4 col = float4(scrolled * fireBarMask, fireBarMask);
                return col;
            }

            ENDCG
        }
    }
}
