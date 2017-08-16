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
            var previousRun = GetPreviousRun(type.Run);
            var previousRunType = previousRun?.Types_.FirstOrDefault(t => t.FullTypeName == type.FullTypeName);
            IEnumerable<BenchmarkType> comparisonTypes = repository
                .GetTypesByCommitAndType(type.Run.Commit, type.FullTypeName)
                .Where(t => t != type)
                .OrderBy(t => t.Environment.Machine)
                .ThenBy(t => t.Environment.TargetFramework)
                .ThenBy(t => t.Environment.RuntimeVersion)
                .ToList();
            return View((type, previousRunType, comparisonTypes));
        }

        [Route("/benchmarks/types/{leftTypeId}/compare/{rightTypeId}")]
        public IActionResult CompareTypes(string leftTypeId, string rightTypeId)
        {
            var left = repository.GetType(leftTypeId);
            var right = repository.GetType(rightTypeId);
            return View(new CompareTypesViewModel(left, right));
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
