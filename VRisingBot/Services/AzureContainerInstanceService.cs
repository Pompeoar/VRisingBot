using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.ContainerInstance;
using Microsoft.Extensions.Configuration;

namespace VRisingBot.Services
{
    public class AzureContainerInstanceService
    {

        private readonly ArmClient armClient;
        private readonly ResourceIdentifier resource;

        public AzureContainerInstanceService(IConfiguration config)
        {
            armClient = new ArmClient(new ClientSecretCredential(
                config["AZURE_TENANT_ID"],
                config["AZURE_CLIENT_ID"],
                config["AZURE_CLIENT_SECRET"]));
            resource = new ResourceIdentifier($"/subscriptions/{config["AZURE_SUBSCRIPTION_ID"]}/resourceGroups/rg-vrising/providers/Microsoft.ContainerInstance/containerGroups/aci-vrising");
        }

        private async Task<Response<ContainerGroupResource>> GetContainerAsync() => await armClient.GetContainerGroupResource(resource).GetAsync();

        public async Task<string> ExecuteCommand(ContainerCommand command) =>
        command switch
        {
            ContainerCommand.Start => await StartContainerAsync(),
            ContainerCommand.Stop => await StopContainerAsync(),
            ContainerCommand.Restart => await RestartContainerAsync(),
            _ => await GetStateAsync()
        };


        private async Task<string> StartContainerAsync()
        {
            var container = await GetContainerAsync();
            if (container.GetRawResponse().IsError)
            {
                return $"Unable to start container at this time: {container.GetRawResponse().ReasonPhrase}";
            }
            if (!GetStateFromResponse(container).Equals("Terminated"))
            {
                return $"Container is not currently stopped. Current State: {GetStateFromResponse(container)}";
            }
            var result = await container.Value.StartAsync(WaitUntil.Started);
            return "Successfully started. Should take about 5-ish minutes to complete.";
        }

        private async Task<string> StopContainerAsync()
        {
            var container = await GetContainerAsync();
            if (container.GetRawResponse().IsError)
            {
                return $"Unable to start container at this time: {container.GetRawResponse().ReasonPhrase}";
            }
            if (GetStateFromResponse(container).Equals("Terminated"))
            {
                return $"Container is already stopped.";
            }
            var result = await container.Value.StopAsync();

            return result.IsError
                ? "Failed to stop"
                : "Stopped";
        }

        private async Task<string> RestartContainerAsync()
        {
            var container = await GetContainerAsync();
            if (container.GetRawResponse().IsError)
            {
                return $"Unable to restart container at this time: {container.GetRawResponse().ReasonPhrase}";
            }
            if (GetStateFromResponse(container).Equals("Terminated"))
            {
                return $"Container is current stopped. Try !vrising start instead.";
            }
            var result = await container.Value.RestartAsync(WaitUntil.Started);
            return "Restart operation begun. Should take about 5-ish minutes to complete. ";
        }

        private async Task<string> GetStateAsync() => GetStateFromResponse(await GetContainerAsync());
        private static string GetStateFromResponse(Response<ContainerGroupResource> azResponse) => azResponse.Value.Data.Containers.First().InstanceView.CurrentState.State;
    }
}

public enum ContainerCommand
{
    Start,
    Stop,
    Restart,
    State
}


