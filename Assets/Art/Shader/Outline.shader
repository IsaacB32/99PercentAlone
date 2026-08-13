Shader "Custom/Outline"
{
    Properties
    {
       _MainTex ("Texture2D", 2D) = "white" {}
        _Threshold ("Threshold", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize; // .xy = 1/width, 1/height

            CBUFFER_START(UnityPerMaterial)
                float _Threshold;
            CBUFFER_END
            
            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            static const float kernel_x[9] = {
                -1,0,1,
                -2,0,2,
                -1,0,1
            };
            
            static const float kernel_y[9] = {
                -1,-2,-1,
                 0,0,0,
                 1,2,1
            };

            v2f vert(appdata IN)
            {
                v2f OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag(v2f IN) : SV_Target
            {
                float gx = 0.0;
                float gy = 0.0;

                 for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        float2 offset = float2(x, y) * _MainTex_TexelSize.xy;
                        float4 samp = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + offset);
                        float grayScale = (samp.r + samp.g + samp.b) / 3.0; // Convert to grayscale
                        gx += grayScale * kernel_x[(y + 1) * 3 + (x + 1)];
                        gy += grayScale * kernel_y[(y + 1) * 3 + (x + 1)];
                    }
                }

                float magnitude = sqrt(gx * gx + gy * gy);
                if (magnitude > _Threshold)
                {
                    return float4(1,1,1,1);
                }
                return float4(0,0,0,1); 
            }
            ENDHLSL
        }
    }
}
