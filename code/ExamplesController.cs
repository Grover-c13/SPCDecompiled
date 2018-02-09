using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CinematicEffects;

public class ExamplesController : MonoBehaviour
{
	public void Start()
	{
		RenderSettings.skybox = this.skyboxMaterial1;
		this.realTimeLight1.SetActive(true);
		this.realTimeLight2.SetActive(false);
		this.realTimeLight3.SetActive(false);
		RenderSettings.customReflection = this.reflectionCubemap1;
		RenderSettings.reflectionIntensity = this.exposure1;
		DynamicGI.UpdateEnvironment();
		this.skyboxSphereActive = this.skyboxSphere1;
		this.currentTargetIndex = 0;
		this.PrepareCurrentObject();
		for (int i = 1; i < this.objectsParams.Length; i++)
		{
			this.objectsParams[i].target.SetActive(false);
		}
		this.hideTime = Time.time + this.hideTimeDelay;
	}

	public void ClickedAutoRotation()
	{
		this.autoRotation = !this.autoRotation;
		this.autorotateButtonOn.SetActive(this.autoRotation);
		this.autorotateButtonOff.SetActive(!this.autoRotation);
	}

	public void ClickedArrow(bool rightFlag)
	{
		this.objectsParams[this.currentTargetIndex].target.transform.rotation = Quaternion.identity;
		this.objectsParams[this.currentTargetIndex].target.SetActive(false);
		if (this.currentRenderer != null && this.originalMaterial != null)
		{
			Material[] sharedMaterials = this.currentRenderer.sharedMaterials;
			sharedMaterials[this.objectsParams[this.currentTargetIndex].submeshIndex] = this.originalMaterial;
			this.currentRenderer.sharedMaterials = sharedMaterials;
			if (!(this.originalMaterial is ProceduralMaterial))
			{
				UnityEngine.Object.DestroyObject(this.currentMaterial);
			}
		}
		if (rightFlag)
		{
			this.currentTargetIndex = (this.currentTargetIndex + 1) % this.objectsParams.Length;
		}
		else
		{
			this.currentTargetIndex = (this.currentTargetIndex + this.objectsParams.Length - 1) % this.objectsParams.Length;
		}
		this.PrepareCurrentObject();
		this.objectsParams[this.currentTargetIndex].target.SetActive(true);
		this.mouseOrbitController.target = this.objectsParams[this.currentTargetIndex].target;
		this.mouseOrbitController.targetFocus = this.objectsParams[this.currentTargetIndex].target.transform.Find("Focus");
		this.mouseOrbitController.Reset();
	}

	public void Update()
	{
		this.skyboxSphereActive.transform.Rotate(Vector3.up, Time.deltaTime * 200f, Space.World);
		if (this.objectsParams[this.currentTargetIndex].Title == "Ice block" && Input.GetKeyDown(KeyCode.L))
		{
			GameObject gameObject = this.objectsParams[this.currentTargetIndex].target.transform.Find("Amber").gameObject;
			gameObject.SetActive(!gameObject.activeSelf);
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			this.ClickedArrow(true);
		}
		else if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			this.ClickedArrow(false);
		}
		if (this.autoRotation)
		{
			this.objectsParams[this.currentTargetIndex].target.transform.Rotate(Vector3.up, Time.deltaTime * this.autoRotationSpeed, Space.World);
		}
		if (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f)
		{
			this.hideTime = Time.time + this.hideTimeDelay;
			this.InteractiveUI.SetActive(true);
		}
		if (Time.time > this.hideTime)
		{
			this.InteractiveUI.SetActive(false);
		}
	}

	public void ButtonPressed(Button button)
	{
		RectTransform component = button.GetComponent<RectTransform>();
		Vector3 v = component.anchoredPosition;
		v.x += 2f;
		v.y -= 2f;
		component.anchoredPosition = v;
	}

	public void ButtonReleased(Button button)
	{
		RectTransform component = button.GetComponent<RectTransform>();
		Vector3 v = component.anchoredPosition;
		v.x -= 2f;
		v.y += 2f;
		component.anchoredPosition = v;
	}

	public void ButtonEnterScale(Button button)
	{
		RectTransform component = button.GetComponent<RectTransform>();
		component.localScale = new Vector3(1.1f, 1.1f, 1.1f);
	}

	public void ButtonLeaveScale(Button button)
	{
		RectTransform component = button.GetComponent<RectTransform>();
		component.localScale = new Vector3(1f, 1f, 1f);
	}

	public void SliderChanged(Slider slider)
	{
		this.mouseOrbitController.disableSteering = true;
		if (this.objectsParams[this.currentTargetIndex].materialProperty == "fallIntensity")
		{
			UBER_GlobalParams component = this.mainCamera.GetComponent<UBER_GlobalParams>();
			component.fallIntensity = slider.value;
		}
		else if (this.objectsParams[this.currentTargetIndex].materialProperty == "_SnowColorAndCoverage")
		{
			Color color = this.currentMaterial.GetColor("_SnowColorAndCoverage");
			color.a = slider.value;
			this.currentMaterial.SetColor("_SnowColorAndCoverage", color);
			slider.wholeNumbers = false;
		}
		else if (this.objectsParams[this.currentTargetIndex].materialProperty == "SPECIAL_Tiling")
		{
			this.currentMaterial.SetTextureScale("_MainTex", new Vector2(slider.value, slider.value));
			slider.wholeNumbers = true;
		}
		else
		{
			this.currentMaterial.SetFloat(this.objectsParams[this.currentTargetIndex].materialProperty, slider.value);
			slider.wholeNumbers = false;
		}
	}

	public void ExposureChanged(Slider slider)
	{
		TonemappingColorGrading component = this.mainCamera.gameObject.GetComponent<TonemappingColorGrading>();
		TonemappingColorGrading.TonemappingSettings tonemapping = component.tonemapping;
		tonemapping.exposure = slider.value;
		component.tonemapping = tonemapping;
	}

	public void ClickedSkybox1()
	{
		this.skyboxSphereActive.transform.rotation = Quaternion.identity;
		Renderer componentInChildren = this.skyboxSphereActive.GetComponentInChildren<Renderer>();
		componentInChildren.sharedMaterial = this.skyboxSphereMaterialInactive;
		this.skyboxSphereActive = this.skyboxSphere1;
		componentInChildren = this.skyboxSphereActive.GetComponentInChildren<Renderer>();
		componentInChildren.sharedMaterial = this.skyboxSphereMaterialActive;
		RenderSettings.customReflection = this.reflectionCubemap1;
		RenderSettings.reflectionIntensity = this.exposure1;
		RenderSettings.skybox = this.skyboxMaterial1;
		this.realTimeLight1.SetActive(true);
		this.realTimeLight2.SetActive(false);
		this.realTimeLight3.SetActive(false);
		DynamicGI.UpdateEnvironment();
	}

	public void ClickedSkybox2()
	{
		this.skyboxSphereActive.transform.rotation = Quaternion.identity;
		Renderer componentInChildren = this.skyboxSphereActive.GetComponentInChildren<Renderer>();
		componentInChildren.sharedMaterial = this.skyboxSphereMaterialInactive;
		this.skyboxSphereActive = this.skyboxSphere2;
		componentInChildren = this.skyboxSphereActive.GetComponentInChildren<Renderer>();
		componentInChildren.sharedMaterial = this.skyboxSphereMaterialActive;
		RenderSettings.customReflection = this.reflectionCubemap2;
		RenderSettings.reflectionIntensity = this.exposure2;
		RenderSettings.skybox = this.skyboxMaterial2;
		this.realTimeLight1.SetActive(false);
		this.realTimeLight2.SetActive(true);
		this.realTimeLight3.SetActive(false);
		DynamicGI.UpdateEnvironment();
	}

	public void ClickedSkybox3()
	{
		this.skyboxSphereActive.transform.rotation = Quaternion.identity;
		Renderer componentInChildren = this.skyboxSphereActive.GetComponentInChildren<Renderer>();
		componentInChildren.sharedMaterial = this.skyboxSphereMaterialInactive;
		this.skyboxSphereActive = this.skyboxSphere3;
		componentInChildren = this.skyboxSphereActive.GetComponentInChildren<Renderer>();
		componentInChildren.sharedMaterial = this.skyboxSphereMaterialActive;
		RenderSettings.customReflection = this.reflectionCubemap3;
		RenderSettings.reflectionIntensity = this.exposure3;
		RenderSettings.skybox = this.skyboxMaterial3;
		this.realTimeLight1.SetActive(false);
		this.realTimeLight2.SetActive(false);
		this.realTimeLight3.SetActive(true);
		DynamicGI.UpdateEnvironment();
	}

	public void TogglePostFX()
	{
		TonemappingColorGrading component = this.mainCamera.gameObject.GetComponent<TonemappingColorGrading>();
		this.togglepostFXButtonOn.SetActive(!component.enabled);
		this.togglepostFXButtonOff.SetActive(component.enabled);
		this.exposureSlider.interactable = !component.enabled;
		component.enabled = !component.enabled;
		Bloom component2 = this.mainCamera.gameObject.GetComponent<Bloom>();
		component2.enabled = component.enabled;
	}

	public void SetTemperatureSun()
	{
		ColorBlock colors = this.buttonSun.colors;
		colors.normalColor = new Color(1f, 1f, 1f, 0.7f);
		this.buttonSun.colors = colors;
		colors = this.buttonFrost.colors;
		colors.normalColor = new Color(1f, 1f, 1f, 0.2f);
		this.buttonFrost.colors = colors;
		UBER_GlobalParams component = this.mainCamera.GetComponent<UBER_GlobalParams>();
		component.temperature = 20f;
	}

	public void SetTemperatureFrost()
	{
		ColorBlock colors = this.buttonSun.colors;
		colors.normalColor = new Color(1f, 1f, 1f, 0.2f);
		this.buttonSun.colors = colors;
		colors = this.buttonFrost.colors;
		colors.normalColor = new Color(1f, 1f, 1f, 0.7f);
		this.buttonFrost.colors = colors;
		UBER_GlobalParams component = this.mainCamera.GetComponent<UBER_GlobalParams>();
		component.temperature = -20f;
	}

	private void PrepareCurrentObject()
	{
		this.currentRenderer = this.objectsParams[this.currentTargetIndex].renderer;
		if (this.currentRenderer)
		{
			this.originalMaterial = this.currentRenderer.sharedMaterials[this.objectsParams[this.currentTargetIndex].submeshIndex];
			if (!(this.originalMaterial is ProceduralMaterial))
			{
				this.currentMaterial = UnityEngine.Object.Instantiate<Material>(this.originalMaterial);
			}
			else
			{
				this.currentMaterial = this.originalMaterial;
			}
			Material[] sharedMaterials = this.currentRenderer.sharedMaterials;
			sharedMaterials[this.objectsParams[this.currentTargetIndex].submeshIndex] = this.currentMaterial;
			this.currentRenderer.sharedMaterials = sharedMaterials;
		}
		bool flag = this.objectsParams[this.currentTargetIndex].materialProperty == null || this.objectsParams[this.currentTargetIndex].materialProperty == string.Empty;
		if (flag)
		{
			this.materialSlider.gameObject.SetActive(false);
		}
		else
		{
			this.materialSlider.gameObject.SetActive(true);
			this.materialSlider.minValue = this.objectsParams[this.currentTargetIndex].SliderRange.x;
			this.materialSlider.maxValue = this.objectsParams[this.currentTargetIndex].SliderRange.y;
			if (this.objectsParams[this.currentTargetIndex].materialProperty == "fallIntensity")
			{
				UBER_GlobalParams component = this.mainCamera.GetComponent<UBER_GlobalParams>();
				this.materialSlider.value = component.fallIntensity;
				component.UseParticleSystem = true;
				this.buttonSun.gameObject.SetActive(true);
				this.buttonFrost.gameObject.SetActive(true);
			}
			else
			{
				UBER_GlobalParams component2 = this.mainCamera.GetComponent<UBER_GlobalParams>();
				component2.UseParticleSystem = false;
				this.buttonSun.gameObject.SetActive(false);
				this.buttonFrost.gameObject.SetActive(false);
				if (this.originalMaterial.HasProperty(this.objectsParams[this.currentTargetIndex].materialProperty))
				{
					if (this.objectsParams[this.currentTargetIndex].materialProperty == "_SnowColorAndCoverage")
					{
						Color color = this.originalMaterial.GetColor("_SnowColorAndCoverage");
						this.materialSlider.value = color.a;
					}
					else
					{
						this.materialSlider.value = this.originalMaterial.GetFloat(this.objectsParams[this.currentTargetIndex].materialProperty);
					}
				}
				else if (this.objectsParams[this.currentTargetIndex].materialProperty == "SPECIAL_Tiling")
				{
					this.materialSlider.value = 1f;
				}
			}
		}
		this.titleTextArea.text = this.objectsParams[this.currentTargetIndex].Title;
		this.descriptionTextArea.text = this.objectsParams[this.currentTargetIndex].Description;
		this.matParamTextArea.text = this.objectsParams[this.currentTargetIndex].MatParamName;
		Vector2 anchoredPosition = this.titleTextArea.rectTransform.anchoredPosition;
		anchoredPosition.y = (float)((!flag) ? 110 : 50) + this.descriptionTextArea.preferredHeight;
		this.titleTextArea.rectTransform.anchoredPosition = anchoredPosition;
	}

	public UBER_ExampleObjectParams[] objectsParams;

	public Camera mainCamera;

	public UBER_MouseOrbit_DynamicDistance mouseOrbitController;

	public GameObject InteractiveUI;

	[Space(10f)]
	public GameObject autorotateButtonOn;

	public GameObject autorotateButtonOff;

	public GameObject togglepostFXButtonOn;

	public GameObject togglepostFXButtonOff;

	public float autoRotationSpeed = 30f;

	public bool autoRotation = true;

	[Space(10f)]
	public GameObject skyboxSphere1;

	public Cubemap reflectionCubemap1;

	[Range(0f, 1f)]
	public float exposure1 = 1f;

	public GameObject realTimeLight1;

	public Material skyboxMaterial1;

	public GameObject skyboxSphere2;

	public Cubemap reflectionCubemap2;

	[Range(0f, 1f)]
	public float exposure2 = 1f;

	public GameObject realTimeLight2;

	public Material skyboxMaterial2;

	public GameObject skyboxSphere3;

	public Cubemap reflectionCubemap3;

	[Range(0f, 1f)]
	public float exposure3 = 1f;

	public GameObject realTimeLight3;

	public Material skyboxMaterial3;

	public Material skyboxSphereMaterialActive;

	public Material skyboxSphereMaterialInactive;

	[Space(10f)]
	public Slider materialSlider;

	public Slider exposureSlider;

	public Text titleTextArea;

	public Text descriptionTextArea;

	public Text matParamTextArea;

	[Space(10f)]
	public Button buttonSun;

	public Button buttonFrost;

	[Space(10f)]
	public float hideTimeDelay = 10f;

	private MeshRenderer currentRenderer;

	private Material currentMaterial;

	private Material originalMaterial;

	private float hideTime;

	private int currentTargetIndex;

	private GameObject skyboxSphereActive;
}
