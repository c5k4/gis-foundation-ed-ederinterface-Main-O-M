using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

#if SILVERLIGHT
using Microsoft.Silverlight.Testing;
using Miner.Server.Client.Query;
using Miner.Server.Client.Tasks;
#elif WPF
using Miner.Mobile.Client.Query;
using Miner.Mobile.Client.Tasks;
#endif

using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

#if SILVERLIGHT
namespace Miner.Server.Client.UnitTests
#elif WPF
namespace Miner.Mobile.Client.UnitTests
#endif
{
    [TestClass]
    public class SearchTests
    {
        private MockRepository _mocks;

        [TestInitialize]
        public void TestSetup()
        {
            _mocks = new MockRepository();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _mocks.VerifyAll();
            _mocks = null;
        }

        [TestMethod]
        public void Constructor_NoExceptions()
        {
            Search search = new Search();
            Assert.IsNotNull(search);
        }

        [TestMethod]
        [Tag("Search")]
        public void LocateAsync_NoSearchLayers_EmptyResults()
        {
            Search search = new Search();
            IEnumerable<IResultSet> results = null;

            search.LocateComplete += (s, e) => results = e.Results;
            search.LocateAsync("test");

            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count(), 0);
        }

        [TestMethod]
        [Tag("Search")]
        public void LocateAsync_TaskReturnsEmptyResult_SearchReturnsEmptyResult()
        {
            Search search = new Search();
            search.SearchLayers.Add(new SearchLayer { ID = 1, Url = "Dummy" });

            ILocateTask task = _mocks.DynamicMock<ILocateTask>();

            task.ExecuteCompleted += null;      // create an expectation that someone will subscribe to this event
            LastCall.IgnoreArguments();         // we don't care who is subscribing
            IEventRaiser raiser = LastCall.GetEventRaiser();

            ITaskFactory<ILocateTask> factory = new TaskFactoryStub<ILocateTask>(new List<ILocateTask> { task });
            search.TaskFactory = factory;

            _mocks.ReplayAll();
            IEnumerable<IResultSet> results = null;
            search.LocateComplete += (s, e) => { results = e.Results; };
            search.LocateAsync("bob");
            raiser.Raise(task, new TaskResultEventArgs(new List<IResultSet>(), null));
            Assert.IsNotNull(results);
        }

        [TestMethod]
        [Tag("Search")]
        public void LocateAsync_TaskReturnsNonEmptyResult_SearchReturnsNonEmptyResult()
        {
            Search search = new Search();
            search.SearchLayers.Add(new SearchLayer { ID = 1, Url = "Dummy" });

            ILocateTask task = _mocks.DynamicMock<ILocateTask>();

            task.ExecuteCompleted += null;      // create an expectation that someone will subscribe to this event
            LastCall.IgnoreArguments();         // we don't care who is subscribing
            IEventRaiser raiser = LastCall.GetEventRaiser();

            ITaskFactory<ILocateTask> factory = new TaskFactoryStub<ILocateTask>(new List<ILocateTask> { task });
            search.TaskFactory = factory;

            _mocks.ReplayAll();
            IEnumerable<IResultSet> results = null;
            search.LocateComplete += (s, e) => { results = e.Results; };
            search.LocateAsync("bob");
            raiser.Raise(task, new TaskResultEventArgs(new List<IResultSet> { new ResultSet() }, null));
            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count(), 1);
        }

        [TestMethod]
        [Tag("Search")]
        public void LocateAsync_MoreThanOneSearchLayer_SearchCompletesAfterAllTasksComplete()
        {
            Search search = new Search();
            search.SearchLayers.Add(new SearchLayer { ID = 1, Url = "Dummy" });
            search.SearchLayers.Add(new SearchLayer { ID = 2, Url = "Dummy2" });

            ILocateTask task = _mocks.DynamicMock<ILocateTask>();
            task.ExecuteCompleted += null;      // create an expectation that someone will subscribe to this event
            LastCall.IgnoreArguments();         // we don't care who is subscribing
            IEventRaiser raiser = LastCall.GetEventRaiser();

            ILocateTask task2 = _mocks.DynamicMock<ILocateTask>();
            task2.ExecuteCompleted += null;      // create an expectation that someone will subscribe to this event
            LastCall.IgnoreArguments();         // we don't care who is subscribing
            IEventRaiser raiser2 = LastCall.GetEventRaiser();

            ITaskFactory<ILocateTask> factory = new TaskFactoryStub<ILocateTask>(new List<ILocateTask> { task, task2 });
            search.TaskFactory = factory;

            _mocks.ReplayAll();
            IEnumerable<IResultSet> results = null;
            search.LocateComplete += (s, e) => { results = e.Results; };
            search.LocateAsync("bob");
            raiser.Raise(task, new TaskResultEventArgs(new List<IResultSet> { new ResultSet() }, null));
            raiser2.Raise(task2, new TaskResultEventArgs(new List<IResultSet> { new ResultSet() }, null));
            Assert.IsNotNull(results);
            // Test for two results which indicates that the LocateCompleted event fired after both tasks completed
            Assert.AreEqual(results.Count(), 2);
        }
    }

}