using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

public class STDMatEditor : ShaderGUI {

	//Variables
	Material target;
	MaterialEditor editor;
	MaterialProperty[] properties;
	static GUIContent staticLabel = new GUIContent ();
	static ColorPickerHDRConfig emiss = new ColorPickerHDRConfig(0f,99f,1f /99f, 3f);

	//booleans
	bool showAlphaCut;
	bool showOutline;

	//Enumerators
	enum SmoothSource { Uniform, Albedo, Metal };
	enum RenderMode { Opaque, Cutout, Fade, Transparent, Outline };

	//Render settings
	struct RenderSettings{

		public RenderQueue queue;
		public string renderType;
		public BlendMode srcBlend, dstBlend;
		public bool zWrite;

		public static RenderSettings[] modes = {
			//Opaque
			new RenderSettings (){ 
				queue = RenderQueue.Geometry,
				renderType = "",
				srcBlend = BlendMode.One,
				dstBlend = BlendMode.Zero,
				zWrite = true
			},

			//Cutout
			new RenderSettings (){ 
				queue = RenderQueue.AlphaTest,
				renderType = "TransparentCutout",
				srcBlend = BlendMode.One,
				dstBlend = BlendMode.Zero,
				zWrite = true
			},

			//Fade
			new RenderSettings (){
				queue = RenderQueue.Transparent,
				renderType = "Transparent",
				srcBlend = BlendMode.SrcAlpha,
				dstBlend = BlendMode.OneMinusSrcAlpha,
				zWrite = false
			},

			//Transparent
			new RenderSettings (){
				queue = RenderQueue.Transparent,
				renderType = "Transparent",
				srcBlend = BlendMode.One,
				dstBlend = BlendMode.OneMinusSrcAlpha,
				zWrite = false
			},

			//Outline
			new RenderSettings (){
				queue = RenderQueue.Geometry,
				renderType = "",
				srcBlend = BlendMode.One,
				dstBlend = BlendMode.Zero,
				zWrite = true
			}
		};
	}


	//Override standard GUI
	public override void OnGUI ( MaterialEditor editor, MaterialProperty[] properties ){

		//Initialize
		this.target = editor.target as Material;
		this.editor = editor;
		this.properties = properties;

		//Create Maps and tint
		SetRenderMode();
		SetMaps();
		SetDetail ();
		if (showOutline) {
			SetOutline ();
		}
	}

	//Set rendering mode
	void SetRenderMode(){

		RenderMode mode = RenderMode.Opaque;
		showAlphaCut = false;
		showOutline = false;

		if (IsKeywordEnabled ("_RENDER_CUT")) {
			mode = RenderMode.Cutout;
			showAlphaCut = true;
		} else if (IsKeywordEnabled ("_RENDER_FADE")) {
			mode = RenderMode.Fade;
		} else if (IsKeywordEnabled ("_RENDER_TRANS")) {
			mode = RenderMode.Transparent;			
		} else if (IsKeywordEnabled ("_RENDER_OUTLINE")) {
			mode = RenderMode.Outline;			
			showOutline = true;
		}

		EditorGUI.BeginChangeCheck ();
		mode = (RenderMode)EditorGUILayout.EnumPopup (
			MakeLabel ("Rendering Mode"), mode
		);

		if (EditorGUI.EndChangeCheck ()) {
			RecordAction ("Rendering Mode");
			SetKeyword ("_RENDER_CUT", mode == RenderMode.Cutout);
			SetKeyword ("_RENDER_FADE", mode == RenderMode.Fade);
			SetKeyword ("_RENDER_TRANS", mode == RenderMode.Transparent);
			SetKeyword ("_RENDER_OUTLINE", mode == RenderMode.Outline);

			//Determine render queue and tags
			RenderSettings settings = RenderSettings.modes[(int)mode];
			foreach (Material m in editor.targets) {
				m.renderQueue = (int)settings.queue;
				m.SetOverrideTag ("RenderType", settings.renderType);
				m.SetInt ("_ScrBlend", (int)settings.srcBlend);
				m.SetInt ("_DstBlend", (int)settings.dstBlend);
				m.SetInt ("_ZWrite", settings.zWrite ? 1 : 0);
			}
		}

		if (mode == RenderMode.Fade || mode == RenderMode.Transparent) {
			SetSemiShadows ();
		}
	}

	//Allow semi Transparent shadows
	void SetSemiShadows(){
		EditorGUI.BeginChangeCheck ();
		bool semiShadows = EditorGUILayout.Toggle (
			                   MakeLabel ("Shadows", "Enable Semi-Transparent Shadows"),
			                   IsKeywordEnabled ("_SEMITRANS_SHADOWS")
		                   );

		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_SEMITRANS_SHADOWS", semiShadows);
		}
		if (!semiShadows) {
			showAlphaCut = true;
		}
	}

	//Create maps and tint
	void SetMaps(){

		//Title section
		GUILayout.Label ("PBR Maps", EditorStyles.boldLabel);

		//Set Albedo Map
		MaterialProperty mainTex = FindProperty("_MainTex");
		editor.TexturePropertySingleLine (MakeLabel(mainTex, "Albedo Map"), mainTex, FindProperty("_Tint"));

		//Set additional Map
		if (showAlphaCut) {
			SetAlpha ();
		}
		SetNormal();
		SetOcclusion ();
		SetEmission ();
		SetMetal();
		SetSmooth ();

		//Set scale and offeset
		editor.TextureScaleOffsetProperty (mainTex);
	}

	//Set Alpha cuttoff slider
	void SetAlpha(){

		MaterialProperty sli = FindProperty ("_AlphaCut");
		EditorGUI.indentLevel += 2;
		editor.ShaderProperty (sli, MakeLabel (sli));
		EditorGUI.indentLevel -= 2;

	}

	//Create Normal Maps
	void SetNormal(){
		MaterialProperty map = FindProperty ("_Normal");
		Texture tex = map.textureValue;
		EditorGUI.BeginChangeCheck ();
		editor.TexturePropertySingleLine (
			MakeLabel (map), map, tex ? FindProperty("_NormalScale") : null
		);
		if (EditorGUI.EndChangeCheck () && tex != map.textureValue) {
			SetKeyword ("_NORMAL_MAP", map.textureValue);
		}
	}

	//Occlusion
	void SetOcclusion(){
		MaterialProperty map = FindProperty ("_OccMap");
		Texture tex = map.textureValue;
		EditorGUI.BeginChangeCheck ();
		editor.TexturePropertySingleLine(
			MakeLabel (map, "Occlusion Map"),
			map,
			tex ? null : FindProperty("_OccIntensity")
		);

		if (EditorGUI.EndChangeCheck () && tex != map.textureValue) {
			SetKeyword ("_OCC_MAP", map.textureValue);
		}
	}

	//Create Emission
	void SetEmission(){
		MaterialProperty map = FindProperty ("_EmissionMap");
		Texture tex = map.textureValue;
		EditorGUI.BeginChangeCheck ();
		editor.TexturePropertyWithHDRColor(
			MakeLabel (map, "Emission Map"),
			map,
			tex ? null : FindProperty("_Emission"),
			emiss, false
		);

		if (EditorGUI.EndChangeCheck () && tex != map.textureValue) {
			SetKeyword ("_EMISSION_MAP", map.textureValue);
		}
	}

	//Create Metal / Dielectric slider
	void SetMetal(){
		MaterialProperty map = FindProperty ("_MetalMap");
		Texture tex = map.textureValue;
		EditorGUI.BeginChangeCheck ();
		editor.TexturePropertySingleLine(
			MakeLabel (map, "Metallic Map"),
			map,
			tex ? null : FindProperty("_Metal")
		);

		if (EditorGUI.EndChangeCheck () && tex != map.textureValue) {
			SetKeyword ("_METAL_MAP", map.textureValue);
		}
	}

	//Create Smoothness slider
	void SetSmooth(){

		SmoothSource source = SmoothSource.Uniform;
		if (IsKeywordEnabled ("_SMOOTH_ALBEDO")) {
			source = SmoothSource.Albedo;
		} else if (IsKeywordEnabled ("_SMOOTH_METAL")) {
			source = SmoothSource.Metal;
		}


		MaterialProperty sli = FindProperty ("_Smooth");
		EditorGUI.indentLevel += 2;
		editor.ShaderProperty (sli, MakeLabel (sli));
		EditorGUI.indentLevel += 1;

		EditorGUI.BeginChangeCheck ();
		source = (SmoothSource) EditorGUILayout.EnumPopup ("Source", source);
		if (EditorGUI.EndChangeCheck ()) {
			RecordAction ("Smoothness Source");
			SetKeyword ("_SMOOTH_ALBEDO", source == SmoothSource.Albedo);
			SetKeyword ("_SMOOTH_METAL", source == SmoothSource.Metal);
		}

		EditorGUI.indentLevel -= 3;
	}



	//Create Detail Maps
	void SetDetail(){

		//Title section
		GUILayout.Label ("Texture Maps", EditorStyles.boldLabel);

		//Set bump map
		MaterialProperty bump = FindProperty("_Bump");
		EditorGUI.BeginChangeCheck ();
		editor.TexturePropertySingleLine (MakeLabel(bump, "Bump Map"), bump);

		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_BUMP_MAP", bump.textureValue);
		}


		//set normal map
		SetDetailNormal();
		SetMask ();

		editor.TextureScaleOffsetProperty (bump);
	}

	//Create Detail Normal Map
	void SetDetailNormal(){
		MaterialProperty map = FindProperty ("_BumpNormal");
		Texture tex = map.textureValue;
		EditorGUI.BeginChangeCheck ();
		editor.TexturePropertySingleLine (
			MakeLabel (map), map, tex ? FindProperty("_BumpScale") : null
		);
		if (EditorGUI.EndChangeCheck () && tex != map.textureValue) {
			SetKeyword ("_BUMP_MAP_NORMAL", map.textureValue);
		}
	}

	//Create Metal / Dielectric slider
	void SetMask(){
		MaterialProperty map = FindProperty ("_BumpMask");
		EditorGUI.BeginChangeCheck ();
		editor.TexturePropertySingleLine(MakeLabel (map, "Detail Mask"),	map);

		if (EditorGUI.EndChangeCheck ()) {
			SetKeyword ("_BUMP_MASK", map.textureValue);
		}
	}

	//Create outline
	void SetOutline(){

		//Title section
		GUILayout.Label ("Outline", EditorStyles.boldLabel);

		MaterialProperty rgb = FindProperty ("_Outline");
		MaterialProperty sli = FindProperty ("_OutlineSize");

		EditorGUI.BeginChangeCheck();
		Color outline = EditorGUILayout.ColorField("Outline Color", rgb.colorValue);
		if (EditorGUI.EndChangeCheck () && outline != rgb.colorValue) {

			foreach (Material m in editor.targets) {
				m.SetColor ("_Outline", outline);
			}
		}

		EditorGUI.indentLevel += 2;
		editor.ShaderProperty (sli, MakeLabel (sli));
		EditorGUI.indentLevel += 1;

	}




	//Return Material Property
	MaterialProperty FindProperty (string name){
		return FindProperty (name, properties);
	}

	//Create Label
	static GUIContent MakeLabel (MaterialProperty prop, string tooltip = null){
		staticLabel.text = prop.displayName;
		staticLabel.tooltip = tooltip;
		return staticLabel;
	}

	static GUIContent MakeLabel(string s, string tool = null){
		staticLabel.text = s;
		staticLabel.tooltip = tool;
		return staticLabel;
	}

	//Set Material Keyword
	void SetKeyword(string key, bool state){
		if (state) {
			foreach (Material m in editor.targets) {
				m.EnableKeyword (key);
			}
		} else {
			foreach (Material m in editor.targets) {
				m.DisableKeyword (key);
			}
		}
	}

	//Check if keyword is enabled
	bool IsKeywordEnabled(string key){
		return target.IsKeywordEnabled (key);
	}

	//record action for undo / redo
	void RecordAction(string label){
		editor.RegisterPropertyChangeUndo (label);
	}
}
