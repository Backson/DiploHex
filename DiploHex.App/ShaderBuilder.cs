using System;
using System.IO;

namespace DiploHex.App
{
    public class ShaderBuilder
    {
        private string? _vertexSource;
        private string? _fragmentSource;

        public ShaderBuilder AddVertexSource(string source)
        {
            _vertexSource = source;
            return this;
        }

        public ShaderBuilder AddVertexFile(string filePath)
        {
            _vertexSource = File.ReadAllText(filePath);
            return this;
        }

        public ShaderBuilder AddFragmentSource(string source)
        {
            _fragmentSource = source;
            return this;
        }

        public ShaderBuilder AddFragmentFile(string filePath)
        {
            _fragmentSource = File.ReadAllText(filePath);
            return this;
        }

        public Shader Build()
        {
            if (string.IsNullOrEmpty(_vertexSource))
                throw new InvalidOperationException("Vertex shader source is not set.");
            if (string.IsNullOrEmpty(_fragmentSource))
                throw new InvalidOperationException("Fragment shader source is not set.");

            return new Shader(_vertexSource, _fragmentSource);
        }
    }
}