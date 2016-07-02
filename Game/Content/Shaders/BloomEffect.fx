texture2D GlowMap;
texture2D LightMap;

SamplerState ColorMapSampler
{
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

float BaseIntensity = 1.0f;
float BloomSaturation = 2.0f;
float BaseSaturation = 1.0f;

float4 AdjustSaturation(float4 color, float saturation)
{
	// The constants 0.3, 0.59, and 0.11 are chosen because the
	// human eye is more sensitive to green light, and less to blue.
	float grey = dot(color, float3(0.3, 0.59, 0.11));

	return lerp(grey, color, saturation);
}

struct PSInput
{
	float4 Position : SV_POSITION0;
	float2 Texture : TEXCOORD0;
};

PSInput VSFunc(float4 Position : SV_POSITION0, float2 Texture : TEXCOORD0)
{
	PSInput output;
	output.Position = Position;
	output.Texture = Texture;
	return output;
}


float4 PSFunc(PSInput input) : COLOR0
{
	// Look up the bloom and original base image colors.
	float4 bloom = GlowMap.Sample(ColorMapSampler, input.Texture);
	float4 base = LightMap.Sample(ColorMapSampler, input.Texture);

	// Adjust color saturation and intensity.
	bloom = AdjustSaturation(bloom, BloomSaturation) * bloom.a;
	base = AdjustSaturation(base, BaseSaturation) * BaseIntensity;

	// Darken down the base image in areas where there is a lot of bloom,
	// to prevent things looking excessively burned-out.
	base *= (1 - saturate(bloom));

	// Combine the two images.
	return base + bloom;
}

technique BloomCombine
{
	pass P0
	{
		VertexShader = compile vs_5_0 VSFunc();
		PixelShader = compile ps_5_0 PSFunc();
	}
}