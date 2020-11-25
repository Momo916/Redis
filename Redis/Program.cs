using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ServiceStack;
using ServiceStack.Redis;

namespace Redis
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Redis rd= new Redis();
            rd.RedisPractise();
        }

      
    }
    class Redis 
    {
        public void RedisPractise()
        {
            // Connect Redis
            RedisClient client = new RedisClient("127.0.0.1", 6379);

            // Clean all
            client.FlushAll();

            #region String Action
            client.Set<string>("key1", "mytest1"); //Set
            client.Set<string>("key1", "mytest2"); //Update
            Console.WriteLine(client.Get<string>("key1")); //Get
            #endregion

            #region List Action
            client.AddItemToList("IdList", "001"); //Add
            client.AddItemToList("IdList", "002"); //Add
            client.AddItemToList("IdList", "003"); //Add
            client.RemoveItemFromList("IdList", "002"); //Remove
            Console.WriteLine(Encoding.UTF8.GetString(client.LIndex("IdList", 1))); //Get by index

            long llen = client.LLen("IdList"); //List Length
            Console.WriteLine($"length: {llen}");
            List<string> idList = client.GetRangeFromList("IdList", 0, (int)llen); //List Range
            Loop(idList); //loop through

            client.PushItemToList("StackList", "stack001"); //Push - FILO
            client.PushItemToList("StackList", "stack002"); //Push - FILO
            client.PushItemToList("StackList", "stack003"); //Push - FILO
            Console.WriteLine($"Pop: {client.PopItemFromList("StackList")}"); //Pop
            List<string> stackList = client.GetRangeFromList("StackList", 0, (int)client.LLen("StackList"));
            Loop(stackList);

            client.EnqueueItemOnList("QueueList", "queue01"); //Enqueue - FIFO
            client.EnqueueItemOnList("QueueList", "queue02"); //Enqueue - FIFO 
            client.EnqueueItemOnList("QueueList", "queue03"); //Enqueue - FIFO 
            Console.WriteLine($"Dequeue: {client.DequeueItemFromList("QueueList")}"); //Dequeue
            List<string> queueList = client.GetRangeFromList("QueueList", 0, (int)client.LLen("QueueList"));
            Loop(queueList); //loop through
            #endregion

            #region Sets
            client.AddItemToSet("Sets1", "user001"); //Add
            client.AddItemToSet("Sets1", "user002"); //Add
            client.AddItemToSet("Sets1", "user003"); //Add
            client.AddItemToSet("Sets2", "user001"); //Add
            client.AddItemToSet("Sets2", "user003"); //Add
            client.AddItemToSet("Sets2", "user004"); //Add
            var intersectSets = client.GetIntersectFromSets("Sets1", "Sets2"); //Intersect
            var unionSets = client.GetUnionFromSets("Sets1", "Sets2"); //Union
            Console.WriteLine("intersect Sets:");
            Loop(intersectSets);
            Console.WriteLine("union Sets:");
            Loop(unionSets);
            #endregion

            #region Sorted Sets
            client.AddItemToSortedSet("ScoreSortedSets", "user001", 199);
            client.AddItemToSortedSet("ScoreSortedSets", "user002", 23);
            client.AddItemToSortedSet("ScoreSortedSets", "user003", 1223);
            client.AddItemToSortedSet("ScoreSortedSets", "user004", 188);
            client.AddItemToSortedSet("ScoreSortedSets", "user005", 588);
            client.IncrementItemInSortedSet("ScoreSortedSets", "user004", 100);
            var highScore = client.GetRangeWithScoresFromSortedSetByHighestScore("ScoreSortedSets", 500, 9999);
            foreach (var score in highScore)
            {
                Console.WriteLine($"highScore:{score.Key}, score:{score.Value}");
            }
            var lowest3 = client.GetRangeWithScoresFromSortedSet("ScoreSortedSets", 0, 2);
            foreach (var score in lowest3)
            {
                Console.WriteLine($"lowest3:{score.Key}, score:{score.Value}");
            }
            #endregion

            #region Hash
            Dictionary<string, string> configDic = new Dictionary<string, string>();
            configDic.Add("config_userName","Kelly");
            configDic.Add("config_password","123456");
            configDic.Add("config_IP","localhost");
            client.SetRangeInHash("configInfo",configDic);
            client.SetEntryInHash("configInfo", "config_serviceName", "Venus");
            var allInfo = client.GetAllEntriesFromHash("configInfo");
            foreach (var config in allInfo)
            {
                Console.WriteLine($"configInfo--{config.Key}:{config.Value}");
            }
            #endregion

            Console.ReadLine();
        }

        private void Loop(IEnumerable<string> list)
        {
            foreach (var id in list) //loop through
            {
                Console.WriteLine(id);
            }
        }
    }
}

