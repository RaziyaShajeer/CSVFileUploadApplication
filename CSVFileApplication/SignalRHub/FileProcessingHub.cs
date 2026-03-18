using Microsoft.AspNetCore.SignalR;

namespace CSVFileApplication.SignalRHub
{
    public class FileProcessingHub:Hub
    {
		public async Task SendStatusUpdate(int fileId, string status, int processedRows = 0, string filePath = "")
		{
			await Clients.All.SendAsync(
				"ReceiveStatusUpdate",
				fileId,
				status,
				processedRows,
				filePath
			);
		}
    }
}
