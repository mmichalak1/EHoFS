float4x4 World, View, Projection;
float FarFrustrum;
Texture2D ShaderTexture;

SamplerState Sampler
{
   magfilter = LINEAR; 
   minfilter = LINEAR; 
   mipfilter = LINEAR; 
   AddressU = Wrap; 
   AddressV = Wrap; 
};

struct PSInput
{
	float4 Position : SV_POSITION0;
	float2 TexCoord : TEXCOORD0;	
	float Depth : TEXCOORD1;
};

struct PSOutput
{
	half4 Color : COLOR0;
	float4 Depth : COLOR1;
};

PSInput VSFunc(float4 Position : SV_POSITION0, float2 TexCoord : TEXCOORD0)
{
	PSInput output = (PSInput)0;

	output.Position = mul(Position, World);
	output.Position = mul(output.Position, View);
	output.Position = mul(output.Position, Projection);
	float4 pos = mul(Position, World);
	pos = mul(pos, View);
	output.Depth = pos.z;
	output.TexCoord = TexCoord;
	return output;
}

PSOutput PSFunc (PSInput input)
{
	PSOutput output = (PSOutput)0;
	float depth = -input.Depth/FarFrustrum;
	output.Depth = float4(depth,1.0f,1.0f,1.0f);
	output.Color = ShaderTexture.Sample(Sampler, input.TexCoord);

	return output;
}

technique tech0
{
	pass p0
	{
		VertexShader = compile vs_5_0 VSFunc();
		PixelShader = compile ps_5_0 PSFunc();
	}
}