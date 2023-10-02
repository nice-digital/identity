using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Reflection;

namespace NICE.Identity.Test.Infrastructure
{
    public class MockWebHostEnvironment : IWebHostEnvironment
    {
        public IFileProvider WebRootFileProvider { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public string WebRootPath { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public string EnvironmentName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public string ApplicationName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public string ContentRootPath { 
            get { 
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); 
            } 
            set => throw new System.NotImplementedException();
        }

        public IFileProvider ContentRootFileProvider { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    }
}
