Shader "Trak/TrackShader" {
	Properties{
		// _Color("Main Color", Color) = (.2745, .1725, .0157, 1)
		_Color("Main Color", Color) = (.0863, .0863, .0863, 1)
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
	Category
	{
		Lighting Off
		ZWrite On
		Cull Back
		SubShader
		{
			Pass
			{
				SetTexture[_MainTex]
				{
					constantColor[_Color]
					Combine texture * constant, texture * constant
				}
			}
		}
	}
}