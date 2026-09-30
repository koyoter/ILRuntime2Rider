using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: Apartment(ApartmentState.STA)]

namespace ReSharperPlugin.ILRuntime2Rider.Tests
{
    [ZoneDefinition]
    public class ILRuntime2RiderTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>, IRequire<IILRuntime2RiderZone>
    {
    }

    [ZoneMarker]
    public class ZoneMarker : IRequire<ICodeEditingZone>, IRequire<ILanguageCSharpZone>, IRequire<ILRuntime2RiderTestEnvironmentZone>
    {
    }

    [SetUpFixture]
    public class ILRuntime2RiderTestsAssembly : ExtensionTestEnvironmentAssembly<ILRuntime2RiderTestEnvironmentZone>
    {
    }
}