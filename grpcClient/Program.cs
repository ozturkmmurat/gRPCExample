using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using grpcMessageClient;
using grpcServer;

namespace grpClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5272");
            var messageClient = new Message.MessageClient(channel);

            //Unary
            // MessageResponse response = await messageClient.SendMessageAsync(new MessageRequest{
            //     Message = "Merhaba",
            //     Name = "Murat"
            // });

            //System.Console.WriteLine(response.Message);

            //Server Streaming
            // var response = messageClient.SendMessage(new MessageRequest{
            //     Message = "Merhaba",
            //     Name = "Murat"
            // });

             CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            // while(await response.ResponseStream.MoveNext(cancellationTokenSource.Token)){
            //     System.Console.WriteLine(response.ResponseStream.Current.Message);
            // }

            // //Client Streaming
            // var request =  messageClient.SendMessage();
            // for (int i = 0; i < 10; i++)
            // {
            //     await Task.Delay(1000);
            //     await request.RequestStream.WriteAsync(new MessageRequest()
            //     {
            //         Name = "Murat",
            //         Message = "Mesaj " + i
            //     });
            // }

            // //Stream datanın sonlandigini ifade eder.
            // await request.RequestStream.CompleteAsync();

            // System.Console.WriteLine((await request.ResponseAsync).Message);

            // Bi - Directional Streaming
            var request = messageClient.SendMessage();
            var task1 = Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                await Task.Delay(1000);
                request.RequestStream.WriteAsync(new MessageRequest { Name = "Murat", Message = "Mesaj " + i});
                }
            });

            while (await request.ResponseStream.MoveNext(cancellationTokenSource.Token))
            {
                System.Console.WriteLine(request.ResponseStream.Current.Message);
            }

            await task1;
            await request.RequestStream.CompleteAsync();

        }
    }
}
