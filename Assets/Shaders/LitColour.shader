Shader "Custom/LitColour" {
    Properties {
        _Color ("Colour", Color) =  (1, 1, 1, 1)
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        
CGPROGRAM
#pragma surface surf Lambert

float4 _Color;

struct Input {
	float4 colour : COLOR;
};


void surf(Input i, inout SurfaceOutput o) {
    o.Albedo = _Color.rgb;
    o.Alpha = _Color.a;
    return;
}
ENDCG

    } 
    FallBack "Diffuse"
}
