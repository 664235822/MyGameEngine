#shader vertex
#version 410 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;
layout (location = 3) in vec3 geo_Tangent;
layout (location = 4) in vec3 geo_BitTangent;
layout (location = 0) out vec3 color;
layout (location = 1) out vec2 texCoords;
uniform mat4 model;
uniform mat4 view;
uniform mat4 perspective;
void main()
{
    gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0) * model * view * perspective;
    //color = aColor;
    texCoords = aTexCoords;
}
#shader fragment
#version 410 core
layout (location = 0) in vec3 color;
layout (location = 1) in vec2 texCoords;
out vec4 FragColor;

uniform sampler2D mainTex;
void main()
{
    FragColor = texture(mainTex, texCoords);
}