
namespace MajiirKerbalLib
{
    internal interface IEngine
    {
        bool EngineEnabled { get; }
        float RealIsp { get; }
        float MaxThrust { get; }
    }
}
