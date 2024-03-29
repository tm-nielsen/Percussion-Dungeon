﻿Shader "Custom/HorizontalGradient"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_LeftCol("Left Colour", Color) = (1, 1, 1, 1)
		_RightCol("Right Colour", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
		Tags{
			"Queue" = "Transparent"
		}
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
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
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			fixed4 _LeftCol;
			fixed3 _RightCol;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
			float x = 1 - i.uv.x;
				col.rgb *= lerp(_RightCol,_LeftCol, x * x);
                //col.rgb = 1 - col.rgb;
                return col;
            }
            ENDCG
        }
    }
}
