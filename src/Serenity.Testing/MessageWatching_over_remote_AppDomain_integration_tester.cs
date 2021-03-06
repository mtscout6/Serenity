﻿using System;
using Bottles.Services.Messaging.Tracking;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using RemoteService;

namespace Serenity.Testing
{
    [TestFixture]
    public class MessageWatching_over_remote_AppDomain_integration_tester
    {
        [Test]
        public void listens_and_finishes_after_receiving_the_all_clear()
        {

            // trying to get the asset pipeline to shut up about
            // non-existent assets
            var settings = new ApplicationSettings
            {
                PhysicalPath = Environment.CurrentDirectory
                    .ParentDirectory().ParentDirectory().ParentDirectory()
                    .AppendPath("RemoteService")
            };

            var system = new FubuMvcSystem(settings, () => FubuApplication.DefaultPolicies().StructureMap().Bootstrap());

            system.AddRemoteSubSystem("Remote", x => {
                x.UseParallelServiceDirectory("RemoteService");
            });

            using (var context = system.CreateContext())
            {
                system.StartListeningForMessages();
                var message = new RemoteGo();
                MessageHistory.Record(MessageTrack.ForSent(message));

                var waitForWorkToFinish = MessageHistory.WaitForWorkToFinish(() => {
                    system.RemoteSubSystemFor("Remote").Runner.SendRemotely(message);
                }, timeoutMilliseconds:30000);
                waitForWorkToFinish.ShouldBeTrue();
            }
        }
    }
}