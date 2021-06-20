Shader "Trak/TrailShader" {
	Properties{
		// _Color("Main Color", Color) = (.2745, .1725, .0157, 1)
		_Color("Main Color", Color) = (.1294, .1294, .1294, 1)
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
		Category
	{
		Lighting Off
		ZWrite On
		ZTest Always
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