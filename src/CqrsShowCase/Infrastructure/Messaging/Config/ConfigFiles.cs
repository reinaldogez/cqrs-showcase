using Confluent.Kafka;

namespace CqrsShowCase.Infrastructure.Messaging.Config;

public static class ConfigFiles
{
    public static async Task<T> LoadConfig<T>(string configPath) where T : ClientConfig
    {
        try
        {
            var configDictionary = (await File.ReadAllLinesAsync(configPath))
                .Where(line => !line.StartsWith("#"))
                .ToDictionary(
                    line => line.Substring(0, line.IndexOf('=')),
                    line => line.Substring(line.IndexOf('=') + 1));

            return (T)Activator.CreateInstance(typeof(T), configDictionary);

        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occured reading the config file from '{configPath}': {e.Message}");
            return null;
        }
    }

    public static async Task<ProducerConfig> LoadConfigProducer(string configPath, string certDir)
    {
        try
        {
            var cloudConfig = (await File.ReadAllLinesAsync(configPath))
                .Where(line => !line.StartsWith("#"))
                .ToDictionary(
                    line => line.Substring(0, line.IndexOf('=')),
                    line => line.Substring(line.IndexOf('=') + 1));

            var producerConfig = new ProducerConfig(cloudConfig);
            if (certDir != null)
            {
                producerConfig.SslCaLocation = certDir;
            }

            return producerConfig;
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occured reading the config file from '{configPath}': {e.Message}");
            return null;
        }
    }

}