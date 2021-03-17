using System;
using System.Reflection;

namespace NICE.Identity.TestClient.API.NETFramework4._5._2.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}