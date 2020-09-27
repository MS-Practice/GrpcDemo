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
        // 为什么每次调用创建一个channel会有很大的性能损失
        // 因为每次调用都需要在客户端和服务端之间进行多次网络往返，以创建一个新的HTTP/2连接，主要开销如下：
        // 1：打开一个socket 2：建立TCP连接 3：TLS握手 4：开始HTTP/2连接 5：开始Grpc调用
        // 创建的client是轻量级的，不需要被缓存和重用
        // 一个channel可以创建多个不同类型的client
        // channel创建的client是线程安全的
        // channel创建的client可以同时多次调用
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            // 下面创建一个已经授权的channl,那么经它生成的client就不要再一遍遍写授权代码了。
            var authenticatedChannel = CreateAuthenticatedAChannel("https://localhost:5001");
            var authenticatedClient = new Greeter.GreeterClient(authenticatedChannel);

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

        // 添加需要授权的Grpc调用
        private static string _token = "";
        private static GrpcChannel CreateAuthenticatedAChannel(string address)
        {
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                if (!string.IsNullOrEmpty(_token))
                {
                    metadata.Add("Authorization", $"Bearer {_token}");
                }
                return Task.CompletedTask;
            });

            var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            });

            return channel;
        }
    }
}
