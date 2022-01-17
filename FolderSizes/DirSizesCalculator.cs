using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace FolderSizes
{
	public class DirSizesCalculator
	{
		public delegate void ResultCallback(long totalSize, string dirPath);

		private BlockingCollection<DirSizeTask> tasks;
		private readonly int maxThreads = Environment.ProcessorCount;
		private HashSet<Thread> threads;
		private bool abortTasks = false;
		private CancellationTokenSource cancelSource;

		public DirSizesCalculator()
		{
			tasks = new BlockingCollection<DirSizeTask>();
			threads = new HashSet<Thread>();
			cancelSource = new CancellationTokenSource();

			for (int i = 0; i < maxThreads; i++)
			{
				Thread t = new Thread(DoTasks);
				threads.Add(t);
				t.Start();
			}
		}

		/// <summary>
		/// Start calculating the size of a directory.
		/// </summary>
		/// <param name="dirPath"></param>
		/// <param name="callback"></param>
		public void StartTask(string dirPath, ResultCallback callback)
		{
			DirSizeTask task = new DirSizeTask(dirPath, callback);
			tasks.Add(task);
		}

		public int GetNumTasks()
		{
			return tasks.Count;
		}

		/// <summary>
		/// Signal for tasks to abort, and wait until all threads are idle.
		/// </summary>
		public void AbortTasksAndWait()
		{
			abortTasks = true;

			while (tasks.Count != 0)
				Thread.Sleep(0); // give up current time slice

			abortTasks = false;
		}

		/// <summary>
		/// Destroy all threads of the calculator.
		/// </summary>
		public void Terminate()
		{
			cancelSource.Cancel();
		}

		/// <summary>
		/// Process tasks.
		/// </summary>
		private void DoTasks()
		{
			while (true)
			{
				DirSizeTask task;

				try
				{
					// wait for a task
					task = tasks.Take(cancelSource.Token);
				}
				catch (OperationCanceledException)
				{
					// operation is canceled when Terminate is called, so stop the thread
					break;
				}

				// if aborting tasks, discard current task
				if (abortTasks)
					continue;

				string dirPath;
				// lock to ensure that subdirs cannot go down without active workers going up
				lock (task)
				{
					// give up if there are no subdirs for whatever reason (shouldn't happen)
					if (!task.subdirsToCalculate.TryDequeue(out dirPath))
						continue;

					task.activeWorkers++;
				}

				IEnumerable<string> filePaths;
				IEnumerable<string> subdirPaths;

				try
				{
					filePaths = Directory.EnumerateFiles(dirPath);
					subdirPaths = Directory.EnumerateDirectories(dirPath);
				}
				catch (UnauthorizedAccessException)
				{
					// treat inaccessible directories as having 0 size
					filePaths = Enumerable.Empty<string>();
					subdirPaths = Enumerable.Empty<string>();
				}

				foreach (string filePath in filePaths)
				{
					long fileSize = new FileInfo(filePath).Length;
					Interlocked.Add(ref task.totalSize, fileSize);
				}

				foreach (string subdirPath in subdirPaths)
				{
					task.subdirsToCalculate.Enqueue(subdirPath);
					// add one task for every new subdir
					tasks.Add(task);
				}

				// lock to ensure active workers and subdirs won't change while checking if task is done
				lock (task)
				{
					task.activeWorkers--;

					// if there are no more subdirs and no one else is working on the task, we can report the result
					if (task.subdirsToCalculate.Count == 0 && task.activeWorkers == 0)
						task.callback(task.totalSize, task.topDirPath);
				}

				// i spent more than 12 hours and stayed up to 3 am making this work
			}
		}

		/// <summary>
		/// Holds data for a task of calculating the total size of one directory.
		/// </summary>
		private class DirSizeTask
		{
			public string topDirPath;
			public long totalSize = 0;
			public ResultCallback callback;
			public ConcurrentQueue<string> subdirsToCalculate;
			public int activeWorkers = 0;

			public DirSizeTask(string topDirPath, ResultCallback callback)
			{
				this.topDirPath = topDirPath;
				this.callback = callback;
				subdirsToCalculate = new ConcurrentQueue<string>();
				subdirsToCalculate.Enqueue(topDirPath);
			}
		}
	}
}
