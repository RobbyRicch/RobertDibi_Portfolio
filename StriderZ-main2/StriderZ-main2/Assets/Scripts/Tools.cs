using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
    private static Tools _instance;
    public static Tools Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }

    #region Flashing Objects
    private IEnumerator Flash(Material groundFlashMat, MeshRenderer mesh, float flashTime, float flashTimeDiminisher)
    {
        Material originalMat = mesh.material;
        float timeBetweenFlashes = flashTime / flashTimeDiminisher;

        mesh.material = groundFlashMat;
        yield return new WaitForSeconds(timeBetweenFlashes);

        mesh.material = originalMat;
        yield return new WaitForSeconds(timeBetweenFlashes);

        mesh.material = groundFlashMat;
        yield return new WaitForSeconds(timeBetweenFlashes);

        mesh.material = originalMat;
        yield return new WaitForSeconds(timeBetweenFlashes);

        mesh.material = groundFlashMat;
        yield return new WaitForSeconds(timeBetweenFlashes);

        mesh.material = originalMat;
    }
    public void ActivateFlash(Material groundFlashMat, MeshRenderer mesh, float flashTime, float flashTimeDiminisher)
    {
        StartCoroutine(Flash(groundFlashMat, mesh, flashTime, flashTimeDiminisher));
    }
    private IEnumerator MultipleFlash(Material groundFlashMats, MeshRenderer[] meshes, float flashTime, float flashTimeDiminisher)
    {
        Material[] originalMats = new Material[meshes.Length];

        for (int i = 0; i < originalMats.Length; i++)
            originalMats[i] = meshes[i].material;

        float timeBetweenFlashes = flashTime / flashTimeDiminisher;

        for (int i = 0; i < meshes.Length; i++)
            meshes[i].material = groundFlashMats;

        yield return new WaitForSeconds(timeBetweenFlashes);

        for (int i = 0; i < meshes.Length; i++)
            meshes[i].material = originalMats[i];

        yield return new WaitForSeconds(timeBetweenFlashes);

        for (int i = 0; i < meshes.Length; i++)
            meshes[i].material = groundFlashMats;

        yield return new WaitForSeconds(timeBetweenFlashes);

        for (int i = 0; i < meshes.Length; i++)
            meshes[i].material = originalMats[i];
    }
    public void ActivateMultipleFlash(Material groundFlashMats, MeshRenderer[] meshes, float flashTime, float flashTimeDiminisher)
    {
        StartCoroutine(MultipleFlash(groundFlashMats, meshes, flashTime, flashTimeDiminisher));
    }
    #endregion
}
