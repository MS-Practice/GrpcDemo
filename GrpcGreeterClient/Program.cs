using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcDemo;
using System;
using System.Threading.Tasks;

namespace GrpcGreeterClient
{
    /// <summary>
    /// 创建Channel是很昂贵的操作,一旦创建,它是长连接,所以可以重用.应该将它缓存起来(单例)
    /// 由Channel创建的客户端Client,这是一个轻量的对象,不需要缓存和重新.
    /// 单个channel可以创建不同类型的client
    /// </summary>
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


            //var payload = new PayloadResponse();
            var status = new GrpcDemo.Status();
            status.Detail = Any.Pack(new HelloRequest() { Name = "marsonshine" });
            if (status.Detail.Is(HelloRequest.Descriptor))
            {

            }

            // oneof
            try
            {
                var oneofResult = await client.OneofReturnValueAsync(new Empty(), deadline: DateTime.UtcNow.AddSeconds(5));  // 还可以设置deadline,更有利于系统的伸缩性
                switch (oneofResult.ResultCase)
                {
                    case ResponseMessage.ResultOneofCase.Person:
                        break;
                    case ResponseMessage.ResultOneofCase.Error:
                        break;
                    default:
                        break;
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
            {
                Console.WriteLine("Timeout!");
            }
            Console.WriteLine("Greeting: " + reply.Message);
            Console.WriteLine("请按任意键结束...");
            Console.ReadKey();
        }
    }
}
