using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;

namespace ReSharperPlugin.ILRuntime2Rider
{
    [ZoneDefinition]
    // [ZoneDefinitionConfigurableFeature("Title", "Description", IsInProductSection: false)]
    public interface IILRuntime2RiderZone : IZone
    {
    }

    [ZoneMarker]
    public class ZoneMarker : IRequire<IILRuntime2RiderZone>
    {
    }
}