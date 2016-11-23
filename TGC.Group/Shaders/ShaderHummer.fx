// ---------------------------------------------------------
// Ejemplo shader Minimo:
// ---------------------------------------------------------

/**************************************************************************************/
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

float time = 0;
float Velocidad = 0;
float Deformation = 1;

float PosX;
float PosZ;
float Largo;
float ChocoAdelante;
float ChocoAtras;

float ChoqueTrasero;
float ChoqueDelantero;

/**************************************************************************************/
/* RenderScene */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position :        POSITION0;
	float2 Texcoord :        TEXCOORD0;
	float4 Color :			COLOR0;
};

//Vertex Shader
VS_OUTPUT vs_main(VS_INPUT Input)
{
	VS_OUTPUT Output;
	//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
	Output.Color = Input.Color;

	return(Output);
}

// Ejemplo de un vertex shader que anima la posicion de los vertices
// ------------------------------------------------------------------
VS_OUTPUT vs_main2(VS_INPUT Input)
{
	VS_OUTPUT Output;

	// Animar posicion
	/*Input.Position.x += sin(time)*30*sign(Input.Position.x);
	Input.Position.y += cos(time)*30*sign(Input.Position.y-20);
	Input.Position.z += sin(time)*30*sign(Input.Position.z);*/
	

	// Animar posicion
	float X = Input.Position.x;
	float Y = Input.Position.y;
	float Z = Input.Position.z;
	float v = 0.08*Velocidad;// *sin(time);
	float d = Deformation;
	float tiempo = 0;
	//if (d < 1) d = 1;
	//if (d > 2) d = 2;

	if (v < 1) v = 1;
	if (v > 1.2) v = 1.25;
	Input.Position.y = Y/v;// *cos(v) - Z * sin(v);
	Input.Position.x = X +sin(time) * 8*Velocidad;
	//Input.Position.z = Z / d;// *cos(v) + Y * sin(v);

	//tiempo = tiempo + time ;
	
	if (ChoqueDelantero == 1)// && Velocidad > 0)
	{
		if (Input.Position.z < Largo * 0.22)
		{
			Input.Position.z = Z - 10 * sin(Y * 300);

			Input.Position.x = Input.Position.x - 10 * sin(Y * 300);
		}
	}
	if (ChoqueTrasero == -1)// && Velocidad < 0)
	{
		if (Input.Position.z > Largo * 0.22)
		{
			Input.Position.z = Z - 10 * sin(Y * 200);

			Input.Position.x = Input.Position.x  - 10 * sin(Y * 200);
		}
	}
	


	//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;

	// Animar color
	Input.Color.r = 255;// Input.Color.r*v + 222;
	//Input.Color.r = abs(sin(time));
	//Input.Color.g;// = abs(cos(time));

	//Propago el color x vertice
	Output.Color = Input.Color;

	return(Output);
}

//Pixel Shader
float4 ps_main(float2 Texcoord: TEXCOORD0, float4 Color : COLOR0) : COLOR0
{
	// Obtener el texel de textura
	// diffuseMap es el sampler, Texcoord son las coordenadas interpoladas
	float screen_dx = 2650;
	float2 texPantalla = 2650*Texcoord; //
	texPantalla.x = floor(texPantalla.x);
	texPantalla.y = floor(texPantalla.y);

	/*if (fmod(texPantalla.x, 32)) {
		discard;
	}*/
	//return 0;
		
	float4 fvBaseColor = tex2D(diffuseMap, Texcoord);// *2 * sin(time)); //para que se ponga re loco

	//punto1
	/*float2 q = 2 * Tex - 1;
	float d = lenght(q);
	float 4 colorBase = 0;
	if (d < 0) {
		float k = d*d;
		float2 uv = 0, 5 + normalize(q) * 0, 5 * k;
		Colorbase = tex2D(RenderTarget, uv);
		fvBaseColor=Colorbase
	}*/

	// combino color y textura
	// en este ejemplo combino un 80% el color de la textura y un 20%el del vertice
	fvBaseColor.r = fvBaseColor.r + 0.005*(Velocidad-1.2);// *3 * sin(2 * time);// *sin(time);
	fvBaseColor.g = fvBaseColor.g;//*sin(time);
		return fvBaseColor;// -0.01*Color*Velocidad;
}

// ------------------------------------------------------------------
technique RenderScene
{
	pass Pass_0
	{
		VertexShader = compile vs_2_0 vs_main2();
		PixelShader = compile ps_2_0 ps_main();
	}
}

