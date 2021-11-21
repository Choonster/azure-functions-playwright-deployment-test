using AzureFunctionsPlaywrightDeploymentTest;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Startup))]

namespace AzureFunctionsPlaywrightDeploymentTest
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string playwrightMessage;
            try
            {
                Microsoft.Playwright.Program.Main(new[] { "install chromium" });
                playwrightMessage = "playwright install worked";

            }
            catch (Microsoft.Playwright.PlaywrightException ex)
            {
                playwrightMessage = $"PlaywrightException: {ex.Message}";
            }

            var appBinFolder = System.IO.Path.GetDirectoryName(typeof(Startup).Assembly.Location);

            using var process = new System.Diagnostics.Process
            {
                StartInfo = new()
                {
                    FileName = "/bin/bash",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    Arguments = $"-c \"ls -lRaA \\\"{appBinFolder}/..\\\"",
                },
            };

            process.Start();
            process.WaitForExit();

            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();

            throw new Exception($"\n\nPlaywright: {playwrightMessage}\n\nstdout: {stdout}\n\nstderr: {stderr}");
        }
    }
}
