//direction of the light
float3 lightDirection;
//color of the light 
float3 Color; 
//position of the camera, for specular light
float3 cameraPosition; 
//this is used to compute the world-position
float4x4 InvertViewProjection; 

float4 AmbientColor = float4(1, 0, 0, 0);
float AmbientIntensity = 0.5;

// diffuse color, and specularIntensity in the alpha channel
texture2D ColorMap; 
// normals, and specularPower in the alpha channel
texture2D NormalMap;
//depth
texture2D DepthMap;

SamplerState colorSampler
{
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};
SamplerState depthSampler
{
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};
SamplerState normalSampler
{
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};


struct VertexShaderInput
{
    float3 Position : SV_POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION0;
    float2 TexCoord : TEXCOORD0;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = float4(input.Position,1);
    //align texture coordinates
    output.TexCoord = input.TexCoord;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : SV_Target
{
    //get normal data from the normalMap
    float4 normalData = NormalMap.Sample(normalSampler,input.TexCoord);
    //tranform normal back into [-1,1] range
    float3 normal = 2.0f * normalData.xyz - 1.0f;
    //get specular power, and get it into [0,255] range]
    float specularPower = normalData.a * 255;
    //get specular intensity from the colorMap
    float specularIntensity = ColorMap.Sample(colorSampler, input.TexCoord).a;
    
    //read depth
    float depthVal = DepthMap.Sample(depthSampler,input.TexCoord).r;

    //compute screen-space position
    float4 position;
    position.x = input.TexCoord.x * 2.0f - 1.0f;
    position.y = -(input.TexCoord.x * 2.0f - 1.0f);
    position.z = depthVal;
    position.w = 1.0f;
    //transform to world space
    position = mul(position, InvertViewProjection);
    position /= position.w;
    
    //surface-to-light vector
    float3 lightVector = -normalize(lightDirection);

    //compute diffuse light
    float NdL = max(0,dot(normal,lightVector));
    float3 diffuseLight = NdL * Color.rgb;

    //reflexion vector
    float3 reflectionVector = normalize(reflect(-lightVector, normal));
    //camera-to-surface vector
    float3 directionToCamera = normalize(cameraPosition - position);
    //compute specular light
    float specularLight = specularIntensity * pow( saturate(dot(reflectionVector, directionToCamera)), specularPower);

    //output the two lights
    return float4(saturate(diffuseLight.rgb + AmbientColor * AmbientIntensity), specularLight) ;
}

technique Technique0
{
    pass Pass0
    {
        VertexShader = compile vs_5_0 VertexShaderFunction();
        PixelShader = compile ps_5_0 PixelShaderFunction();
    }
}