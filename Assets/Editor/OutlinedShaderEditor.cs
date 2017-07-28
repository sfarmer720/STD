using UnityEngine;
using UnityEditor;

public class OutlinedShaderEditor : ShaderGUI {

	//Variables
	MaterialEditor editor;
	MaterialProperty[] properties;
	static GUIContent staticLabel = new GUIContent ();

	//GUI OVerride
	public override void OnGUI ( MaterialEditor editor, MaterialProperty[] properties){

		this.editor = editor;
		this.properties = properties;
		MainMaps();
		DetailMaps ();
		SetOutline ();

	}


	void MainMaps(){

		//Albedo Map & Tint
		GUILayout.Label ("Main Maps", EditorStyles.boldLabel);
		MaterialProperty mainTex = FindProperty ("_MainTex");
		editor.TexturePropertySingleLine (
			MakeLabel(mainTex, "Albedo Map / Color (RGB)"),
			mainTex,
			FindProperty ("_albedo")
		);

		//Set Normal map and sliders
		SetNormal ();
		MetallicSlide ();
		SmoothSlide ();

		//tile and offset controls
		editor.TextureScaleOffsetProperty (mainTex);
	}

	//Normal Map
	void SetNormal(){
		MaterialProperty map = FindProperty ("_normal");
		editor.TexturePropertySingleLine (
			MakeLabel (map),
			map,
			map.textureValue ? FindProperty("_normalScale") : null
		);
	}

	//Metallic slider
	void MetallicSlide(){
		MaterialProperty slider = FindProperty ("_metal");
		EditorGUI.indentLevel += 2;
		editor.ShaderProperty (slider, MakeLabel (slider));
		EditorGUI.indentLevel -= 2;
	}

	//smooth slider
	void SmoothSlide(){
		MaterialProperty slider = FindProperty ("_smooth");
		EditorGUI.indentLevel += 2;
		editor.ShaderProperty (slider, MakeLabel (slider));
		EditorGUI.indentLevel -= 2;
	}

	//Detail Maps
	void DetailMaps(){

		//Detail Maps
		GUILayout.Label ("Detail Maps", EditorStyles.boldLabel);

		MaterialProperty detailTex = FindProperty ("_bump");
		editor.TexturePropertySingleLine (MakeLabel(detailTex, "Detail / Bump Map"), detailTex);

		//Secondary Detail Maps
		SecondDetailMaps();

		editor.TextureScaleOffsetProperty (detailTex);

	}

	void SecondDetailMaps(){
		MaterialProperty map = FindProperty ("_bumpNormal");
		editor.TexturePropertySingleLine(
			MakeLabel(map),
			map,
			map.textureValue?FindProperty("_bumpScale") : null
		);
	}


	//Outline Color / Use
	void SetOutline(){


		GUILayout.Label ("Outline", EditorStyles.boldLabel);

		MaterialProperty outlineSize = FindProperty ("_OutSize");
		editor.ShaderProperty (outlineSize, MakeLabel (outlineSize));

		MaterialProperty outlineColor = FindProperty ("_OutColor");
		editor.ShaderProperty (outlineColor, MakeLabel (outlineColor));


	}



	//Get Shader Property
	MaterialProperty FindProperty ( string name ) {
		return FindProperty (name, properties);
	}

	//Set tool tip
	static GUIContent MakeLabel(string text, string tooltip = null) {
		staticLabel.text = text;
		staticLabel.tooltip = tooltip;
		return staticLabel;
	}
	static GUIContent MakeLabel(MaterialProperty property, string tooltip = null) {
		staticLabel.text = property.displayName;
		staticLabel.tooltip = tooltip;
		return staticLabel;
	}

}
