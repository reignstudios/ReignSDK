#GLOBAL
#END

#VS
attribute vec3 Position0;
attribute vec4 Color0;
attribute vec2 Texcoord0;

out vec4 Position_VSPS;
out vec4 Color_VSPS;
out vec2 UV_VSPS;

uniform mat4 Camera;

void main()
{
	gl_Position = Position_VSPS =  vec4(Position0, 1) * Camera;
	Color_VSPS = Color0;
	UV_VSPS = vec2(Texcoord0.x, 1.0-Texcoord0.y);
}
#END

#PS
in vec4 Position_VSPS;
in vec4 Color_VSPS;
in vec2 UV_VSPS;

out vec4 glFragColorOut[1];

uniform sampler2D Diffuse;

void main()
{
	glFragColorOut[0] = texture2D(Diffuse, UV_VSPS) * Color_VSPS;
}
#END

