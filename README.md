# COP5615 Distributed Operating System Principles Project 3

## Chord-P2P-System-And-Simulation-Actor-Model-F#-Akka.Net

## Project Description

You have to implement the network join and routing as described in the Chord paper (Section 4) and encode the simple application that associates a key (same as the ids used in Chord) with a string. You can change the message type sent and the specific activity as long as you implement it using a similar API to the one described in the paper. <br>
Link: https://pdos.csail.mit.edu/papers/ton:chord/paper-ton.pdf

## Project Requirements

### Input

The input provided (as command line) will be of the form: numberOfNodes numberOfRequests Where numberOfNodes is the number of peers to be created in the peer-to-peer system and numberOfRequests is the number of requests each peer has to make. When all peers performed that many requests, the program can exit. Each peer should send a request/second.

### Actor modeling

In this project, you have to use exclusively the AKKA actor framework (projects that do not use multiple actors or use any other form of parallelism will receive no credit).  You should have one actor for each of the peers modeled.

### Output

Print the average number of hops (node connections) that have to be traversed to deliver a message.

### Example:


```F#
dotnet fsi Project3.fsx 2500 100
```
```F#
Average number of hops (node connections) that have to be traversed to deliver a message : 5.295244
```

## Submitted By:

Name: Parth Gupta, UFID: 91997064

## What is Working?

- Implemented the network join and routing as described in the Chord paper and encoded the application that associates a key with a string.
- After implementation of Chord Algorithm, I am printing the average hop count that has to be traversed to deliver a message.

## Some Results of my Algorithm:

| Number of Clients | Time taken to register a user | Time taken to subscribe in Zipf Dist. | Time taken to tweet initially | Time taken to perform random operations (tweet, retweet, etc) |
| --------------- | ------------------ | ------------------ | ------------------ | ------------------ |
| 100 | 10 | 2.936000 |
| 500 | 10 | 4.062600 | 
| 1000 | 10 | 4.565500 |
| 500 | 50 | 4.064400 |
| 1000 | 50 | 4.565960 |
| 1000 | 100 | 4.590240 |
| 1500 | 100 | 4.948467 |
| 2500 | 100 | 5.295244 |
| 2500 | 150 | 5.369304 |
| 3000 | 150 | 5.437124 |
| 4500 | 200 | 5.760827 |
| 5000 | 200 | 5.795034 |
| 5500 | 200 | 5.920152 |

## The largest network that I managed to deal with:

For this project, the largest network that I was able to manage was for 15000 users.

```F#
[INFO][30-11-2021 22:49:45][Thread 0008][remoting (akka://Twitter)] Remoting shut down
[INFO][30-11-2021 22:49:45][Thread 0022][remoting-terminator] Remoting shut 
down.
**********************************************************************      
The time taken to register 15000 users is 16300.514000
The time taken to subscribe in Zipf distance for 15000 users is 58712.691900 
The time taken to send tweets for 15000 users is 15378.415000
The time taken to perform random operations for 15000 users is 289675.220000  
**********************************************************************      
PS C:\Users\Parth Gupta\desktop\Project 4>
```

## Built On

- Programming language: F# 
- Framework: AKKA.NET
- Operating System: Windows 10
- Programming Tool: Visual Studio Code
