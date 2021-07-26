using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using ST_KafkaConsumer.Controllers;
using ST_KafkaConsumer.Models;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using Microsoft.IdentityModel.Protocols;

using System.Linq;

namespace ST_KafkaConsumer.Handlers
{
    public class KafkaConsumerHandler : IHostedService
    {
        private readonly string topic = "lab";
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var optionsBuilder = new DbContextOptionsBuilder<KafkaContext>();
            var options = optionsBuilder
                    .UseSqlServer(@"Data Source=PASHUTA-PC\KAFKASERVER;Initial Catalog=Kafka;Integrated Security=True")
                    .Options;
            //ApplicationContext db = new ApplicationContext(options);

            var conf = new ConsumerConfig
            {
                GroupId = "st_consumer_group",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            using (var builder = new ConsumerBuilder<Ignore,
                string>(conf).Build())
            {
                builder.Subscribe(topic);
                var cancelToken = new CancellationTokenSource();
                try
                {
                    while (true)
                    {
                        var consumer = builder.Consume(cancelToken.Token);

                        try
                        {
                            Temperature t = new Temperature { Value = Int32.Parse(consumer.Message.Value) };
                            Console.WriteLine($"Message: {consumer.Message.Value} received from {consumer.TopicPartitionOffset}");

                            if (t.Value >= 25) { 
                                using (KafkaContext db = new KafkaContext(options))
                                {
                                    db.Temperatures.Add(t);
                                    db.SaveChanges();
                                    Console.WriteLine($"High temperature: {t.Value}!");
                                    Console.WriteLine($"Message recorded to DB!");
                                }
                            }
                        }
                        catch (Exception) 
                        {
                            //Console.WriteLine($"Message: {consumer.Message.Value} received from {consumer.TopicPartitionOffset} was not saved!({ex})");
                        }
                    }
                }
                catch (Exception)
                {
                    builder.Close();
                }
            }
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}