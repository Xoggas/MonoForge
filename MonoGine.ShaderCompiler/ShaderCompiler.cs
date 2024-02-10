using System;
using System.Diagnostics;
using System.IO;

namespace MonoGame.Effect.Compiler;

public sealed class ShaderCompiler : IDisposable
{
    private static EffectCompilerOutput s_output = new();

    public void Dispose()
    {
        s_output = null;
    }

    public static void CompileAllShaders(string path)
    {
        foreach (var shaderFilePath in Directory.EnumerateFiles(path, "*.fx", SearchOption.AllDirectories))
        {
            Compile(shaderFilePath);
        }
    }

    private static void Compile(string path)
    {
        var options = new Options
        {
            SourceFile = path,
            OutputFile = Path.ChangeExtension(path, "shader")
        };

        GetShaderBytecode(options);
    }

    private static void GetShaderBytecode(Options options)
    {
        using FileStream stream = File.OpenWrite(options.OutputFile);
        using var writer = new BinaryWriter(stream);
        CompileShader(options).Write(writer, options);
    }

    private static EffectObject CompileShader(Options options)
    {
        return EffectObject.CompileEffect(GetShaderResult(options), out _);
    }

    private static ShaderResult GetShaderResult(Options options)
    {
        ShaderResult result = ShaderResult.FromFile(options.SourceFile, options, s_output);
        return result;
    }

    private class EffectCompilerOutput : IEffectCompilerOutput
    {
        public void WriteWarning(string file, int line, int column, string message)
        {
            Debug.Print($"Warning: {file}({line},{column}): {message}");
        }

        public void WriteError(string file, int line, int column, string message)
        {
            throw new Exception($"Error: {file}({line},{column}): {message}");
        }
    }
}