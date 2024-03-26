using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private Image fadeImage;
    [SerializeField] private Color fadeColor;
    public bool isFadingIn { get; private set; }
    public bool isFadingOut { get; private set; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        fadeColor.a = 0f;
    }
    private void Update()
    {
        if (isFadingOut) {
            if (fadeColor.a < 1) {
                fadeColor.a += Time.deltaTime * fadeSpeed;
                fadeImage.color = fadeColor;
            }
            else{ isFadingOut = false;}
        }
        if (isFadingIn) {
            if (fadeColor.a > 0) {
                fadeColor.a -= Time.deltaTime * fadeSpeed;
                fadeImage.color = fadeColor;
            }
            else { isFadingIn = false; }
        }
    }

    public void StartFadeOut() {
        fadeImage.color = fadeColor;
        isFadingOut = true;
    }
    public void StartFadeIn() {
        if (fadeColor.a >= 1) {
            fadeImage.color = fadeColor;
            isFadingIn = true;
        }
        
    }
}
