#  Parallel and Distributed Programming (PDP)

## Overview

This repository includes all solved lab assignments for the *Parallel and Distributed Programming* (PDP) course at UBB. The labs explore core concepts in multithreading, parallel algorithms, futures and continuations, distributed systems, and GPU programming. Each assignment builds practical experience in concurrent programming, synchronization, message passing, and task-level parallelism using modern APIs and paradigms.

## Technologies Used

- **Languages**: C, C++, Java, Python (depending on lab)
- **Libraries/Frameworks**:
  - POSIX threads (pthreads)
  - Java Concurrency (Executors, Futures, Continuations)
  - OpenMP (C/C++)
  - MPI (Message Passing Interface)
  - OpenCL (GPU programming)
  - Sockets (TCP networking)



## Assignments Summary

###  Lab 1 – Non-Cooperative Multithreading
- Created threads using `pthread_create`
- Passed arguments and synchronized thread joins
- Observed uncontrolled interleaving and race conditions

###  Lab 2 – Producer–Consumer Problem
- Implemented bounded buffer using mutexes and condition variables
- Applied wait/signal logic with multiple producers and consumers
- Ensured safe concurrent access and avoidance of deadlocks

###  Lab 3 – Parallel Sum and Computation
- Split computation tasks among multiple threads
- Applied static/dynamic load balancing
- Explored performance scaling via thread pools

###  Lab 4 – Futures and Continuations
- Used Java `ExecutorService` and `Future` for asynchronous computation
- Chained tasks using `thenApply`/`thenAccept` (CompletableFuture)
- Implemented parallel dependencies with structured callbacks

###  Lab 5 – Classic Parallel Algorithms I
- Implemented parallel prefix sum using OpenMP
- Parallelized merge sort and reduction algorithms
- Compared time complexity vs sequential implementation

###  Lab 6 – Classic Parallel Algorithms II
- Optimized matrix multiplication using tiling and OpenMP
- Applied parallel graph traversal algorithms (BFS, DFS)
- Benchmarked nested parallelism and loop collapsing

###  Lab 7 – Message Passing (MPI)
- Used `mpicc` and `mpirun` for distributed programs
- Built distributed matrix multiplication with point-to-point and collective communication
- Explored MPI communicators, barriers, and ranks

###  Lab 8 – Distributed Systems over TCP
- Built a client–server model using sockets in C
- Enabled message-based communication and request-response semantics
- Added concurrency support for multiple clients using `fork` or threading

###  Lab 9 – GPU Programming with OpenCL
- Initialized OpenCL platforms, contexts, and devices
- Wrote and compiled OpenCL kernel functions
- Ran data-parallel programs on GPU: vector addition, image filtering

