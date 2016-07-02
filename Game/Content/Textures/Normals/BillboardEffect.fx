float4x4 World, View, Projection;

float3 CamPos, RotDir;

Texture2D billboardTexture;

SamplerState Sampler
{
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

struct PSInput
{
	float4 Position : SV_POSITION0;
	float2 TexCoord : TEXCOORD0;
};

PSInput VSFunc(float4 Position : SV_POSITION0, float2 TexCoord : TEXCOORD0)
{
	PSInput output = (PSInput)0;
	float3 center =  mul(Position, World);
	float3 eye = center - CamPos;

	float3 upVector = normalize(RotDir);
	float3 sideVector = cross(eye,upVector);

	center += (TexCoord.x - 0.5f) * sideVector;
	center += (1.5f - TexCoord.y * 1.5f) * upVector;

	output.Position = float4(center, 1);
	output.Position = mul(output.Position, View);
	output.Position = mul(output.Position, Projection);

	output.TexCoord = TexCoord;

	return output;
}

float4 PSFunc(PSInput input) : COLOR0
{
	return billboardTexture.Sample(Sampler, input.TexCoord);
}

technique technique0
{
	pass pass0
	{
		VertexShader = compile vs_5_0 VSFunc();
		PixelShader = compile ps_5_0 PSFunc();
	}
}