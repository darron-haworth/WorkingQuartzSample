using Quartz;
using System;
using Topshelf;
using Topshelf.Quartz;

namespace mpustelak.TopShelfAndQuartz
{
    class Program
    {
        static void Main(string[] args)
        {
            Topshelf.HostFactory.Run(x =>
            {
                x.Service<MyService>(s =>
                {
                    s.WhenStarted(service => service.OnStart());
                    s.WhenStopped(service => service.OnStop());
                    s.ConstructUsing(() => new MyService());


                    s.ScheduleQuartzJob(q =>
                        q.WithJob(() =>
                            JobBuilder.Create<SecondsJob>().Build())
                            .AddTrigger(() => TriggerBuilder.Create()
                                .WithSimpleSchedule(b => b
                                    .WithIntervalInSeconds(15)
                                    .RepeatForever())
                                .Build()));

                    s.ScheduleQuartzJob(q =>
                        q.WithJob(() =>
                            JobBuilder.Create<MinuteJob>().Build())
                            .AddTrigger(() => TriggerBuilder.Create()
                                .WithSimpleSchedule(b => b
                                    .WithIntervalInMinutes(1)
                                    .RepeatForever())
                                .Build()));
                });

                x.RunAsLocalSystem()
                    .DependsOnEventLog()
                    .StartAutomatically()
                    .EnableServiceRecovery(rc => rc.RestartService(1));

                x.SetServiceName("My Topshelf Service");
                x.SetDisplayName("My Topshelf Service");
                x.SetDescription("My Topshelf Service's description");
            });
        }
    }

    public class MyService
    {
        public void OnStart()
        {
        }

        public void OnStop()
        {
        }
    }

    public class MinuteJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var dt = DateTime.Now;
            Console.WriteLine($"[{DateTime.Now}] Minute job");
        }
    }

    public class SecondsJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            
            Console.WriteLine($"[{DateTime.Now}] 15 seconds job!");
        }
    }
}