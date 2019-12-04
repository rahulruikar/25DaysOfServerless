using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Day4
{
    public class Potluck
    {
        public class DishEntity : TableEntity
        {
            public DishEntity(string user, string dishType)
            {
                this.PartitionKey = user;
                this.RowKey = dishType;
            }
            public DishEntity() { }
            public string DishName { get; set; }
        }

        public class Dish
        {
            [JsonProperty(PropertyName ="user")]
            public string User { get; set; }

            [JsonProperty(PropertyName = "dishType")]
            public string DishType { get; set; }

            [JsonProperty(PropertyName = "dish")]
            public string DishName { get; set; }

        }
        [FunctionName("AddUpdateDish")]
        public async Task<Dish> AddOrUpdateDish(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", "post", Route = "dishes")] HttpRequest req,
            [Table("Potluck")] CloudTable cloudTable,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Dish>(requestBody);
            DishEntity dish = new DishEntity(data.User, data.DishType);
            dish.DishName = data.DishName;

            TableOperation addOrUpdateOperation = TableOperation.InsertOrReplace(dish);
            await cloudTable.ExecuteAsync(addOrUpdateOperation);
            return data;
        }

        [FunctionName("GetDishes")]
        public async Task<List<Dish>> GetAllDishes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dishes")] HttpRequest req,
            [Table("Potluck")] CloudTable cloudTable,
            ILogger log)
        {
            List<Dish> records = new List<Dish>();
            TableQuery<DishEntity> getRecordsQuery = new TableQuery<DishEntity>();
            var continuationToken = default(TableContinuationToken);
            TableQuerySegment<DishEntity> segments = await cloudTable.ExecuteQuerySegmentedAsync(getRecordsQuery, continuationToken);
            if (segments != null)
            {
                foreach (DishEntity entity in segments)
                {
                    records.Add(new Dish()
                    {
                        User = entity.PartitionKey,
                        DishType = entity.RowKey,
                        DishName = entity.DishName
                    });
                }
            }

            return records;
        }

        [FunctionName("DeleteDish")]
        public async Task DeleteDish(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "dishes")] HttpRequest req,
            [Table("Potluck")] CloudTable cloudTable,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Dish>(requestBody);
            DishEntity dish = new DishEntity(data.User, data.DishType);
            dish.DishName = data.DishName;
            dish.ETag = "*";

            TableOperation deleteOperation = TableOperation.Delete(dish);
            await cloudTable.ExecuteAsync(deleteOperation);
            return;
        }
    }
}
