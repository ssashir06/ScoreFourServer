using Microsoft.Azure.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ScoreFourServer.Adapters.Azure.Tools
{
    public class StorageTool
    {
        public static CloudStorageAccount CreateStorageAccount(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Trace.TraceError("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Trace.TraceError("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                throw;
            }

            return storageAccount;
        }
    }
}
