#GLOBAL
varying vec4 Position_VSPS;
varying vec2 UV_VSPS;
#END

#VS
attribute vec3 Position0;
attribute vec2 Texcoord0;

uniform mat4 Camera;
uniform mat4 BillboardTransform;
uniform vec4 ColorPallet[4];
uniform vec4 ScalePallet;
uniform vec4 Transforms[10];

void main()
{
	vec4 loc = ( vec4(Position0, 1) * BillboardTransform);
	gl_Position = Position_VSPS = (loc * Camera);
	UV_VSPS = vec2(Texcoord0.x, 1.0-Texcoord0.y);
}
#END

#PS
uniform sampler2D Diffuse;

void main()
{
	gl_FragData[0] = texture2D(Diffuse, UV_VSPS);
}
#END

