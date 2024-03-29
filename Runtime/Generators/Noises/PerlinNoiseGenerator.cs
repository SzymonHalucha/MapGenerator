using UnityEngine;
using SH.MapGenerator.GPUBuffers;
using SH.MapGenerator.CPUBuffers;
using SH.MapGenerator.Utils;

namespace SH.MapGenerator.Generators.Noises
{
    public class PerlinNoiseGenerator : BaseGenerator
    {
        [SerializeField] private Bounds1DArrayCPUBuffer boundsBuffer = null;
        [SerializeField] private Float2DArrayGPUBuffer targetBuffer = null;
        [SerializeField, Range(1, 16)] private int octaves = 4;
        [SerializeField, Range(0, 512f)] private float scale = 30f;
        [SerializeField, Range(0, 1f)] private float persistence = 0.5f;
        [SerializeField, Range(0, 16f)] private float lacunarity = 2f;

        public override void Generate(RuntimeData data)
        {
            if (boundsBuffer != null && boundsBuffer.Size == 0)
                return;

            ComputeShader shader = ComputeShadersContrainer.GetShader("Perlin");
            int kernel = shader.FindKernel("Perlin");

            shader.SetInt("_Size", targetBuffer.Width);
            shader.SetInt("_Seed", data.Random.Range(int.MaxValue));
            shader.SetInt("_Octaves", octaves);
            shader.SetFloat("_Scale", scale);
            shader.SetFloat("_Persistence", persistence);
            shader.SetFloat("_Lacunarity", lacunarity);
            shader.SetVector("_StartOffset", boundsBuffer != null ? boundsBuffer.StartOffset : Vector2.zero);
            shader.SetBuffer(kernel, "TargetBuffer", targetBuffer.Buffer);

            if (boundsBuffer != null)
                DispatchComputeShader(shader, kernel, boundsBuffer.Size, boundsBuffer.Size);
            else
                DispatchComputeShader(shader, kernel, targetBuffer.Width, targetBuffer.Height);
        }

        public override BaseGPUBuffer[] GetAllGPUBuffers()
        {
            return new BaseGPUBuffer[] { targetBuffer };
        }

        public override BaseCPUBuffer[] GetAllCPUBuffers()
        {
            return new BaseCPUBuffer[] { boundsBuffer };
        }
    }
}