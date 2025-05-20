#version 330 core

layout (location = 0) in vec3 aPosition;

uniform mat4 uTransform;

void main()
{
    gl_Position = uTransform * vec4(aPosition, 1.0);
}