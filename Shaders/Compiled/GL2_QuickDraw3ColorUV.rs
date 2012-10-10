#GLOBAL
varying vec4 Position_VSPS;
varying vec4 Color_VSPS;
varying vec2 UV_VSPS;
#END

#VS
attribute vec3 Position0;
attribute vec4 Color0;
attribute vec2 Texcoord0;

uniform mat4 Camera;

void main()
{
	gl_Position = Position_VSPS = ( vec4(Position0, 1) * Camera);
	Color_VSPS = Color0;
	UV_VSPS = vec2(Texcoord0.x, 1.0-Texcoord0.y);
}
#END

#PS
uniform sampler2D Diffuse;

void main()
{
	gl_FragData[0] = texture2D(Diffuse, UV_VSPS) * Color_VSPS;
}
#END

