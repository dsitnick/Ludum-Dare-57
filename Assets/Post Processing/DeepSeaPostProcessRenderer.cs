using UnityEngine.Rendering.Universal;

[System.Serializable]
public class DeepSeaPostProcessRenderer : ScriptableRendererFeature
{
    DeepSeaPostProcessPass pass;

    public override void Create()
    {
        pass = new DeepSeaPostProcessPass();
    }
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}