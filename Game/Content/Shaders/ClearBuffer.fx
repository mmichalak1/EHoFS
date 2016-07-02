struct VertexShaderInput
{
	float3 Position : SV_POSITION0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Position = float4(input.Position,1);
	return output;
}
struct PixelShaderOutput
{
	half4 Color : COLOR0;
	half4 Normal : COLOR1;
	half4 Depth : COLOR2;
	half4 Emission : COLOR3;
	half4 Output : COLOR4;
};

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
	PixelShaderOutput output;
	//black color
	output.Color = float4(0.5, 0.5, 0.5, 0);
	//when transforming 0.5f into [-1,1], we will get 0.0f
	output.Normal.rgb = 0.5f;
	//no specular power
	output.Normal.a = 0.0f;
	//max depth
	output.Depth = 0.0f;
	output.Emission = (float4)0;
	output.Output = (float4)0;
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