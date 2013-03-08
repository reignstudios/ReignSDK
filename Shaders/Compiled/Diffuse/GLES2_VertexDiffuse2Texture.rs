#GLOBAL
varying vec4 Position_VSPS;
varying vec4 Light_VSPS;
varying vec2 UV_VSPS;
#END

#VS
attribute vec3 Position0;
attribute vec3 Normal0;
attribute vec2 Texcoord0;

uniform mat4 Camera;
uniform vec3 LightDirection;
uniform vec3 LightDirection2;
uniform vec4 LightColor;
uniform vec4 LightColor2;
uniform mat4 Transform;

void main()
{
	vec4 loc = (Transform *  vec4(Position0, 1));
	gl_Position = Position_VSPS = (loc * Camera);
	UV_VSPS = vec2(Texcoord0.x, 1.0-Texcoord0.y);

	vec3 normal = normalize((Transform *  vec4(Normal0, 0)).xyz);
	float light = max(dot(-LightDirection, normal), 0.0);
	float light2 = max(dot(-LightDirection2, normal), 0.0);
	Light_VSPS = ((light * LightColor) + (light2 * LightColor2));
}
#END

#PS
uniform sampler2D Diffuse;

void main()
{
	gl_FragData[0] = texture2D(Diffuse, UV_VSPS) * Light_VSPS;
}
#END

