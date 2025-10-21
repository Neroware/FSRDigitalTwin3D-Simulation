using System;
using System.Linq;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using UniRx;

namespace FSR.DigitalTwin.Client.Features.DES.SimSharpBridge
{
    /// <summary>
    /// A class for a naive scheduling strategy. Scheduling is a non-trivial problem, 
    /// however, I lack the time to create a proper scheduler. Therefore, I'll use a naive scheduling strategy
    /// that just always selects the first offered method and task disjunction.
    /// <br/><br/>
    /// NOTE: At some point a scheduler could query the ROS2 scheduler from the sharework project...
    /// </summary>
    public class NaiveTaskScheduler : ITaskScheduler
    {
        public IDisposable Schedule(ProcessSimulationBase sim, IProcessSimulationContext ctxt)
        {
            CompositeDisposable disposable = new();
            var prev = Observable.Return(Unit.Default);
            foreach (HRCGoal goal in ctxt.Goals.Keys)
            {
                disposable.Add(ScheduleGoal(goal, prev, sim, ctxt));
                prev = sim.ObserveOnGoalFinished(goal.GoalId).First().AsUnitObservable();
            }
            return disposable;
        }
        public IDisposable ScheduleGoal(HRCGoal goal, ProcessSimulationBase sim, IProcessSimulationContext ctxt)
        {
            var prev = Observable.Return(Unit.Default);
            return ScheduleGoal(goal, prev, sim, ctxt);
        }
        public IDisposable ScheduleMethod(HRCMethod method, ProcessSimulationBase sim, IProcessSimulationContext ctxt)
        {
            var prev = Observable.Return(Unit.Default);
            return ScheduleMethod(method, prev, sim, ctxt);
        }

        private IDisposable ScheduleGoal(HRCGoal goal, IObservable<Unit> previous, ProcessSimulationBase sim, IProcessSimulationContext ctxt)
        {
            // The naive scheduler always selects the first method given!
            var myMethod = ctxt.Goals[goal].First();
            var goalSuccess = sim.ProcessFinished.Where(x => x.Process.ProcessType == EHRCProcessType.Method
                && ((HRCMethod)x.Process).MethodId == myMethod.MethodId)
                    .Select(xs => xs with { Process = goal });
            return new CompositeDisposable()
            {
                previous.Subscribe(_ => sim.Process(goal, goalSuccess)),
                ScheduleMethod(myMethod, previous, sim, ctxt)
            };
        }
        private IDisposable ScheduleMethod(HRCMethod method, IObservable<Unit> previous, ProcessSimulationBase sim, IProcessSimulationContext ctxt)
        {
            CompositeDisposable disposable = new();
            var methodTasks = ctxt.Methods[method].Keys
                .Where(task => !ctxt.Methods[method].Values
                    .Any(x => x.Any(x => x.Any(x => x.TaskId == task.TaskId))));
            var methodTasksObs = methodTasks.Select(subTask => sim.ObserveOnTaskFinished<HRCTask>(subTask.TaskId));
            var methodSuccess = Observable.Zip(methodTasksObs).First().Select(xs => new HRCProcessResult()
            {
                Process = method,
                Outputs = new object[] { 0 },
                Succeeded = xs.All(x => x.Succeeded),
                TimeStamp = sim.Now()
            });
            disposable.Add(previous.Subscribe(_ => sim.Process(method, methodSuccess)));
            var prev = previous;
            foreach(HRCTask task in methodTasks)
            {
                disposable.Add(ScheduleTask(task, method, prev, sim, ctxt));
                prev = sim.ObserveOnTaskFinished<HRCTask>(task.TaskId).First().AsUnitObservable();
            }
            return disposable;
        }
        private IDisposable ScheduleTask(HRCTask task, HRCMethod method, IObservable<Unit> previous, ProcessSimulationBase sim, IProcessSimulationContext ctxt)
        {
            if (task.ProcessType == EHRCProcessType.Function)
            {
                return ScheduleFunction(task as HRCFunction, previous, sim);
            }
            // The naive scheduler just selects the first alternative of a process disjunction
            var subTasks = ctxt.Methods[method][task].First();
            var subTasksObs = subTasks.Select(subTask => sim.ObserveOnTaskFinished<HRCTask>(subTask.TaskId));
            var taskSuccess = Observable.Zip(subTasksObs).First().Select(xs => new HRCProcessResult()
            {
                Process = task,
                Outputs = new object[] { 0 },
                Succeeded = xs.All(x => x.Succeeded),
                TimeStamp = sim.Now()
            });
            CompositeDisposable disposable = new()
            {
                previous.Subscribe(_ => sim.Process(task, taskSuccess))
            };
            if (task.TaskDescription == null || task.TaskDescription.TaskType == EHRCTaskType.Basic)
            {
                var prev = previous;
                foreach (var subTask in subTasks)
                {
                    disposable.Add(ScheduleTask(subTask, method, prev, sim, ctxt));
                    prev = sim.ObserveOnTaskFinished<HRCTask>(subTask.TaskId).First().AsUnitObservable();
                }
            }
            else if (task.TaskDescription.TaskType == EHRCTaskType.Sequential && task.TaskDescription.Constraints.Any(x => x is HRCPrecidenceConstraint))
            {
                var constraint = task.TaskDescription.Constraints.First(x => x is HRCPrecidenceConstraint) as HRCPrecidenceConstraint;
                var function1 = ctxt.Methods[method][task].First().Where(t => t.TaskId == constraint.First).First();
                var function2 = ctxt.Methods[method][task].First().Where(t => t.TaskId == constraint.Second).First();
                disposable.Add(ScheduleTask(function1, method, previous, sim, ctxt));
                disposable.Add(ScheduleTask(function2, method, sim.ObserveOnTaskFinished<HRCFunction>(function1.TaskId)
                    .First().AsUnitObservable(), sim, ctxt));
            }
            else if (task.TaskDescription.TaskType == EHRCTaskType.Sequential)
            {
                var function1 = ctxt.Methods[method][task].First().First();
                var function2 = ctxt.Methods[method][task].First().Skip(1).First();
                disposable.Add(ScheduleTask(function1, method, previous, sim, ctxt));
                disposable.Add(ScheduleTask(function2, method, sim.ObserveOnTaskFinished<HRCFunction>(function1.TaskId)
                    .First().AsUnitObservable(), sim, ctxt));
            }
            else
            {
                foreach (var subTask in subTasks)
                {
                    disposable.Add(ScheduleTask(subTask, method, previous, sim, ctxt));
                }
            }
            return disposable;
            
        }
        private IDisposable ScheduleFunction(HRCFunction function, IObservable<Unit> previous, ProcessSimulationBase sim)
        {
            return previous.Subscribe(_ => sim.Process(function));
        }
    }
}