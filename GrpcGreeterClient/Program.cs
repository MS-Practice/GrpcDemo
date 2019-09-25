using Grpc.Net.Client;
using GrpcDemo;
using System;
using System.Threading.Tasks;

namespace GrpcGreeterClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            // 方式1
            var reply = await client.SayHelloAsync(
                new HelloRequest { Name = "GreeterClient" }
                );
            // 方式2：流服务器调用
            Console.WriteLine("Greeting: " + reply.Message);
            Console.WriteLine("请按任意键结束...");
            Console.ReadKey();
        }
    }
}
