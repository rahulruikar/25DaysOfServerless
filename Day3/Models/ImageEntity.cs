using Microsoft.WindowsAzure.Storage.Table;

namespace Day3.Models
{
    public class ImageEntity : TableEntity
    {
        public ImageEntity()
        {

        }

        public ImageEntity(string owner, string rowKey)
        {
            PartitionKey = owner;
            RowKey = rowKey;
        }

        public string ImageUrl { get; set; }
    }
}
