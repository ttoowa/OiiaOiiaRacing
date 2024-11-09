Shader "TTOOWA/TriPlanarURP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tiling ("Tiling", Float) = 1.0
        _TopTint ("Top Tint", Color) = (1,1,1,1)
        _SideTint ("Side Tint", Color) = (1,1,1,1)
        _BottomTint ("Bottom Tint", Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // Shadow keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 shadowCoord : TEXCOORD2;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _Tiling;
                float4 _TopTint;
                float4 _SideTint;
                float4 _BottomTint;
            CBUFFER_END
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                
                OUT.positionCS = positionInputs.positionCS;
                OUT.positionWS = positionInputs.positionWS;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.shadowCoord = GetShadowCoord(positionInputs);
                return OUT;
            }
            
            float4 frag(Varyings IN) : SV_Target
            {
                // Normalize the world normal
                float3 normalWS = normalize(IN.normalWS);
                
                // Calculate blend weights based on normal
                float3 blendWeights = abs(normalWS);
                blendWeights = blendWeights / (blendWeights.x + blendWeights.y + blendWeights.z);
                
                // Sample texture from three directions
                float3 positionWS = IN.positionWS * _Tiling;
                
                // XY plane (top)
                float4 topTexture = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, positionWS.xz) * _TopTint;
                
                // XZ plane (front/back)
                float4 sideTextureZ = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, positionWS.xy) * _SideTint;
                
                // YZ plane (left/right)
                float4 sideTextureX = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, positionWS.zy) * _SideTint;
                
                // Bottom
                float4 bottomTexture = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, positionWS.xz) * _BottomTint;
                
                // Blend textures based on normal
                float4 color = 0;
                
                if (normalWS.y > 0)
                {
                    color += topTexture * blendWeights.y;
                }
                else
                {
                    color += bottomTexture * blendWeights.y;
                }
                
                color += sideTextureX * blendWeights.x;
                color += sideTextureZ * blendWeights.z;
                
                // Get lighting information
                Light mainLight = GetMainLight(IN.shadowCoord);
                float shadow = mainLight.shadowAttenuation;
                float3 lighting = mainLight.color * (shadow * mainLight.distanceAttenuation);
                
                // Apply additional lights
                #ifdef _ADDITIONAL_LIGHTS
                uint additionalLightsCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0u; lightIndex < additionalLightsCount; ++lightIndex)
                {
                    Light light = GetAdditionalLight(lightIndex, IN.positionWS);
                    lighting += light.color * (light.distanceAttenuation * light.shadowAttenuation);
                }
                #endif
                
                // Apply lighting to color
                color.rgb *= lighting;
                
                return color;
            }
            ENDHLSL
        }
        
        // Shadow casting pass
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            float3 _LightDirection;

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
            };

            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output;
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
                
                #if UNITY_REVERSED_Z
                    positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #else
                    positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #endif
                
                output.positionCS = positionCS;
                return output;
            }

            half4 ShadowPassFragment(Varyings input) : SV_TARGET
            {
                return 0;
            }
            
            ENDHLSL
        }
    }
}