Shader "Devwinsoft/TestTerrain"
{
	Properties
	{
		_Color0 ("Color 0", Color) = (1.0, 1.0, 1.0, 1.0)
		_Color1 ("Color 1", Color) = (0.0, 0.0, 0.0, 1.0)
	 	_MainTex ("", 2D) = "white" {}
	}
	 
	SubShader
	{
		Tags { "Queue" = "Geometry" "RenderType" = "Opaque" }
		ZTest Always
		Fog { Mode Off }
	 
	 	Pass
	 	{
	  		CGPROGRAM
	  		#pragma vertex vert_img
	  		#pragma fragment frag
	  		#include "UnityCG.cginc"

			uniform fixed4 _Color0;
			uniform fixed4 _Color1;
	  		uniform sampler2D _MainTex;
	    
	  		fixed4 frag (v2f_img i) : COLOR
	  		{
	   			fixed3 original = tex2D (_MainTex, i.uv).rgb;
	   			fixed dist0 = distance (original, _Color0.rgb);
	   			fixed4 col = fixed4 (0,0,0,0);
				if (dist0 < 0.5)
				{
					col = _Color0;
				}
				else
				{
					col = _Color1;
				}
				return col;
	  		}
	  		
	  		ENDCG
	 	}
	}
	
	FallBack "Diffuse"
}
