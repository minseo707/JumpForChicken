using UnityEngine;
using System.IO;

public class PrefabCapture : MonoBehaviour
{
    public Camera cameraToCapture;
    public GameObject[] prefabsToCapture;  // 캡처할 프리팹이 씬에 배치된 오브젝트들
    public Vector3 cameraOffset = new Vector3(0, 2, -3); // 프리팹과의 거리 설정

    void Start()
    {
        StartCoroutine(CapturePrefabs());
    }

    System.Collections.IEnumerator CapturePrefabs()
    {
        for (int i = 0; i < prefabsToCapture.Length; i++)
        {
            // 모든 프리팹 비활성화
            foreach (GameObject obj in prefabsToCapture)
            {
                obj.SetActive(false);
            }

            // 현재 프리팹만 활성화
            prefabsToCapture[i].SetActive(true);

            // 프리팹이 위치한 지점으로 카메라 이동
            Transform prefabTransform = prefabsToCapture[i].transform;
            cameraToCapture.transform.position = prefabTransform.position + cameraOffset;
            yield return new WaitForEndOfFrame();

            // 캡처
            CaptureScreenshot(prefabsToCapture[i].name);
        }
    }

    void CaptureScreenshot(string prefabName)
    {
        RenderTexture renderTexture = new RenderTexture(1024, 1024, 24);
        cameraToCapture.targetTexture = renderTexture;

        cameraToCapture.Render();

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D image = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        image.Apply();

        // 이미지 파일로 저장
        byte[] bytes = image.EncodeToPNG();
        File.WriteAllBytes($"Assets/CapturedImages/{prefabName}.png", bytes);

        RenderTexture.active = currentRT;
        cameraToCapture.targetTexture = null;
        Destroy(renderTexture);
    }
}