#version 330 core
layout (location = 0) in vec3 aPosition;

uniform vec3 aOffset;

void main()
{
    gl_Position = vec4(aPosition + aOffset, 1.0);
}