#GLOBAL
varying vec4 Position_VSPS;
varying vec3 Normal_VSPS;
varying vec2 UV_VSPS;
#END

#VS
attribute vec3 Position0;
attribute vec3 Normal0;
attribute vec2 Texcoord0;

uniform mat4 Camera;
uniform mat4 Transform;

void main()
{
	vec4 loc = (Transform *  vec4(Position0, 1));
	gl_Position = Position_VSPS = (loc * Camera);
	Normal_VSPS = normalize((Transform *  vec4(Normal0, 0)).xyz);
	UV_VSPS = vec2(Texcoord0.x, 1.0-Texcoord0.y);
}
#END

#PS
uniform vec3 LightDirection;
uniform vec4 LightColor;
uniform sampler2D Diffuse;

void main()
{
	float light = dot(-LightDirection, Normal_VSPS);
	gl_FragData[0] = texture2D(Diffuse, UV_VSPS) * LightColor * light;
}
#END

