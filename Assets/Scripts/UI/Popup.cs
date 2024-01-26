using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Popup : MonoBehaviour
{
    [SerializeField] private float maskOpacity;
    [SerializeField] private float slideSpeed;

    [Header("References")]
    [SerializeField] private Image image;
    [SerializeField] private Image backgroundMask;
    [SerializeField] private PlayerInput input;

    private RectTransform imageTransform;
    private Vector3 initialPopupPosition;
    private IEnumerator slideInHandle;
    private bool slidingIn = false;

    private void Awake()
    {
        imageTransform = image.GetComponent<RectTransform>();
        initialPopupPosition = imageTransform.localPosition;
    }

    internal void DisplayPopup()
    {
        slideInHandle = SlideInPopup();
        StartCoroutine(slideInHandle);
    }

    private IEnumerator SlideInPopup()
    {
        input.SwitchCurrentActionMap("Popup");

        // darken background
        Color targetMaskColor = backgroundMask.color;
        targetMaskColor.a = maskOpacity;

        while (Vector3.Distance(Vector3.zero, imageTransform.localPosition) > 0.1f)
        {
            imageTransform.localPosition = Vector3.Lerp(imageTransform.localPosition, Vector3.zero, Time.deltaTime * slideSpeed);
            backgroundMask.color = Color.Lerp(backgroundMask.color, targetMaskColor, Time.deltaTime * slideSpeed);
            yield return new WaitForEndOfFrame();
        }

        slidingIn = false;
    }

    private IEnumerator SlideOutPopup()
    {
        StopCoroutine(slideInHandle);
        input.SwitchCurrentActionMap("Main");

        // lighten background
        Color targetMaskColor = backgroundMask.color;
        targetMaskColor.a = 0f;

        while (Vector3.Distance(initialPopupPosition, imageTransform.localPosition) > 0.1f)
        {
            imageTransform.localPosition = Vector3.Lerp(imageTransform.localPosition, initialPopupPosition, Time.deltaTime * slideSpeed);
            backgroundMask.color = Color.Lerp(backgroundMask.color, targetMaskColor, Time.deltaTime * slideSpeed);
            yield return new WaitForEndOfFrame();
        }
    }

    public void ClosePopup(InputAction.CallbackContext context)
    {
        if (!context.started || slidingIn)
            return;

        StartCoroutine(SlideOutPopup());
    }
}
