#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

    void MainLight_float (out float3 Direction, out float3 Color, out float DistanceAtten){
        #ifdef SHADERGRAPH_PREVIEW
            Direction = normalize(float3(1,1,-0.4));
            Color = float4(1,1,1,1);
            DistanceAtten = 1;
        #else
            Light mainLight = GetMainLight();
            Direction = mainLight.direction;
            Color = mainLight.color;
            DistanceAtten = mainLight.distanceAttenuation;
        #endif
    }

#ifndef SHADERGRAPH_PREVIEW
    #if VERSION_GREATER_EQUAL(9, 0)
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #if (SHADERPASS != SHADERPASS_FORWARD)
            #undef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
        #endif
    #else
        #ifndef SHADERPASS_FORWARD
            #undef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
        #endif
    #endif
#endif

    void MainLightShadows_float (float3 WorldPos, out float ShadowAtten){
        #ifdef SHADERGRAPH_PREVIEW
            ShadowAtten = 1;
        #else
            float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);

            #if VERSION_GREATER_EQUAL(10, 1)
                ShadowAtten = MainLightShadow(shadowCoord, WorldPos, float4(1,1,1,1), _MainLightOcclusionProbes);
            #else
                ShadowAtten = MainLightRealtimeShadow(shadowCoord);
            #endif
        #endif
    }

    void AmbientSampleSH_float (float3 WorldNormal, out float3 Ambient){
        #ifdef SHADERGRAPH_PREVIEW
            Ambient = float3(0.1, 0.1, 0.1);
        #else
            Ambient = SampleSH(WorldNormal);
        #endif
    }

    void MixFog_float (float3 Color, float Fog, out float3 Out){
        #ifdef SHADERGRAPH_PREVIEW
            Out = Color;
        #else
            Out = MixFog(Color, Fog);
        #endif
    }

    void AdditionalLights_float(float3 SpecColor, float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView,
                                out float3 Diffuse, out float3 Specular) {
       float3 diffuseColor = 0;
       float3 specularColor = 0;

    #ifndef SHADERGRAPH_PREVIEW
       Smoothness = exp2(10 * Smoothness + 1);
       WorldNormal = normalize(WorldNormal);
       WorldView = SafeNormalize(WorldView);
       int pixelLightCount = GetAdditionalLightsCount();
       for (int i = 0; i < pixelLightCount; ++i) {
            #if VERSION_GREATER_EQUAL(10, 1)
                Light light = GetAdditionalLight(i, WorldPosition, half4(1,1,1,1));
            #else
                Light light = GetAdditionalLight(i, WorldPosition);
            #endif

           float3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
           diffuseColor += LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
           specularColor += LightingSpecular(attenuatedLightColor, light.direction, WorldNormal, WorldView, float4(SpecColor, 0), Smoothness);
       }
    #endif

       Diffuse = diffuseColor;
       Specular = specularColor;
    }

#endif