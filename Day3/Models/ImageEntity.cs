using Microsoft.WindowsAzure.Storage.Table;

namespace Day3.Models
{
    public class ImageEntity : TableEntity
    {
        public ImageEntity()
        {

        }

        public ImageEntity(string owner, string commitId)
        {
            PartitionKey = owner;
            RowKey = commitId;
        }

        public string ImageUrl { get; set; }
    }
}
