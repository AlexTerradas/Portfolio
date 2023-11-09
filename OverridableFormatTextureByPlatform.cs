using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using System.ComponentModel;
using UnityEditor;
using UnityEngine.Experimental.Rendering;
#endif

namespace Utils
{
#if UNITY_EDITOR
	[CustomEditor(typeof(OverridableFormatTextureByPlatform))]
	public class OverridableFormatTextureByPlatformEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			if(GUILayout.Button("Set Textures to Format"))
			{
				((OverridableFormatTextureByPlatform)target).ClearTextures();
				((OverridableFormatTextureByPlatform)target).SetTextures();
				((OverridableFormatTextureByPlatform)target).TextureConfig();
			}
		}
	}

	[System.Serializable]
	public class Condition 
    {
		public string m_Name;
		public TextureImporterTypeRule m_TextureImporterTypeRule;
		public AlphaRule m_AlphaRule;
		public Conversion m_Conversion;
    }

	[System.Serializable]
	public class Conversion
	{
		[SerializeField] public TextureImporterFormat m_TextureType;
	}

	[System.Serializable]
    public class Rule
    {
		virtual public bool IsTrue(Texture _Texture)
		{
			return true;
		}
    }

	[System.Serializable]
	public class TextureImporterTypeRule : Rule
	{
		[SerializeField] public TextureImporterType m_TextureImporterType=new TextureImporterType();
		public TextureImporterTypeRule()
		{
		}
		public override bool IsTrue(Texture _Texture)
		{
			string l_Path=AssetDatabase.GetAssetPath(_Texture);
			TextureImporter l_TextureImporter=AssetImporter.GetAtPath(l_Path) as TextureImporter;
			return m_TextureImporterType==l_TextureImporter.textureType;
		}
	}

	[System.Serializable]
	public class AlphaRule : Rule
	{
		[SerializeField]public bool m_Alpha;
		public AlphaRule()
		{
		}
		public override bool IsTrue(Texture _Texture)
		{
			return m_Alpha==GraphicsFormatUtility.HasAlphaChannel(_Texture.graphicsFormat);
		}
	}

	[CreateAssetMenu(fileName="OverridableFormatTextureByPlatform", menuName="Tools/OverridableFormatTextureByPlatform")]
	public class OverridableFormatTextureByPlatform : ScriptableObject
	{
		public enum TPlatforms {
			Standalone=0,
			Web, 
			iPhone, 
			Android, 
			WebGL,
			Windows_Store_Apps,
			PS4, 
			XboxOne,
			Nintendo_Switch,
			tvOS
		};
		public TPlatforms m_Platform;
		[Header("Rules")]
		[SerializeField] public List<Condition> m_Conditions=new List<Condition>();

		List<Texture> m_Textures=new List<Texture>();
		public void SetTextures()
		{
			foreach(object l_Test in Selection.objects)
            {
				if(!m_Textures.Contains((Texture)l_Test))
					m_Textures.Add((Texture)l_Test);
            }
		}
		public void ClearTextures()
		{
			m_Textures=new List<Texture>();
		}
		public IEnumerator ProgressTextureConfig()
		{
			int l_ProgressBar=Progress.Start("ProgressTextureConfig", "Changing textures configurations...", 0);
			foreach(Texture l_Texture in m_Textures)
			{
				Progress.Report(l_ProgressBar, m_Textures.IndexOf(l_Texture), m_Textures.Count);
				string l_Path=AssetDatabase.GetAssetPath(l_Texture);
				TextureImporter l_TextureImporter=AssetImporter.GetAtPath(l_Path) as TextureImporter;

				foreach(Condition l_Condition in m_Conditions)
				{
					if(l_Condition.m_TextureImporterTypeRule.m_TextureImporterType==l_TextureImporter.textureType)
					{
						TextureImporterPlatformSettings l_TextureImporterPlatformSettings=l_TextureImporter.GetPlatformTextureSettings(m_Platform.ToString().Replace("_", " "));
						
						if(!l_TextureImporterPlatformSettings.overridden)
							l_TextureImporterPlatformSettings.overridden=true;
						
						switch(l_Condition.m_TextureImporterTypeRule.m_TextureImporterType)
						{
							default:
								Debug.Log("Type "+l_Condition.m_TextureImporterTypeRule.m_TextureImporterType+" not implemented");
								break;
							case TextureImporterType.Default:
								if(l_Condition.m_AlphaRule.m_Alpha)
								{
									if(l_TextureImporter.DoesSourceTextureHaveAlpha())
									{
										l_TextureImporterPlatformSettings.format=l_Condition.m_Conversion.m_TextureType;
										Debug.Log("Texture Settings -> Default + Alpha \n");
										Debug.Log("Texture Type -> "+l_Condition.m_Conversion.m_TextureType+"\n\n");
										SaveTexture(l_TextureImporter, l_TextureImporterPlatformSettings);
									}
								} 
								else
								{
									if(!l_Condition.m_AlphaRule.m_Alpha)
									{
										if(!l_TextureImporter.DoesSourceTextureHaveAlpha())
										{
											l_TextureImporterPlatformSettings.format=l_Condition.m_Conversion.m_TextureType;
											Debug.Log("Texture Settings -> Default + No Alpha \n");
											Debug.Log("Texture Type -> "+l_Condition.m_Conversion.m_TextureType+"\n\n");
											SaveTexture(l_TextureImporter, l_TextureImporterPlatformSettings);
										}
									}
								}
								break;
							case TextureImporterType.Sprite:
								if(!l_Condition.m_AlphaRule.m_Alpha)
								{
									if(!l_TextureImporter.DoesSourceTextureHaveAlpha())
									{
										l_TextureImporterPlatformSettings.format=l_Condition.m_Conversion.m_TextureType;
										Debug.Log("Texture Settings -> Sprite + No Alpha \n");
										Debug.Log("Texture Type -> "+l_Condition.m_Conversion.m_TextureType+"\n\n");
										SaveTexture(l_TextureImporter, l_TextureImporterPlatformSettings);
									}
								}
								break;
							case TextureImporterType.NormalMap:
								if(!l_TextureImporter.DoesSourceTextureHaveAlpha()) 
								{
									l_TextureImporterPlatformSettings.format=l_Condition.m_Conversion.m_TextureType;
									Debug.Log("Texture Settings -> NormalMap + No Alpha \n");
									Debug.Log("Texture Type -> "+l_Condition.m_Conversion.m_TextureType+"\n\n");
									SaveTexture(l_TextureImporter, l_TextureImporterPlatformSettings);
								}
								break;
							case TextureImporterType.Lightmap:
								if(l_Condition.m_AlphaRule.m_Alpha)
								{
									if(l_TextureImporter.DoesSourceTextureHaveAlpha())
									{
										l_TextureImporterPlatformSettings.format=l_Condition.m_Conversion.m_TextureType;
										Debug.Log("Texture Settings -> Lightmap + Alpha \n");
										Debug.Log("Texture Type -> "+l_Condition.m_Conversion.m_TextureType+"\n\n");
										SaveTexture(l_TextureImporter, l_TextureImporterPlatformSettings);
									}
								}
								else
								{
									if(!l_TextureImporter.DoesSourceTextureHaveAlpha())
									{
										l_TextureImporterPlatformSettings.format=l_Condition.m_Conversion.m_TextureType;
										Debug.Log("Texture Settings -> Lightmap + No Alpha \n");
										Debug.Log("Texture Type -> "+l_Condition.m_Conversion.m_TextureType+"\n\n");
										SaveTexture(l_TextureImporter, l_TextureImporterPlatformSettings);
									}
								}
								break;
						}
					}
				}
			}
			Progress.Remove(l_ProgressBar);
			yield return null;
		}
		public void SaveTexture(TextureImporter _TextureImporter, TextureImporterPlatformSettings _TextureImporterPlatformSettings)
		{
			_TextureImporter.SetPlatformTextureSettings(_TextureImporterPlatformSettings);
			_TextureImporter.SaveAndReimport();
		}
		public void TextureConfig()
		{
			Coroutiner.StartCoroutine(ProgressTextureConfig());
		}
		void OnEnable() 
		{
			Application.logMessageReceived+=HandleLog;
		}
		void OnDisable() 
		{
			Application.logMessageReceived-=HandleLog;
		}
		void HandleLog(string LogString, string StackTrace, LogType _LogType)
		{
			if(_LogType==LogType.Error)
				Debug.Log("	- Error: "+LogString+"\n");
		}
	}
#endif
}

	