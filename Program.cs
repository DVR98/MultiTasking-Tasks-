using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTasking_Tasks_
{
    class Program
    {
        static void Main(string[] args)
        {
            //Provide options for user
            Console.WriteLine("Enter 1 to run the BasicTask function...");
            Console.WriteLine("Enter 2 to run the ReturnValue function...");
            Console.WriteLine("Enter 3 to run the Cancel, Fault or Complete Task function...");
            Console.WriteLine("Enter 4 to run the Parent Children Task(Async) function...");
            Console.WriteLine("Enter 5 to run the Task factory function(Sync) function...");
            Console.WriteLine("Enter 6 to run the Keep Track of Tasks function...");

            //Get user input
            var input = Console.ReadLine();
            try 
            {
                switch(input)
                {
                    //Basic Task function
                    case "1":
                    {
                        BasicTask();
                        break;
                    }
                    //Return Value Task
                    case "2":
                    {
                        ReturnValueTask();
                        break;
                    }
                    //Cancel, Fault or Complete Task function
                    case "3":
                    {
                        CancelFaultCompleteTasks();
                        break;
                    }
                    //Parent Children Task function(Async)
                    case "4":
                    {
                        ParentChildrenTasks();
                        break;
                    }
                    //Task factory function(Sync)
                    case "5":
                    {
                        TaskFactory();
                        break;
                    }
                    //Keep Track of Tasks function
                    case "6":
                    {
                        KeepTrackOfTasks();
                        break;
                    }
                    //If entered number doesn't match, Display this message
                    default: 
                    {
                        Console.WriteLine("The value entered doesn't correspond with any of the given options....");
                        break;
                    }
                }
            }
            //Catch any exception that occurs and handle it
            catch(Exception e) 
            {
                Console.WriteLine("Exception caught: {0}", e.Message);
                Console.WriteLine("App will now be terminated....sorry :(");
            }
            //Goodbye message before App is terminated
            finally
            {
                Console.WriteLine("Thank you. Have a good one");
            }
        }

        //Basic Task
        public static void BasicTask()
        {
            Task t = Task.Run(()=>
            {
                //Write 1-100 in console
                for (int i = 0; i < 100; i++)
                {
                    Console.Write("{0}, ", i);
                }
            });

            //Wait untill Task is finished before terminating app
            t.Wait();
        }

        //Return Value function
        public static void ReturnValueTask()
        {
            //Return value through Task
            Task<int> t = Task.Run(() => {
                return 42;
            })
            //Multiply value returned from task
            .ContinueWith((i) => {
                return i.Result * 10;
            });

            //Wait untill Task is finished before terminating app
            t.Wait();

            //Display value
            Console.WriteLine("{0}", t.Result);
        }

        //Cancel, Fault and Complete Task function
        public static void CancelFaultCompleteTasks()
        {
            //Return value through Task
            Task<int> t = Task.Run(() => {
                return 42;
            });

            //Cancel Task
            t.ContinueWith((i) => {
                Console.WriteLine("Task Canceled");
            }, TaskContinuationOptions.OnlyOnCanceled);

            //Fault Task
            t.ContinueWith((i) => {
                Console.WriteLine("Task Faulted");
            }, TaskContinuationOptions.OnlyOnFaulted);

            //Cancel Task
            var completedTask = t.ContinueWith((i) => {
                Console.WriteLine("Task Completed");
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            //Wait untill Task is finished before terminating app
            completedTask.Wait();
        }
        
        //Parent Children Task function(Async)
        public static void ParentChildrenTasks() 
        {
            //Parent Task
            Task<Int32[]> parent = Task.Run(() => {
                var results = new Int32[3];
                //ChildTask 1
                new Task(() => results[0] = 0, TaskCreationOptions.AttachedToParent).Start();
                //ChildTask2
                new Task(() => results[1] = 1, TaskCreationOptions.AttachedToParent).Start();
                //ChildTask3
                new Task(() => results[2] = 2, TaskCreationOptions.AttachedToParent).Start();

                return results;
            });

            //Final Task
            var finalTask = parent.ContinueWith(
              parentTask => {
                  //Display Results
                  foreach(int i in parentTask.Result){
                      Console.Write("{0}, ", i);
                  }
              }
            );

            //Wait untill Task is finished before terminating app
            finalTask.Wait();
        } 

        //Task factory function(Sync)
        public static void TaskFactory() 
        {
            //Parent Task
            Task<Int32[]> parent = Task.Run(() => {
                var results = new Int32[3];

                //Task Factory(Allows Tasks to be deployed synchronously)
                TaskFactory tf = new TaskFactory(TaskCreationOptions.AttachedToParent, TaskContinuationOptions.ExecuteSynchronously);

                //ChildTask 1
                new Task(() => results[0] = 0, TaskCreationOptions.AttachedToParent).Start();
                //ChildTask2
                new Task(() => results[1] = 1, TaskCreationOptions.AttachedToParent).Start();
                //ChildTask3
                new Task(() => results[2] = 2, TaskCreationOptions.AttachedToParent).Start();

                return results;
            });

            //Final Task
            var finalTask = parent.ContinueWith(
              parentTask => {
                  //Display Results
                  foreach(int i in parentTask.Result){
                      Console.Write("{0}, ", i);
                  }
              }
            );

            //Wait untill Task is finished before terminating app
            finalTask.Wait();
        } 

        public static void KeepTrackOfTasks(){
            //Task array
            Task<int>[] tasks = new Task<int>[3];

            //Task 1
            tasks[0] = Task.Run(() => 
            { 
                Thread.Sleep(1000); 
                return 1;
            });
            //Task 2
            tasks[1] = Task.Run(() => 
            { 
                Thread.Sleep(1000); 
                return 2;
            });
            //Task 3
            tasks[2] = Task.Run(() => 
            { 
                Thread.Sleep(1000); 
                return 3;
            });

            //Display results for completed tasks asynchronously
            while(tasks.Length > 0){
                //Wait till any task is finished
                int i = Task.WaitAny(tasks);
                //Store result in Task array
                Task<int> completedTasks = tasks[i];

                //Display results of completed functions
                Console.WriteLine("Task {0} completed. Result: {1}", completedTasks.Result, completedTasks.Result);

                //Manage uncompleted and completed task
                var temp = tasks.ToList();
                temp.RemoveAt(i);
                tasks = temp.ToArray();
            }
        }


    }
}
