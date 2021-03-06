#GLOBAL
#END

#VS
in vec2 Position0;

out vec4 Position_VSPS;
out vec2 UV_VSPS;

uniform mat4 Camera;
uniform vec2 Position;
uniform vec2 Size;
uniform vec2 PositionUV;
uniform vec2 SizeUV;
uniform vec2 TexelOffset;

void main()
{
	vec3 loc = vec3((Position0 * Size) + Position, 0);
	gl_Position = Position_VSPS = ( vec4(loc, 1.0) * Camera);

	vec2 uv = Position0 + TexelOffset;
	UV_VSPS = ( vec2(uv.x, 1.0-uv.y) * SizeUV) + PositionUV;
}
#END

#PS
in vec4 Position_VSPS;
in vec2 UV_VSPS;

out vec4 glFragColorOut[1];

uniform vec4 Color;
uniform sampler2D DiffuseTexture;

void main()
{
	glFragColorOut[0] = texture2D(DiffuseTexture, UV_VSPS) * Color;
}
#END

