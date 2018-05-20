#version 330 core
layout (location = 0) in vec3 in_position;
layout (location = 1) in vec3 in_normal;
layout (location = 2) in vec2 in_texcood;

out vec3 FragPos;
out vec2 TexCoord;
out vec3 Normal;
out vec3 LightPos;

const vec3 lightPos = vec3(1200, 1000, 2000);

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    Normal = mat3(transpose(inverse(view * model))) * in_normal;
    LightPos = vec3(view * vec4(lightPos, 1.0));
    FragPos = vec3(view * model * vec4(in_position, 1.0));
	TexCoord = vec2(in_texcood.x, in_texcood.y);
    
    gl_Position = projection * view * model * vec4(in_position, 1.0);
}
