float4x4 World, View, Projection;

float4 CameraPosition;

TextureCube SkyboxTexture;

bool isReflecting;

SamplerState SkyboxSampler
{
   magfilter = LINEAR; 
   minfilter = LINEAR; 
   mipfilter = LINEAR; 
   AddressU = Mirror; 
   AddressV = Mirror; 
};

SamplerState ColorMapSampler
{
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct PixelShaderInput
{
	float4 Position : SV_POSITION0;
	float3 TexCoord : POSITION1;
};

PixelShaderInput VSFunc(float4 Position : SV_POSITION0, float2 TexCoord :TEXCOORD0, float3 Normal : NORMAL)
{
	PixelShaderInput output = (PixelShaderInput)0;
	output.Position=mul(Position, World);
	output.Position=mul(output.Position, View);
	output.Position=mul(output.Position, Projection);
	if(!isReflecting)
	{	
		
		output.TexCoord= mul(Position, World) - CameraPosition;
	}
	else
	{
		float3 bigNormal = ((Normal) * 2) -1;
		float3 normal=normalize(bigNormal);
		float4 WorldPosition = mul(Position, World);
		float3 eyeVector = WorldPosition-CameraPosition;
    		output.TexCoord = reflect(eyeVector,normal);
	}
	return output;
	
}

float4 PSFunc(PixelShaderInput input) : COLOR0
{
	return SkyboxTexture.Sample(SkyboxSampler, input.TexCoord);
}

technique t1
{
	pass p0 
	{
		VertexShader = compile vs_5_0 VSFunc();
		PixelShader = compile ps_5_0 PSFunc();
	}
}