#version 330 core
layout(location = 0) in vec3 vPos;
layout(location = 1) in vec2 vTexCoord;
layout(location = 2) in vec3 vNormal;

out vec3 pos;
out vec3 normal;
out vec2 texCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(){
    vec4 a = projection * view * model * vec4(vPos, 1.0f);
    pos = (model * vec4(vPos, 1.0f)).xyz;
    gl_Position = a;

    vec3 aNormal = vNormal;
    aNormal.z *= -1.0f;
    //aNormal.y = vNormal.z;
    //aNormal.z = vNormal.y;

    normal = mat3(model) * aNormal; 
    texCoord = vTexCoord.xy;
}

/*
 vec4 a = vec4(vPos, 1.0f) * model * view;
    pos = vec3(a.xyz);
    gl_Position = a * projection;
    normal = vNormal * mat3(model);
    texCoord = vTexCoord.xy;
*/