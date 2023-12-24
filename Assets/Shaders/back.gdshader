shader_type spatial;
render_mode specular_schlick_ggx;

uniform vec4 Color;
uniform float HowShiny = 0.5;
uniform float Roughness = 0.2;



void vertex() {
// Output:0

}

void fragment() {
	ALBEDO =Color.rgb;;
	METALLIC = HowShiny;
	ROUGHNESS = Roughness;

}

void light() {
// Output:0

}
