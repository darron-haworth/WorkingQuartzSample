using Quartz;
using System;
using System.Diagnostics;
using Topshelf;
using Topshelf.Quartz;

namespace mpustelak.TopShelfAndQuartz
{
    class Program
    {
        int _firstCount = 0;
        public int firstCount
        {
            get { return _firstCount; }
            set { _firstCount = value; }
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            Topshelf.HostFactory.Run(x =>
            {
                x.UseLog4Net();
                x.Service<MyService>(s =>
                {
                    s.WhenStarted(service => service.OnStart());
                    s.WhenStopped(service => service.OnStop());
                    s.ConstructUsing(() => new MyService());

                    s.ScheduleQuartzJob(q =>
                        q.WithJob(() =>
                            JobBuilder.Create<MinuteJob>().Build())
                            .AddTrigger(() => TriggerBuilder.Create()
                                .WithSimpleSchedule(b => b
                                    .WithIntervalInMinutes(1)
                                    .RepeatForever())
                                .Build()));

                    s.ScheduleQuartzJob(q =>
                        q.WithJob(() =>
                            JobBuilder.Create<SecondsJob>().Build())
                            .AddTrigger(() => TriggerBuilder.Create()
                                .WithSimpleSchedule(b => b
                                    .WithIntervalInSeconds(15)
                                    .RepeatForever())
                                .Build()));
                });

                x.RunAsLocalSystem()
                    .DependsOnEventLog()
                    .StartAutomatically()
                    .EnableServiceRecovery(rc => rc.RestartService(1));

                x.SetServiceName("ApiGwLogHarvester");
                x.SetDisplayName("Api Gateway Log Harvester");
                x.SetDescription("Topshelf service that harvests audit logs from Api Gateway APIs using Quartz.net for scheduling");
            });
        }
    }

    public class MyService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void OnStart()
        {
            log.Warn("MyService - SVC Start");
        }

        public void OnStop()
        {
            log.Warn("MyService - SVC Stop");
        }
    }

    public  class MinuteJob : IJob
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {            
            var dt = DateTime.Now;
            var line_to_write = $"[{dt}] Minute job";
            Console.WriteLine(line_to_write);
            log.Info(line_to_write);
        }
    }

    public class SecondsJob : IJob
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(IJobExecutionContext context)
        {
            var strLine = $"[{DateTime.Now}] 15 seconds job!";
            Console.WriteLine(strLine);
            log.Info(strLine);
        }
    }
}