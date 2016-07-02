float4x4 World, View, Projection;
int frameX, frameY;
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
	output.Position = mul(Position, World);
	output.Position = mul(output.Position, View);
	output.Position = mul(output.Position, Projection);
	
	output.TexCoord.y = TexCoord.y/4 + 0.25f * frameY;
	output.TexCoord.x = TexCoord.x/4 + 0.25f * frameX;
	
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