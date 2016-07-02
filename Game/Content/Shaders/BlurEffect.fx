// Pixel shader applies a one dimensional gaussian blur filter.
// This is used twice by the bloom postprocess, first to
// blur horizontally, and then again to blur vertically.

texture2D shaderTexture;
SamplerState Sampler
{
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	Mipfilter = LINEAR;
	AddressU = CLAMP;
	AddressV = CLAMP;
};

#define SAMPLE_COUNT 15

float2 SampleOffsets[SAMPLE_COUNT];
float SampleWeights[SAMPLE_COUNT];

struct PSInput
{
	float4 Position : SV_POSITION0;
	float2 Texture : TEXCOORD0;
};

PSInput VertexShaderF(float4 Position : SV_POSITION0, float2 text : TEXCOORD0)
{
	PSInput output;
	output.Position = Position;
	output.Texture = text;
	return output;
}

float4 PixelShaderF(PSInput input) : COLOR0
{
	float4 c = 0;

	// Combine a number of weighted image filter taps.
	for (int i = 0; i < SAMPLE_COUNT; i++)
	{
		c += shaderTexture.Sample(Sampler, input.Texture + SampleOffsets[i]) * SampleWeights[i];
	}
	c.a = shaderTexture.Sample(Sampler, input.Texture + SampleOffsets[7]).a;

	return c;
}


technique GaussianBlur
{
	pass Pass1
	{
		VertexShader = compile vs_5_0 VertexShaderF();
		PixelShader = compile ps_5_0 PixelShaderF();
	}
}
