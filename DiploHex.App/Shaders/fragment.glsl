#version 330 core

uniform vec4 uColor;
uniform float uColorBlend;

in vec4 vColor;

out vec4 FragColor;

void main()
{
    FragColor = mix(vColor, uColor, uColorBlend);
}