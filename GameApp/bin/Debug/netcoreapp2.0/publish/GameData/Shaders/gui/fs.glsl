#version 330 core
out vec4 FragColor;

in vec2 TexCoord;

uniform sampler2D texture1;
uniform int useTexture;
uniform vec4 color;

void main()
{
    if(useTexture == 1)
    {
        FragColor = texture(texture1, TexCoord).rgba * color;
        //FragColor = vec4(TexCoord.x, TexCoord.y, 0, 1);
    }
    else
    {
        FragColor = color;
    }
}