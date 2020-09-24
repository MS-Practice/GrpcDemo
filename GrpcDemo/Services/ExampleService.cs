using System.Threading.Tasks;
using Grpc.Core;

namespace GrpcDemo.Services {
    public class ExampleService : GrpcDemo.ExampleService.ExampleServiceBase {
        public override Task<ExampleResponse> UnaryCall(ExampleRequest request, ServerCallContext context) {
            var response = new ExampleResponse();
            return Task.FromResult(response);
        }
    }
}