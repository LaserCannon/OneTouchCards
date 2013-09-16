Shader "Text/Text Shader Plus" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent" }
		
		Lighting Off Cull Back Fog { Mode Off } 
		Blend SrcAlpha OneMinusSrcAlpha
	Pass {
		
		CGPROGRAM
		#pragma vertex vert alpha
		#pragma fragment frag alpha
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		
		struct appdata {
		    float4 vertex : POSITION;
    		float4 texcoord : TEXCOORD1;
		    fixed4 color : COLOR;
		};
		
		struct v2f
		{
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float4 color : COLOR0;
		};
		
		float4 _MainTex_ST;
		
		v2f vert (appdata v)
		{
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
			o.color = v.color;
			return o;
		}
		
		half4 frag(v2f i) : COLOR
		{
			return tex2D(_MainTex,i.uv).a * i.color;
		}
		ENDCG
	}
	} 
	FallBack "Diffuse"
}
