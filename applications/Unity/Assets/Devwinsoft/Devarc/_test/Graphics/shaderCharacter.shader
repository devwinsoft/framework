Shader "Devwinsoft/TestCharacter"
{
	Properties
	{
		_Color0 ("Color 0", Color) = (1.00, 1.00, 1.00, 1.0)
	}
	 
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
	 
	 	Pass
	 	{
			Color(1, 1, 1, 1)
	 	}

		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf BlinnPhong

		uniform fixed4 _Color0;

		struct Input
		{
			float4 color : COLOR;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = fixed3(IN.color.r * _Color0.r, IN.color.g * _Color0.g, IN.color.b * _Color0.b);
		}
		ENDCG
	}
	
	FallBack "Diffuse"
}
