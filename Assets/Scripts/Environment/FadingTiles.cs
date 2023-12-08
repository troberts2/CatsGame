using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FadingTiles : MonoBehaviour
{
    private Tilemap tiles;
    private float fadeDur = 0.4f;

    private void Start() {
        tiles = GetComponent<Tilemap>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            StartCoroutine(Fade());
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")){
            StartCoroutine(Fade());
        }
    }

    private IEnumerator Fade()
    {
        Color initialColor = tiles.color;
        Color targetColor;
        if(initialColor.a == 0f){
            targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 255f);
        }else{
            targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
        }

        float elapsedTime = 0f;

        while (elapsedTime < fadeDur)
        {
            elapsedTime += Time.deltaTime;
            tiles.color = Color.Lerp(initialColor, targetColor, elapsedTime / fadeDur);
            yield return null;
        }
    }
}
