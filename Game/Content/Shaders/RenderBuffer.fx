float4x4 World;
float4x4 View;
float4x4 Projection;
float specularIntensity = 1.0f;
float specularPower = 1.0f;
float emissionPower = 1.0f;
float FarFrustrum;
bool isEmissive;
float4x3 Bones[72];

float4 EmissiveColorChanger;
float4 DiffuseColorChanger;

texture2D ColorMap;
texture2D EmissionMap;
texture2D SpecularMap;
texture2D NormalMap;

TextureCube CubeTexture;

float3 CameraPosition;

SamplerState Sampler
{
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

struct VertexShaderInput
{
	float4 Position : SV_POSITION0;
	float3 Normal : NORMAL0;
	float2 TexCoord : TEXCOORD0;
	float3 Binormal : BINORMAL0;
	float3 Tangent : TANGENT0;
};

struct VertexShaderSkeletonInput
{
	float4 Position : SV_POSITION0;
	float3 Normal   : NORMAL0;
	float2 TexCoord : TEXCOORD0;
	int4   Indices  : BLENDINDICES0;
	float4 Weights  : BLENDWEIGHT0;
	float3 Binormal : BINORMAL0;
	float3 Tangent  : TANGENT0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION0;
	float2 TexCoord : TEXCOORD0;
	float Depth : TEXCOORD1;
	float3x3 tangentToWorld : TEXCOORD2;
};


void Skin(inout VertexShaderSkeletonInput vin, uniform int boneCount)
{
	float4x3 skinning = 0;

	[unroll]
	for (int i = 0; i < boneCount; i++)
	{
		skinning += Bones[vin.Indices[i]] * vin.Weights[i];
	}

	vin.Position.xyz = mul(vin.Position, skinning);
	vin.Normal = mul(vin.Normal, (float3x3)skinning);
}

VertexShaderOutput VSSkinned(VertexShaderSkeletonInput input)
{
	VertexShaderOutput output;

	Skin(input, 4);

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(input.Position, View);
	float4 worldViewProj = mul(viewPosition, Projection);
	
	output.Position = worldViewProj;

	output.TexCoord = input.TexCoord;
	output.Depth = viewPosition.z;

	// calculate tangent space to world space matrix using the world space tangent,
	// binormal, and normal as basis vectors
	output.tangentToWorld[0] = mul(input.Tangent, World);
	output.tangentToWorld[1] = mul(input.Binormal, World);
	output.tangentToWorld[2] = mul(input.Normal, World);

	return output;
}


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.TexCoord = input.TexCoord;
	output.Depth = viewPosition.z;

	// calculate tangent space to world space matrix using the world space tangent,
	// binormal, and normal as basis vectors
	output.tangentToWorld[0] = mul(input.Tangent, World);
	output.tangentToWorld[1] = mul(input.Binormal, World);
	output.tangentToWorld[2] = mul(input.Normal, World);

	return output;
}

struct PixelShaderOutput
{
	half4 Color : COLOR0;
	half4 Normal : COLOR1;
	float4 Depth : COLOR2;
	half4 Emission : COLOR3;
	
};

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
	PixelShaderOutput output;
	output.Color = ColorMap.Sample(Sampler, input.TexCoord);
	output.Color += DiffuseColorChanger;
	output.Color = normalize(output.Color);
	float4 specularAttributes = SpecularMap.Sample(Sampler, input.TexCoord);
	//specular Intensity
	output.Color.a = specularAttributes.r * specularIntensity;

	// read the normal from the normal map
	float3 normalFromMap = NormalMap.Sample(Sampler, input.TexCoord);
	//tranform to [-1,1]
	normalFromMap = 2.0f * normalFromMap - 1.0f;
	//transform into world space
	normalFromMap = mul(normalFromMap, input.tangentToWorld);
	//normalize the result
	normalFromMap = normalize(normalFromMap);
	//output the normal, in [0,1] space
	output.Normal.rgb = 0.5f * (normalFromMap + 1.0f);

	//specular Power
	output.Normal.a = specularAttributes.a * specularPower;

	float depth = -input.Depth/FarFrustrum;
	output.Depth = float4(depth,1.0f,1.0f,1.0f);
	output.Emission = EmissionMap.Sample(Sampler, input.TexCoord);
	output.Emission *= EmissiveColorChanger;
	output.Emission.a = emissionPower;
	return output;
}

PixelShaderOutput PSSkinned(VertexShaderOutput input)
{
	PixelShaderOutput output;
	output.Color = ColorMap.Sample(Sampler, input.TexCoord);
	output.Color += DiffuseColorChanger;
	float4 specularAttributes = SpecularMap.Sample(Sampler, input.TexCoord);
	//specular Intensity
	output.Color.a = specularAttributes.r * specularIntensity;

	// read the normal from the normal map
	float3 normalFromMap = NormalMap.Sample(Sampler, input.TexCoord);
	//tranform to [-1,1]
	normalFromMap = 2.0f * normalFromMap - 1.0f;
	//transform into world space
	normalFromMap = mul(normalFromMap, input.tangentToWorld);
	//normalize the result
	normalFromMap = normalize(normalFromMap);
	//output the normal, in [0,1] space
	output.Normal.rgb = 0.5f * (normalFromMap + 1.0f);

	//specular Power
	output.Normal.a = specularAttributes.a * specularPower;

	float depth = -input.Depth/FarFrustrum;
	output.Depth = float4(depth, 1.0f, 1.0f, 1.0f);
	output.Emission = EmissionMap.Sample(Sampler, input.TexCoord);
	output.Emission *= EmissiveColorChanger;
	output.Emission.a = emissionPower;
	return output;
}

technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunction();
	}
}

technique Technique2
{
	pass Pass1
	{
		VertexShader = compile vs_5_0 VSSkinned();
		PixelShader = compile ps_5_0 PSSkinned();
	}
}


