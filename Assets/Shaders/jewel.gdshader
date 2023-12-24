shader_type spatial;
render_mode specular_schlick_ggx;

uniform vec4 Color;
uniform float Metallic = 1.0;
uniform float Roughness = 0.0;
uniform float Specular = 1.0;

void vertex() {
    // Vertex output
}

void fragment() {
        vec3 base_color = Color.rgb * 15.0;
    
    ALBEDO = base_color;
    
    METALLIC = Metallic;
    SPECULAR = Specular;
    ROUGHNESS = Roughness;

    vec3 view_dir = normalize(VIEW);
    vec3 normal = normalize(NORMAL);
    
    float fresnel = dot(view_dir, normal);
    fresnel = clamp(1.0 - fresnel, 0.0, 1.0);
    fresnel = pow(fresnel, 3.0);  

    vec2 uv = UV; 
    uv.y += TIME;
    
    vec3 shine = mix(vec3(0.8, 0.9, 1.0), vec3(1.0), uv.y);    
    shine *= fresnel;

    ALBEDO += shine * 0.5;
}

void light() {
  
}