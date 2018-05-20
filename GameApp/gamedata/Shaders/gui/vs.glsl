#version 330 core
layout (location = 0) in vec2 in_position;
layout (location = 1) in vec2 in_texcood;

out vec2 TexCoord;

uniform mat3 texcoodTransform;
uniform mat3 transform;
uniform mat3 screen;

void main()
{
	TexCoord = (texcoodTransform * vec3(in_texcood, 1)).xy;
    gl_Position = vec4(screen * transform * vec3(in_position, 1), 1);
}
