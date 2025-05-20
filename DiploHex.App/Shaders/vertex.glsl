#version 330 core

layout (location = 0) in vec3 aPosition;

uniform vec2 uOffset;

void main()
{
    vec3 offset = vec3(uOffset, 0.0);
    vec3 position = aPosition + offset;
    gl_Position = vec4(position, 1.0);
}