#GLOBAL
#END

#VS
attribute vec3 Position0;
attribute vec4 Color0;

out vec4 Position_VSPS;
out vec4 Color_VSPS;

uniform mat4 Camera;

void main()
{
	gl_Position = Position_VSPS =  vec4(Position0, 1.0) * Camera;
	Color_VSPS = Color0;
}
#END

#PS
in vec4 Position_VSPS;
in vec4 Color_VSPS;

out vec4 glFragColorOut[1];


void main()
{
	glFragColorOut[0] = Color_VSPS;
}
#END

