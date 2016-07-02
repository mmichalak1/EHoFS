Texture2D FinalMap, TransparentMap, SourceDepth, TransparentDepth;

bool isBlack(float4 color)
{
	return (color.r == 0.0f && color.g == 0.0f && color.b == 0.0f);
}

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
	output.Position=Position;
	output.TexCoord=TexCoord;

	return output;
}

float4 PSFunc(PSInput input) : COLOR0
{

	float4 finalColor = FinalMap.Sample(Sampler, input.TexCoord);
	float4 transparentColor = TransparentMap.Sample(Sampler, input.TexCoord);
	float4 sourceValue = SourceDepth.Sample(Sampler, input.TexCoord);
	float4 transparentValue = TransparentDepth.Sample(Sampler, input.TexCoord);

	if (transparentColor.a > 0.0f)
	{
		if(isBlack(sourceValue))
		{
			return transparentColor;
		}
			if(sourceValue.r < transparentValue.r)
				return finalColor;
			else
				return transparentColor;
	}
	else
		return finalColor;
	
}

technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_5_0 VSFunc();
		PixelShader = compile ps_5_0 PSFunc();
	}
}