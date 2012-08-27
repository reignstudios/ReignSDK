#GLOBAL
varying vec4 Position_VSPS;
varying vec4 Color_VSPS;
#END

#VS
attribute vec3 Position0;
attribute vec4 Color0;

uniform mat4 Camera;

void main()
{
	gl_Position = Position_VSPS =  vec4(Position0, 1.0) * Camera;
	Color_VSPS = Color0;
}
#END

#PS

void main()
{
	gl_FragData[0] = Color_VSPS;
}
#END

