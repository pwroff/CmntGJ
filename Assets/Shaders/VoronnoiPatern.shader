Shader "Unlit/VoronnoiPatern"
{
	Properties
	{
		_Color("Initial Color", Color) = (0.0, 0.0, 0.0, 1.0)
		_ColorAdd("Added Color", Color) = (0.0, 0.0, 1.0, 1.0)
		_CellScale("Cell Scale", Range(0, 1200)) = 2
		_CellOffset("Cell Offset", Range(0, 120)) = 0
		_MovementSpeed("Movement Speed", Range(0, 20)) = 0
		_Cutout("Cell cutout", Range(0.1, 10)) = 0.5

	}
		SubShader{

			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				fixed4 _Color, _ColorAdd;
				float _CellOffset, _CellScale, _MovementSpeed, _Cutout;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}
				float2 random2(float2 p)
				{
					return frac(sin(float2(dot(p,float2(117.12,341.7)),dot(p,float2(269.5,123.3))))*43458.5453);
				}
				float voronnoiDistance(float2 uv)
				{
					uv -= float2(0.5, 0.5);
					uv *= _CellScale; //Scaling amount (larger number more cells can be seen)
					float2 iuv = floor(uv); //gets integer values no floating point
					float2 fuv = frac(uv); // gets only the fractional part
					float minDist = 1.0;  // minimun distance
					for (int y = -1; y <= 1; y++)
					{
						for (int x = -1; x <= 1; x++)
						{
							// Position of neighbour on the grid
							float2 neighbour = float2(float(x), float(y));
							// Random position from current + neighbour place in the grid
							float2 pointv = random2(iuv + neighbour);
							// Move the point with time
							pointv = 0.5 + 0.5*sin(_Time.y * _MovementSpeed + _CellOffset * pointv);//each point moves in a certain way
																			// Vector between the pixel and the point
							float2 diff = neighbour + pointv - fuv;
							// Distance to the point
							float dist = length(diff);
							// Keep the closer distance
							minDist = min(minDist, dist);
						}
					}
					return minDist;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = _Color;
					float2 uv = i.uv;
					float xu = uv.x;
					float yu = uv.y;
					float minDist = voronnoiDistance(i.uv);
					// Draw the min distance (distance field)
					col += pow(_ColorAdd * (minDist * minDist), _Cutout); // squared it to to make edges look sharper

					return col;
				}
				ENDCG
			}
	}
}
