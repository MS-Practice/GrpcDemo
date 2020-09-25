using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace GrpcDemo.Services {
    public class ExampleService : GrpcDemo.ExampleService.ExampleServiceBase {
        public override Task<ExampleResponse> UnaryCall (ExampleRequest request, ServerCallContext context) {
            var response = new ExampleResponse ();
            return Task.FromResult (response);
        }

        public override async Task StreamingFromServer (ExampleRequest request, IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context) {
            for (var i = 0; i < 5; i++) {
                // È¡Ïû²Ù×÷
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    await responseStream.WriteAsync(new ExampleResponse());
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }

        public override async Task<ExampleResponse> StreamingFromClient(IAsyncStreamReader<ExampleRequest> requestStream, ServerCallContext context)
        {
            await foreach (var message in requestStream.ReadAllAsync())
            {

            }
            return new ExampleResponse();
        }

        public override async Task StreamingBothWays(IAsyncStreamReader<ExampleRequest> requestStream,
    IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            await foreach (var message in requestStream.ReadAllAsync())
            {
                await responseStream.WriteAsync(new ExampleResponse());
            }
        }
    }
}