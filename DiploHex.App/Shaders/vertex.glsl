#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec4 aColor;

uniform mat4 uTransform;

out vec4 vColor;

void main()
{
    gl_Position = uTransform * vec4(aPosition, 1.0);
    vColor = aColor;
}