// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UnlitColour" {
    Properties {
        _Color ("Colour", Color) =  (1, 1, 1, 1)
    }

    SubShader {
        Tags { "RenderType"="Opaque" }

        Pass {
        
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

float4 _Color;

struct v2f {
    float4 pos : POSITION;
};


v2f vert(appdata_base v) {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    return o;
}


half4 frag(v2f i) : COLOR {
    return _Color;
}

ENDCG
        }   // end pass

    }   // end SubShader

    FallBack "Diffuse"
}
