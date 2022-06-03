Shader "Unlit/Transparent Colored" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
    }

    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        
        Stencil {
            Ref 7
            Comp NotEqual
        }

        ZWrite Off
        Lighting Off
        Fog { Mode Off }

        Blend SrcAlpha OneMinusSrcAlpha 

        Pass {
            Color [_Color]
        }
    }
}