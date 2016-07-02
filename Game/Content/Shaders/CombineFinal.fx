texture2D ColorMap;
texture2D LightMap;

SamplerState colorSampler
{
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};
SamplerState lightSampler
{
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};


struct VertexShaderInput
{
    float4 Position : SV_POSITION0;
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
    output.Position = input.Position;
    output.TexCoord = input.TexCoord;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 diffuseColor = ColorMap.Sample(colorSampler,input.TexCoord);
    float4 light = LightMap.Sample(lightSampler,input.TexCoord);
    float3 diffuseLight = light.rgb;
    float specularLight = light.a;
    return saturate(float4((diffuseColor * diffuseLight + specularLight * diffuseColor.a),0));
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_5_0 VertexShaderFunction();
        PixelShader = compile ps_5_0 PixelShaderFunction();
    }
}
