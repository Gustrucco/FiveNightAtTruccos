/*********************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

//Textura para Lightmap
texture texLightMap;
sampler2D lightMap = sampler_state
{
   Texture = (texLightMap);
};

//Material del mesh
float3 materialEmissiveColor; //Color RGB
float3 materialAmbientColor; //Color RGB
float4 materialDiffuseColor; //Color ARGB (tiene canal Alpha)
float3 materialSpecularColor; //Color RGB
float materialSpecularExp; //Exponente de specular

//Parametros Comunes de las luces
float3 lightColor; //Color RGB de la luz
float4 eyePosition; //Posicion de la camara
float lightAttenuation; //Factor de atenuacion de la luz

//Parametros Point Light
float4 lightPosition; //Posicion de la luz
float lampIntensity; //Intensidad de la luz


//Parametros Spot Light
float4 lanternPosition; //Posicion de la luz
float lanternIntensity; //Intensidad
float3 spotLightDir; //Direccion del cono de luz
float spotLightAngleCos; //Angulo de apertura del cono de luz (en radianes)
float spotLightExponent; //Exponente de atenuacion dentro del cono de luz

/**************************************************************************************/
/* VERTEX_COLOR */
/**************************************************************************************/



//Input del Vertex Shader
struct VS_INPUT_VERTEX_COLOR 
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float4 Color : COLOR;
};

//Output del vecter shader
struct VS_OUTPUT_VERTEX_COLOR
{
	float4 Position : POSITION0;
	float4 Color : COLOR;
	float3 WorldPosition : TEXCOORD0;
	float3 WorldNormal : TEXCOORD1;
	float3 PointLightLightVec	: TEXCOORD2;
	float3 PointLightHalfAngleVec	: TEXCOORD3;
	float3 SpotLightLightVec	: TEXCOORD4;
	float3 SpotLightHalfAngleVec	: TEXCOORD5;
};

VS_OUTPUT_VERTEX_COLOR vs_VertexColor(VS_INPUT_VERTEX_COLOR input)
{
	VS_OUTPUT_VERTEX_COLOR output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar color directamente
	output.Color = input.Color;

	//Posicion pasada a World-Space (necesaria para atenuación por distancia)
	output.WorldPosition = mul(input.Position, matWorld);

	/* Pasar normal a World-Space 
	Solo queremos rotarla, no trasladarla ni escalarla.
	Por eso usamos matInverseTransposeWorld en vez de matWorld */
	output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;
	
	//ViewVec (V): vector que va desde el vertice hacia la camara.
	float3 viewVector = eyePosition.xyz - output.WorldPosition;

	//----------------------------------------PointLight--------------------------------------------
		//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
	output.PointLightLightVec = lightPosition.xyz - output.WorldPosition;
	
	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
	output.PointLightHalfAngleVec = viewVector + output.PointLightLightVec;
	
	//-----------------------------------------SpotLight--------------------------------------------
		//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
	output.SpotLightLightVec = lanternPosition.xyz - output.WorldPosition;
	
	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
	output.SpotLightHalfAngleVec = viewVector + output.SpotLightLightVec;
	
	return output;
}

//Input del Pixel Shader
struct PS_INPUT_VERTEX_COLOR 
{
	float4 Color : COLOR0; 
	float3 WorldPosition : TEXCOORD0;
	float3 WorldNormal : TEXCOORD1;
	float3 PointLightLightVec	: TEXCOORD2;
	float3 PointLightHalfAngleVec	: TEXCOORD3;
	float3 SpotLightLightVec	: TEXCOORD4;
	float3 SpotLightHalfAngleVec	: TEXCOORD5;
};

//Pixel Shader
float4 ps_VertexColor(PS_INPUT_VERTEX_COLOR input) : COLOR0
{      
	//Normalizar vectores
	float3 Nn = normalize(input.WorldNormal);
	float3 Lns = normalize(input.SpotLightLightVec);
	float3 Hns = normalize(input.SpotLightHalfAngleVec);
	float3 Lnp = normalize(input.PointLightLightVec);
	float3 Hnp = normalize(input.PointLightHalfAngleVec);
	
	//Calcular intensidad de luz, con atenuacion por distancia
	float distAtten =length(lightPosition.xyz - input.WorldPosition) * lightAttenuation;
	float distAtten2 =length(lanternPosition.xyz - input.WorldPosition) * lightAttenuation;
	
	//Calcular atenuacion por Spot Light. Si esta fuera del angulo del cono tiene 0 intensidad.
	float spotAtten = dot(-spotLightDir, normalize(input.SpotLightLightVec));
	spotAtten = (spotAtten > spotLightAngleCos) 
					? pow(spotAtten, spotLightExponent)
					: 0.0;

	//Calcular intensidad de la luz segun la atenuacion por distancia y si esta adentro o fuera del cono de luz
	float intensity1 = lanternIntensity * spotAtten / pow(distAtten2,2);
	float intensity2 = lampIntensity / distAtten;
	float intensity = intensity1 + intensity2;
	
	//Componente Ambient
	float3 ambientLight = intensity * lightColor * materialAmbientColor;
	
		//Componente Diffuse: N dot L
	
	float3 n_dot_lp = dot(Nn, Lnp);	
	float3 pointDiffuseLight = intensity2 * lightColor * materialDiffuseColor.rgb * max(0.0, n_dot_lp); //Controlamos que no de negativo
	
	float3 n_dot_ls = dot(Nn, Lns);
	float3 spotDiffuseLight = intensity1 * lightColor * materialDiffuseColor.rgb * max(0.0, n_dot_ls); //Controlamos que no de negativo
	
	//Componente Specular: (N dot H)^exp
	
	float3 n_dot_hp = dot(Nn, Hnp);
	float3 pointSpecularLight = n_dot_lp <= 0.0
			? float3(0.0, 0.0, 0.0)
			: (intensity2 * lightColor * materialSpecularColor * pow(max( 0.0, n_dot_hp), materialSpecularExp));
	
	float3 n_dot_hs = dot(Nn, Hns);
	float3 spotSpecularLight = n_dot_ls <= 0.0
			? float3(0.0, 0.0, 0.0)
			: (intensity1 * lightColor * materialSpecularColor * pow(max( 0.0, n_dot_hs), materialSpecularExp));
	
	
	/* Color final: modular (Emissive + Ambient + Diffuse) por el color del mesh, y luego sumar Specular.
	   El color Alpha sale del diffuse material */
	float4 finalColor = float4(saturate(materialEmissiveColor + ambientLight + pointDiffuseLight + spotDiffuseLight) * input.Color + pointSpecularLight + spotSpecularLight , materialDiffuseColor.a);
	
	
	return finalColor;
}

/*
* Technique VERTEX_COLOR
*/
technique VERTEX_COLOR
{
   pass Pass_0
   {
	  VertexShader = compile vs_3_0 vs_VertexColor();
	  PixelShader = compile ps_3_0 ps_VertexColor();
   }
}


/**************************************************************************************/
/* DIFFUSE_MAP */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT_DIFFUSE_MAP 
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float4 Color : COLOR;
	float2 Texcoord : TEXCOORD0;
};

//Output del vecter shader
struct VS_OUTPUT_DIFFUSE_MAP
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
	float3 WorldPosition : TEXCOORD1;
	float3 WorldNormal : TEXCOORD2;
	float3 PointLightLightVec	: TEXCOORD3;
	float3 PointLightHalfAngleVec	: TEXCOORD4;
	float3 SpotLightLightVec	: TEXCOORD5;
	float3 SpotLightHalfAngleVec	: TEXCOORD6;
};

//Vertex Shader
VS_OUTPUT_DIFFUSE_MAP vs_DiffuseMap(VS_INPUT_DIFFUSE_MAP input)
{ 
	VS_OUTPUT_DIFFUSE_MAP output;

	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;

	//Posicion pasada a World-Space (necesaria para atenuación por distancia)
	output.WorldPosition = mul(input.Position, matWorld);

	/* Pasar normal a World-Space 
	Solo queremos rotarla, no trasladarla ni escalarla.
	Por eso usamos matInverseTransposeWorld en vez de matWorld */
	output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;
	
	//ViewVec (V): vector que va desde el vertice hacia la camara.
	float3 viewVector = eyePosition.xyz - output.WorldPosition;

	//----------------------------------------PointLight--------------------------------------------
		//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
	output.PointLightLightVec = lightPosition.xyz - output.WorldPosition;
	
	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
	output.PointLightHalfAngleVec = viewVector + output.PointLightLightVec;
	
	//-----------------------------------------SpotLight--------------------------------------------
		//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
	output.SpotLightLightVec = lanternPosition.xyz - output.WorldPosition;
	
	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
	output.SpotLightHalfAngleVec = viewVector + output.SpotLightLightVec;


	return output;
}

struct PS_DIFFUSE_MAP
{
	float4 Texcoord: TEXCOORD0;
	float3 WorldPosition : TEXCOORD1;
	float3 WorldNormal : TEXCOORD2;
	float3 PointLightLightVec	: TEXCOORD3;
	float3 PointLightHalfAngleVec	: TEXCOORD4;
	float3 SpotLightLightVec	: TEXCOORD5;
	float3 SpotLightHalfAngleVec	: TEXCOORD6;
};

//Pixel Shader
float4 ps_DiffuseMap(PS_DIFFUSE_MAP input) : COLOR0
{
	//Normalizar vectores
	float3 Nn = normalize(input.WorldNormal);
	float3 Lns = normalize(input.SpotLightLightVec);
	float3 Hns = normalize(input.SpotLightHalfAngleVec);
	float3 Lnp = normalize(input.PointLightLightVec);
	float3 Hnp = normalize(input.PointLightHalfAngleVec);
	
	//Calcular intensidad de luz, con atenuacion por distancia
	float distAtten =  length(lightPosition.xyz - input.WorldPosition) *lightAttenuation;
	float distAtten2 =length(lanternPosition.xyz - input.WorldPosition) * lightAttenuation* 0.1;
	
	//Calcular atenuacion por Spot Light. Si esta fuera del angulo del cono tiene 0 intensidad.
	float spotAtten = dot(-spotLightDir, normalize(input.SpotLightLightVec));
	spotAtten = (spotAtten > spotLightAngleCos) 
					? pow(spotAtten, spotLightExponent)
					: 0.0;

	//Calcular intensidad de la luz segun la atenuacion por distancia y si esta adentro o fuera del cono de luz
	float intensity1 = lanternIntensity * spotAtten / pow(distAtten2,2);
	float intensity2 = lampIntensity / distAtten;
	float intensity = intensity1 + intensity2;
					
	//Obtener texel de la textura
	float4 texelColor = tex2D(diffuseMap, input.Texcoord);
	
	//Componente Ambient
	float3 ambientLight = intensity * lightColor * materialAmbientColor;
	
	//Componente Diffuse: N dot L
	
	float3 n_dot_lp = dot(Nn, Lnp);	
	float3 pointDiffuseLight = intensity2 * lightColor * materialDiffuseColor.rgb * max(0.0, n_dot_lp); //Controlamos que no de negativo
	
	float3 n_dot_ls = dot(Nn, Lns);
	float3 spotDiffuseLight = intensity1 * lightColor * materialDiffuseColor.rgb * max(0.0, n_dot_ls); //Controlamos que no de negativo
	
	//Componente Specular: (N dot H)^exp
	
	float3 n_dot_hp = dot(Nn, Hnp);
	float3 pointSpecularLight = n_dot_lp <= 0.0
			? float3(0.0, 0.0, 0.0)
			: (intensity2 * lightColor * materialSpecularColor * pow(max( 0.0, n_dot_hp), materialSpecularExp));
	
	float3 n_dot_hs = dot(Nn, Hns);
	float3 spotSpecularLight = n_dot_ls <= 0.0
			? float3(0.0, 0.0, 0.0)
			: (intensity1 * lightColor * materialSpecularColor * pow(max( 0.0, n_dot_hs), materialSpecularExp));
	/* Color final: modular (Emissive + Ambient + Diffuse) por el color de la textura, y luego sumar Specular.
	   El color Alpha sale del diffuse material */
	float4 finalColor = float4(saturate(materialEmissiveColor + ambientLight + pointDiffuseLight + spotDiffuseLight) * texelColor + pointSpecularLight + spotSpecularLight, materialDiffuseColor.a);
	
	
	return finalColor;
}

/*
* Technique DIFFUSE_MAP
*/
technique DIFFUSE_MAP
{
   pass Pass_0
   {
	  VertexShader = compile vs_3_0 vs_DiffuseMap();
	  PixelShader = compile ps_3_0 ps_DiffuseMap();
   }
}

/**************************************************************************************/
/* DIFFUSE_MAP_AND_LIGHTMAP */
/**************************************************************************************/


//Input del Vertex Shader
struct VS_INPUT_DIFFUSE_MAP_AND_LIGHTMAP
{
   float4 Position : POSITION0;
   float3 Normal : NORMAL0;
   float4 Color : COLOR;
   float2 Texcoord : TEXCOORD0;
   float2 TexcoordLightmap : TEXCOORD1;
};

//Output del Vertex Shader
struct VS_OUTPUT_DIFFUSE_MAP_AND_LIGHTMAP
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
	float2 TexcoordLightmap : TEXCOORD1;
	float3 WorldPosition : TEXCOORD2;
	float3 WorldNormal : TEXCOORD3;
	float3 PointLightLightVec	: TEXCOORD4;
	float3 PointLightHalfAngleVec	: TEXCOORD5;
	float3 SpotLightLightVec	: TEXCOORD6;
	float3 SpotLightHalfAngleVec	: TEXCOORD7;
};

//Vertex Shader
VS_OUTPUT_DIFFUSE_MAP_AND_LIGHTMAP vs_diffuseMapAndLightmap(VS_INPUT_DIFFUSE_MAP_AND_LIGHTMAP input)
{

	VS_OUTPUT_DIFFUSE_MAP_AND_LIGHTMAP output;
	
	//Proyectar posicion
	output.Position = mul(input.Position, matWorldViewProj);

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;
	output.TexcoordLightmap = input.TexcoordLightmap;

	//Posicion pasada a World-Space (necesaria para atenuación por distancia)
	output.WorldPosition = mul(input.Position, matWorld);

	/* Pasar normal a World-Space 
	Solo queremos rotarla, no trasladarla ni escalarla.
	Por eso usamos matInverseTransposeWorld en vez de matWorld */
	output.WorldNormal = mul(input.Normal, matInverseTransposeWorld).xyz;

	//ViewVec (V): vector que va desde el vertice hacia la camara.
	float3 viewVector = eyePosition.xyz - output.WorldPosition;
	
	//----------------------------------------PointLight--------------------------------------------
		//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
	output.PointLightLightVec = lightPosition.xyz - output.WorldPosition;
		
	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
	output.PointLightHalfAngleVec = viewVector + output.PointLightLightVec;
	
	//-----------------------------------------SpotLight--------------------------------------------
		//LightVec (L): vector que va desde el vertice hacia la luz. Usado en Diffuse y Specular
	output.SpotLightLightVec = lanternPosition.xyz - output.WorldPosition;

	//HalfAngleVec (H): vector de reflexion simplificado de Phong-Blinn (H = |V + L|). Usado en Specular
	output.SpotLightHalfAngleVec = viewVector + output.SpotLightLightVec;

	return output;
}

//Input del Pixel Shader
struct PS_INPUT_DIFFUSE_MAP_AND_LIGHTMAP
{
	float2 Texcoord : TEXCOORD0;
	float2 TexcoordLightmap : TEXCOORD1;
	float3 WorldPosition : TEXCOORD2;
	float3 WorldNormal : TEXCOORD3;
	float3 PointLightLightVec	: TEXCOORD4;
	float3 PointLightHalfAngleVec	: TEXCOORD5;
	float3 SpotLightLightVec	: TEXCOORD6;
	float3 SpotLightHalfAngleVec	: TEXCOORD7;
};

//Pixel Shader
float4 ps_diffuseMapAndLightmap(PS_INPUT_DIFFUSE_MAP_AND_LIGHTMAP input) : COLOR0
{	
		//Normalizar vectores
	float3 Nn = normalize(input.WorldNormal);
	float3 Lns = normalize(input.SpotLightLightVec);
	float3 Hns = normalize(input.SpotLightHalfAngleVec);
	float3 Lnp = normalize(input.PointLightLightVec);
	float3 Hnp = normalize(input.PointLightHalfAngleVec);
	
	//Calcular intensidad de luz, con atenuacion por distancia
	float distAtten = length(lightPosition.xyz - input.WorldPosition) * lightAttenuation;
	float distAtten2 =length(lanternPosition.xyz - input.WorldPosition) * lightAttenuation * 0.1;
	//Calcular atenuacion por Spot Light. Si esta fuera del angulo del cono tiene 0 intensidad.
	float spotAtten = dot(-spotLightDir, normalize(input.SpotLightLightVec));
	spotAtten = (spotAtten > spotLightAngleCos) 
					? pow(spotAtten, spotLightExponent)
					: 0.0;
	
	//Calcular intensidad de la luz segun la atenuacion por distancia y si esta adentro o fuera del cono de luz
	float intensity1 = lanternIntensity * spotAtten / pow(distAtten2,2);
	float intensity2 = lampIntensity / distAtten;
	float intensity = intensity1 + intensity2;
	
	//Obtener color de diffuseMap y de Lightmap
	float4 texelColor = tex2D(diffuseMap, input.Texcoord);
	float4 lightmapColor = tex2D(lightMap, input.TexcoordLightmap);
	
	//Componente Ambient
	float3 ambientLight = intensity * lightColor * materialAmbientColor;
	
	//Componente Diffuse: N dot L
	
	float3 n_dot_lp = dot(Nn, Lnp);	
	float3 pointDiffuseLight = intensity2 * lightColor * materialDiffuseColor.rgb * max(0.0, n_dot_lp); //Controlamos que no de negativo
	
	float3 n_dot_ls = dot(Nn, Lns);
	float3 spotDiffuseLight = intensity1 * lightColor * materialDiffuseColor.rgb * max(0.0, n_dot_ls); //Controlamos que no de negativo
	
	//Componente Specular: (N dot H)^exp
	
	float3 n_dot_hp = dot(Nn, Hnp);
	float3 pointSpecularLight = n_dot_lp <= 0.0
			? float3(0.0, 0.0, 0.0)
			: (intensity2 * lightColor * materialSpecularColor * pow(max( 0.0, n_dot_hp), materialSpecularExp));
	
	float3 n_dot_hs = dot(Nn, Hns);
	float3 spotSpecularLight = n_dot_ls <= 0.0
			? float3(0.0, 0.0, 0.0)
			: (intensity1 * lightColor * materialSpecularColor * pow(max( 0.0, n_dot_hs), materialSpecularExp));
	
	/* Color final: modular (Emissive + Ambient + Diffuse) por el color de la textura, y luego sumar Specular.
	   El color Alpha sale del diffuse material */
	float4 finalColor = float4(saturate(materialEmissiveColor + ambientLight + pointDiffuseLight + spotDiffuseLight) * (texelColor * lightmapColor) + pointSpecularLight + spotSpecularLight, materialDiffuseColor.a);
	return finalColor;
}


//technique DIFFUSE_MAP_AND_LIGHTMAP
technique DIFFUSE_MAP_AND_LIGHTMAP
{
   pass Pass_0
   {
	  VertexShader = compile vs_3_0 vs_diffuseMapAndLightmap();
	  PixelShader = compile ps_3_0 ps_diffuseMapAndLightmap();
   }
}
