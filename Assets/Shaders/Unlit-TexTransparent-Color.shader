Shader "Unlit/Color Tex Transparent" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200
		
		Blend SrcAlpha OneMinusSrcAlpha
	Pass {
		
		CGPROGRAM
		#pragma vertex vert alpha
		#pragma fragment frag alpha
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		half4 _Color;
		
		struct v2f
		{
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};
		
		float4 _MainTex_ST;
		
		v2f vert (appdata_base v)
		{
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
			return o;
		}
		
		half4 frag(v2f i) : COLOR
		{
			return tex2D(_MainTex,i.uv) * _Color;
		}
		ENDCG
	} 
	}
	FallBack "Diffuse"
}
