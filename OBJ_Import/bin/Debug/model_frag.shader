#version 330 core
in vec3 pos;
in vec2 texCoord;
in vec3 normal;

struct Material {
	vec3 ambient;
	vec3 diffuse;
	vec3 specular;
	float shininess;
};

struct DirectionalLight {
	vec3 direction;
	vec3 ambient;
	vec3 diffuse;
	vec3 specular;
	float intensity;
};


uniform vec3 camPos;
uniform Material material;
uniform DirectionalLight light;

uniform sampler2D texture_diffuse1;

out vec4 color;

void main(){
    vec2 t = texCoord;
    //t.y = 1 - t.y;
    //t.x = 1 - t.x;

    color =  texture(texture_diffuse1, t);

	//Ambient color
	vec3 ambient = material.ambient * light.ambient;

	//Diffuse color
	vec3 Normal = normalize(normal);
	//Normal *= -1.0f;
	vec3 lightDir = light.direction;

	float diff = max(dot(-lightDir, Normal), 0.0f);
	vec3 diffuse = diff * material.diffuse * light.diffuse;

	//Specular color
	vec3 viewDir = normalize(camPos - pos);
	vec3 reflectDir = normalize(reflect(lightDir, Normal));
	vec3 specular = material.specular * pow(max(dot(viewDir, reflectDir), 0.0f), material.shininess) * light.specular;

	//Output color//vec3(0.1f,1,0.5f);
	vec3 texColor =  texture(texture_diffuse1, t).xyz;

	color = vec4(vec3((ambient + diffuse) * texColor + specular) * light.intensity, 1.0f);
}
