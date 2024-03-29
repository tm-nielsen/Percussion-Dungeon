﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColourPresetMenu : MonoBehaviour, RequiresInitialSetup
{
    public List<ColourPreset> presets;

    public Shader colourShader;
    public GameObject colourPreview, colourView, presetButton;
    public Button back;
    //public ColourPresetMaterials materials;

    private ColourSettingsInstance colourSettings;

    private Image[] previewImages, currentImages;
    // Start is called before the first frame update
    public void Setup()
    {
        colourSettings = FindObjectOfType<ColourSettingsInstance>();

        currentImages = colourView.GetComponentsInChildren<Image>();
        previewImages = colourPreview.GetComponentsInChildren<Image>();
        foreach (Image i in previewImages)
            i.material = new Material(colourShader);

        MakePresetOptions();

        UpdatePreview(0);
    }

    private void MakePresetOptions()
    {
        List<Button> buttons = new List<Button>();
        List<Navigation> navs = new List<Navigation>();
        buttons.Add(presetButton.GetComponent<Button>());

        for(int i = 1; i < presets.Count; i++)
        {
            GameObject g = Instantiate(presetButton, transform);
            g.AddComponent<ColourPresetButton>().Initialize(i, UpdatePreview, ApplyPreset, presets[i].name);
            buttons.Add(g.GetComponent<Button>());
        }

        presetButton.AddComponent<ColourPresetButton>().Initialize(0, UpdatePreview, ApplyPreset, presets[0].name);

        Navigation na = new Navigation();
        na.mode = Navigation.Mode.Explicit;
        na.selectOnUp = back;
        na.selectOnDown = buttons[1];
        navs.Add(na);
        for(int i = 1; i< buttons.Count; i++)
        {
            Navigation n = new Navigation();
            n.mode = Navigation.Mode.Explicit;
            n.selectOnUp = buttons[i - 1];
            if (i < buttons.Count - 1)
                n.selectOnDown = buttons[i + 1];
            navs.Add(n);
        }

        for (int i = 0; i < buttons.Count; i++)
            buttons[i].navigation = navs[i];
    }

    public void UpdatePreview(int index)
    {
        if (index >= presets.Count)
            return;

        int i = 0;
        foreach (ColourPair p in presets[index])
        {
            previewImages[i].material.SetColor("_MainCol", p.main);
            previewImages[i++].material.SetColor("_MonoCol", p.mono);
        }
    }

    public void ApplyPreset(int index)
    {
        if (index >= presets.Count)
            return;

        //materials.Apply(presets[index]);
        int i = 0;
        foreach (ColourPair p in presets[index])
        {
            currentImages[i].material.SetColor("_MainCol", p.main);
            currentImages[i++].material.SetColor("_MonoCol", p.mono);
        }

        colourSettings.UpdateColours();
    }
}

//[System.Serializable]
//public class ColourPresetMaterials
//{
//    public Material player, enemy, platform, background, health, objects, corpse;

//    public void Apply(ColourPreset preset)
//    {
//        IEnumerator<Material> it = GetEnumerator();
//        foreach (ColourPair p in preset)
//        {
//            it.MoveNext();
//            ApplyColourPair(it.Current, p);
//        }
//    }

//    private void ApplyColourPair(Material m, ColourPair p)
//    {
//        m.SetColor("_MainCol", p.main);
//        m.SetColor("_MonoCol", p.mono);
//    }
//    public IEnumerator<Material> GetEnumerator()
//    {
//        yield return background;
//        yield return enemy;
//        yield return player;
//        yield return objects;
//        yield return platform;
//        yield return health;
//        yield return corpse;
//    }
//}
