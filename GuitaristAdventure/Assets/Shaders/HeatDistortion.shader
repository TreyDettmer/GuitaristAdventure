﻿Shader "Unlit/HeatDistortion"
{
    Properties
    {
		_Strength("Distort Strength",Float) = 1.0
		_Noise("Noise Texture",2D) = "white" {}
		_Speed("Speed",Float) = 1.0
		_StrengthFilter("Strength Filter",2D) = "white" {}
		_Tint("Tint",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags {
			"Queue"="Transparent" 
			"DisableBatching" = "True"
		}
        LOD 100
		GrabPass {
			"_BackgroundTexture"
		}

        Pass
        {

			ZTest Always
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct vertexInput
            {
                float4 pos : POSITION;
                float3 texCoord : TEXCOORD0;
            };

            struct vertexOutput
            {

                float4 pos : SV_POSITION;
				float4 grabPos : TEXCOORD0;
            };

			sampler2D _BackgroundTexture;
			sampler2D _Noise;
			sampler2D _StrengthFilter;
			float _Strength;
			float _Speed;
			float4 _Tint;

            vertexOutput vert(vertexInput input)
            {
                vertexOutput output;
                output.pos = UnityObjectToClipPos(input.pos);
				float noise = tex2Dlod(_Noise, float4(input.texCoord, 0)).rgb;
				float filter = tex2Dlod(_StrengthFilter, float4(input.texCoord, 0)).rgb;
				output.grabPos = ComputeGrabScreenPos(output.pos);
				output.grabPos.x += cos(noise * _Time.x * _Speed) * filter * _Strength;
				output.grabPos.y += sin(noise * _Time.x * _Speed) * filter * _Strength;
                return output;
            }

			fixed4 frag(vertexOutput input) : COLOR
			{

				return tex2Dproj(_BackgroundTexture,input.grabPos) * _Tint;
            }
            ENDCG
        }
    }
}
