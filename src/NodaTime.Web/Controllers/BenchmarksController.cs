// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using Microsoft.AspNetCore.Mvc;
using NodaTime.Benchmarks;
using NodaTime.Web.Models;
using NodaTime.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.Web.Controllers
{
    public class BenchmarksController : Controller
    {
        private readonly IBenchmarkRepository repository;

        public BenchmarksController(IBenchmarkRepository repository)
        {
            this.repository = repository;
        }

        [Route("/benchmarks")]
        public IActionResult Index() => View(repository.ListEnvironments());

        [Route("/benchmarks/environments/{id}")]
        public IActionResult ViewEnvironment(string id) => View(repository.GetEnvironment(id));

        [Route("/benchmarks/runs/{runId}")]
        public IActionResult ViewRun(string runId) => View(repository.GetRun(runId));

        [Route("/benchmarks/types/{typeId}")]
        public IActionResult ViewType(string typeId)
        {
            var type = repository.GetType(typeId);
            var previousCommit = GetPreviousRun(type.Run)?.Commit;
            return View((type, previousCommit));
        }

        // TODO: Revisit these URLs. They're not terribly nice. I tried using colons (e.g. /{typeId}:compareEnvironments)
        // which also isn't great, but indicates it's somewhat less of a resource... it worked locally, but not on Azure.

        [Route("/benchmarks/types/{typeId}/compareEnvironments")]
        public IActionResult CompareTypesByEnvironment(string typeId)
        {
            var left = repository.GetType(typeId);
            var runs = repository.ListEnvironments()
                .Select(e => e.Runs.FirstOrDefault(r => r.Commit == left.Run.Commit))
                .Where(r => r != null && r != left.Run)
                .Select(r => r.Types_.FirstOrDefault(t => t.FullTypeName == left.FullTypeName))
                .ToList();
            // Always make the selected run the first one.
            runs.Insert(0, left);
            return View(new CompareTypesByEnvironmentViewModel(runs));
        }


        [Route("/benchmarks/types/{leftTypeId}/compareWithCommit/{commit}")]
        public IActionResult CompareTypesByCommit(string leftTypeId, string commit)
        {
            var leftType = repository.GetType(leftTypeId);
            var environment = leftType.Environment;
            var run = environment.Runs.FirstOrDefault(r => r.Commit == commit);
            if (run == null)
            {
                return NotFound();
            }
            var rightType = run.Types_.FirstOrDefault(t => t.FullTypeName == leftType.FullTypeName);
            if (rightType == null)
            {
                return NotFound();
            }
            return View(new CompareTypesByCommitViewModel(leftType, rightType));
        }

        [Route("/benchmarks/benchmarks/{benchmarkId}")]
        public IActionResult ViewBenchmark(string benchmarkId) => View(repository.GetBenchmark(benchmarkId));

        [Route("/benchmarks/benchmarks/{benchmarkId}/history")]
        public IActionResult ViewBenchmarkHistory(string benchmarkId)
        {
            // Use the provided benchmark as the latest one to use
            var latest = repository.GetBenchmark(benchmarkId);
            var benchmarks =
                from run in latest.Environment.Runs.SkipWhile(r => r != latest.Run)
                from type in run.Types_ where type.FullTypeName == latest.Type.FullTypeName
                from benchmark in type.Benchmarks where benchmark.Method == latest.Method
                select benchmark;

            return View(benchmarks.ToList());
        }        

        private BenchmarkRun GetPreviousRun(BenchmarkRun run) =>
            repository.GetEnvironment(run.BenchmarkEnvironmentId).Runs
                .SkipWhile(r => r.BenchmarkRunId != run.BenchmarkRunId)
                .Skip(1)
                .FirstOrDefault();            
    }
}
